using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        RowLock row_lock = new RowLock();
        RowsLock rows_lock = new RowsLock();
        /// <summary>
        /// 读取缓存中的数据对象
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="args">返回的字段</param>
        /// <returns></returns>
        public T GetCache(long id, params string[] args)
        {
            if (id < 0) return null;
            T row = null;
            if (RowCache.Get(id, ref row)) return Get(row, args);

            row_lock.id = id;
            lock (row_lock)
            {
                if (RowCache.Get(id, ref row)) return Get(row, args);

                var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, " WHERE ", DB.GetName("id"), "=", id, " limit 1;");
                row = Get(_sql, null);

                if (row == null)
                    RowCache.Add(id, null, 5);
                else
                    RowCache.Add(id, row);
            }
            return Get(row, args);
        }
        public T GetCache(Sql cond, params string[] args)
        {
            var ids = GetCacheIds(cond);
            if (ids == null) return null;
            return GetCache(ids[0], args);
        }
        /// <summary>
        /// 读取实时数据对象
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="args">返回的字段</param>
        /// <returns></returns>
        public T Get(long id, params string[] args)
        {
            if (id < 0) return null;
            var _sql = string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetColumnString(args), FromTableString, " WHERE ", DB.GetName("id"), "=", id, " limit 1;");
            return Get(_sql, null, args);
        }
        /// <summary>
        /// 读取数据对象
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public T Get(T bean)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }

            var _Params = new List<DbParameter>();
            var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, GetWhereCondition(bean, _Params), " limit 1;");

            return Get(_sql, _Params);
        }

        public T Get(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, cond.GetWhereString(), " limit 1;");
                return Get(_sql, cond.Params);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }
        public object GetSingle(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, cond.GetWhereString(), " limit 1;");
                return GetSingle(_sql, cond.Params);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }

        /// <summary>
        /// 读取前size个对象
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="orderby">排序</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public List<T> Gets(T bean, string orderby = null, uint count = 1000)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }

            var _Params = new List<DbParameter>();
            var _sql = new StringBuilder(string.Concat("SELECT ", SelectColumnString, FromTableString, GetWhereCondition(bean, _Params)));
            if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
            if (count > 0) _sql.Append(" LIMIT " + count);

            return Gets(_sql, _Params);
        }
        public List<T> Gets(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", cond.Columns.Count == 0 ? SelectColumnString : GetColumnString(cond.Columns), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return Gets(_sql, cond.Params, cond.Columns);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }
        public List<T> GetCaches(Sql cond, params string[] args)
        {
            var ids = GetCacheIds(cond);
            if (ids == null) return null;
            var _list = new List<T>();
            T _obj = null;
            foreach (var id in ids)
            {
                _obj = GetCache(id, args);
                if (_obj == null) continue;
                _list.Add(_obj);
            }
            return _list;
        }
        /// <summary>
        /// 读取ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public long[] GetIds(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return GetIds(_sql, cond.Params);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }
        /// <summary>
        /// 读取前size个ID
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="orderby">排序</param>
        /// <param name="count"></param>
        /// <returns>数量</returns>
        public long[] GetIds(T bean, string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }
            long[] ids = null;
            string ck = CombineCacheKey(bean, orderby, count);
            if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

            rows_lock.key = ck;
            lock (rows_lock)
            {
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                var _Params = new List<DbParameter>();
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, GetWhereCondition(bean, _Params)));
                if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
                if (count > 0) _sql.Append(" LIMIT " + count);

                ids = GetIds(_sql, _Params);

                if (from != CacheTypes.None)
                {
                    if (ids == null) ListCache.Add(ck, null, 5);
                    else ListCache.Add(ck, ids);
                }
                from = CacheTypes.From;
            }
            return ids;
        }
        /// <summary>
        /// 读取缓存的ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public long[] GetCacheIds(Sql cond, CacheTypes from = CacheTypes.From)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                long[] ids = null;
                string ck = CombineCacheKey(cond);
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                rows_lock.key = ck;
                lock (rows_lock)
                {
                    if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                    ids = GetIds(cond);

                    if (from != CacheTypes.None)
                    {
                        if (ids == null)
                            ListCache.Add(ck, null, 5);
                        else
                            ListCache.Add(ck, ids);
                    }
                    from = CacheTypes.From;
                }
                return ids;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public bool Exist(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                cond.Top(1);
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetPageString());
                return Exist(_sql, cond.Params);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(long id)
        {
            if (GetCache(id) == null)
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, " WHERE ", DB.GetName("id"), "=", id, " limit 1;"));
                return Exist(_sql, null);
            }
            return true;
        }
        /// <summary>
        /// 统计符合条件的记录数
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public int Count(Sql cond)
        {
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT COUNT(*)", FromTableString));
                if (cond != null)
                    _sql.Append(cond.GetWhereString());

                return Count(_sql, cond.Params);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                cond.Dispose();
            }
        }
    }

    public class RowLock
    {
        public long id { set; get; }
    }
    public class RowsLock
    {
        public string key { set; get; }
    }
}
