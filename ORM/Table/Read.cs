using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        static RowLock row_lock = new RowLock();
        static RowsLock rows_lock = new RowsLock();

        #region 单个数据对象
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
                row = Get(_sql, args);

                if (row == null)
                    RowCache.Add(id, null, 5);
                else
                    RowCache.Add(id, row);
            }
            return Get(row, args);
        }
        public T GetCache(Sql cond)
        {
            var ids = GetCacheIds(cond);
            if (ids == null) return null;
            return GetCache(ids[0], cond.Columns.ToArray());
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
            var _sql = string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetSelectColumnStringFromArgs(args), FromTableString, " WHERE ", DB.SetColumnFunc("id", id), " limit 1;");
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
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 原生SQL方式读取实时数据对象
        /// 例如：select * from users where id=?
        /// </summary>
        /// <param name="sqlString">原生sql字符串</param>
        /// <param name="args">查询条件值,和sql字符串中的？号对应</param>
        /// <returns></returns>
        public T Get(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
            {
                throw new Exception("数据库查询字符串不能为空");
            }

            var Params = new List<DbParameter>();
            var sql = DB.GetRawSql(sqlString, ref Params, args);
            try
            {
                return Get(sql, Params);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 多条数据
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

            return Gets(_sql.ToString(), _Params);
        }
        public List<T> Gets(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", cond.Columns.Count == 0 ? SelectColumnString : GetSelectColumnStringFromArgs(cond.Columns), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return Gets(_sql.ToString(), cond.Params, cond.Columns);
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 原生SQL方式读取实时数据对象列表
        /// 例如：select * from users where age>=? and sex=?
        /// </summary>
        /// <param name="sqlString">原生sql字符串</param>
        /// <param name="args">查询条件值,和sql字符串中的？号对应</param>
        /// <returns></returns>
        public List<T> Gets(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
            {
                throw new Exception("数据库查询字符串不能为空");
            }

            var Params = new List<DbParameter>();
            var sql = DB.GetRawSql(sqlString, ref Params, args);
            try
            {
                return Gets(sql, Params);
            }
            catch { throw; }
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
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
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
            if (from == CacheTypes.None)
            {
                var _Params = new List<DbParameter>();
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, GetWhereCondition(bean, _Params)));
                if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
                if (count > 0) _sql.Append(" LIMIT " + count);

                return GetIds(_sql, _Params);
            }

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

                if (ids == null) ListCache.Add(ck, null, 5);
                else ListCache.Add(ck, ids);

                from = CacheTypes.From;
            }
            return ids;
        }
        /// <summary>
        /// 读取缓存的数据列表(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<T> GetCaches(Sql cond)
        {
            var ids = GetCacheIds(cond);
            if (ids == null) return null;
            var _list = new List<T>();
            T _obj = null;
            var args = cond.Columns.ToArray();
            foreach (var id in ids)
            {
                _obj = GetCache(id, args);
                if (_obj == null) continue;
                _list.Add(_obj);
            }
            return _list;
        }
        /// <summary>
        /// 读取缓存的ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public long[] GetCacheIds(Sql cond, CacheTypes from = CacheTypes.From)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                if (from == CacheTypes.None) { return GetIds(cond); }

                long[] ids = null;
                string ck = CombineCacheKey(cond);
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                rows_lock.key = ck;
                lock (rows_lock)
                {
                    if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                    ids = GetIds(cond);

                    if (ids == null)
                        ListCache.Add(ck, null, 5);
                    else
                        ListCache.Add(ck, ids);

                    from = CacheTypes.From;
                }
                return ids;
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        #endregion

        #region  读取首行首列数据
        /// <summary>
        /// 读取首行首列数据
        /// </summary>
        /// <param name="cond">Snow.Orm.Sql 读取条件</param>
        /// <returns></returns>
        public object GetSingle(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, cond.GetWhereString(), " limit 1;");
                return Db.ReadSingle(_sql, cond.Params);
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 读取指定id的指定列数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public object GetSingle(long id, string col)
        {
            if (id < 0) return null;
            var _sql = string.Concat("SELECT ", DB.GetName(col), FromTableString, " WHERE ", DB.SetColumnFunc("id", id), " limit 1;");
            return Db.ReadSingle(_sql);
        }
        #endregion

        #region 其他方法
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
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
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
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        #endregion

        /// <summary>
        /// 执行原生查询
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable Query(string sqlString, params object[] args)
        {
            return Db.Query(sqlString, args);
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
