using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using adeway;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        static RowLock row_lock = new RowLock();
        static CondRowLock condrow_lock = new CondRowLock();
        static RowsLock rows_lock = new RowsLock();
        static IdsLock ids_lock = new IdsLock();

        #region 单个数据对象
        /// <summary>
        /// 读取实时数据对象
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="args">返回的字段</param>
        /// <returns></returns>
        public T Get(long id, params string[] args)
        {
            if (id < 0) return null;
            var _sql = string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetSelectColumnStringByArgs(args), FromTableString, " WHERE ", DB.SetColumnFunc("id", id), " limit 1;");
            return _Get(_sql, null, args);
        }
        //[Obsolete]
        /// <summary>
        /// 读取缓存中的数据对象的指定字段
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="args">返回的字段</param>
        /// <returns></returns>
        public T GetCache(long id, params string[] args)
        {
            if (id < 0) return null;
            T row = null;
            if (RowCache.Get(id, ref row)) return _Get(row, args);

            row_lock.key = id;
            lock (row_lock)
            {
                if (RowCache.Get(id, ref row)) return _Get(row, args);
                row = Get(id);
                if (row == null)
                    RowCache.Add(id, null, 5);
                else
                    RowCache.Add(id, row);
            }
            return _Get(row, args);
        }
        public T GetCache(long id, CacheTypes from = CacheTypes.From, params string[] args)
        {
            if (id < 0) return null;
            if (from == CacheTypes.None) { return Get(id, args); }

            T row = null;
            if (from == CacheTypes.From && RowCache.Get(id, ref row)) return _Get(row, args);

            row_lock.key = id;
            lock (row_lock)
            {
                if (from == CacheTypes.From && RowCache.Get(id, ref row)) return _Get(row, args);
                row = Get(id);
                if (row == null)
                    RowCache.Add(id, null, 5);
                else
                    RowCache.Add(id, row);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return _Get(row, args);
        }

        public T Get(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = string.Concat("SELECT ", GetSelectColumnString(cond), FromTableString, cond.GetWhereString(), cond.GetGroupbyString(), cond.GetOrderbyString(), " limit 1;");
                return _Get(_sql, cond.Params, cond.Columns);
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        public T GetCache(Sql cond, CacheTypes from = CacheTypes.From)
        {
            if (from == CacheTypes.None) { return Get(cond); }

            T row = null;
            string ck = CombineCacheKey(cond);
            if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

            condrow_lock.key = ck;
            lock (condrow_lock)
            {
                if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

                row = Get(cond);

                if (row == null)
                    CondRowCache.Add(ck, null, 5);
                else
                    CondRowCache.Add(ck, row);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return _Clone(row);
        }
        /// <summary>
        /// 读取数据对象
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public T Get(T bean, params string[] args)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }

            var _Params = new List<DbParameter>();
            var _sql = string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetSelectColumnStringByArgs(args), FromTableString, GetWhereCondition(bean, _Params), " limit 1;");

            return _Get(_sql, _Params);
        }
        /// <summary>
        /// 读取数据对象
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public T GetCache(T bean, CacheTypes from = CacheTypes.From)
        {
            if (from == CacheTypes.None) { return Get(bean); }

            T row = null;
            string ck = CombineCacheKey(bean);
            if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

            condrow_lock.key = ck;
            lock (condrow_lock)
            {
                if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

                row = Get(bean);

                if (row == null)
                    CondRowCache.Add(ck, null, 5);
                else
                    CondRowCache.Add(ck, row);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return _Clone(row);
        }
        /// <summary>
        /// 读取数据对象
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="col">字段名</param>
        /// <param name="val">字段值</param>
        /// <returns></returns>
        public T Get<V>(string col, V val)
        {
            var _Params = new List<DbParameter>();
            var _sql = string.Concat("SELECT ", SelectColumnString, FromTableString, " WHERE ", GetCondition(col, val, _Params), " limit 1;");
            return _Get(_sql, _Params);
        }
        public T GetCache<V>(string col, V val, CacheTypes from = CacheTypes.From)
        {
            if (from == CacheTypes.None) { return Get(col, val); }

            T row = null;
            string ck = CombineCacheKey(col, val);
            if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

            condrow_lock.key = ck;
            lock (condrow_lock)
            {
                if (from == CacheTypes.From && CondRowCache.Get(ck, ref row)) return _Clone(row);

                row = Get(col, val);

                if (row == null)
                    CondRowCache.Add(ck, null, 5);
                else
                    CondRowCache.Add(ck, row);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return _Clone(row);
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
        public List<T> Gets(T bean, string orderby = null, uint count = 1000, params string[] args)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }

            var _Params = new List<DbParameter>();
            var _sql = new StringBuilder(string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetSelectColumnStringByArgs(args), FromTableString, GetWhereCondition(bean, _Params)));
            if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
            if (count > 0) _sql.Append(" LIMIT " + count);

            return _Gets(_sql.ToString(), _Params);
        }
        public List<T> GetCaches(T bean, string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From, params string[] args)
        {
            var ids = GetCacheIds(bean, orderby, count, from);
            if (ids == null) return null;
            var _list = new List<T>();
            T _obj = null;
            foreach (var id in ids)
            {
                _obj = GetCache(id, from, args);
                if (_obj == null) continue;
                _list.Add(_obj);
            }
            return _list;
        }

        public List<T> Gets(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", GetSelectColumnString(cond), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetGroupbyString());
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return _Gets(_sql.ToString(), cond.Params, cond.Columns);
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 读取缓存的数据列表(缺省读取前1000个)
        /// (只用于有唯一主键ID的数据表或视图)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<T> GetCaches(Sql cond, CacheTypes from = CacheTypes.From)
        {
            var args = cond == null ? new string[0] : cond.Columns.ToArray();
            var ids = GetCacheIds(cond, from);
            if (ids == null) return null;
            var _list = new List<T>();
            T _obj = null;
            foreach (var id in ids)
            {
                _obj = GetCache(id, from, args);
                if (_obj == null) continue;
                _list.Add(_obj);
            }
            return _list;
        }
        public List<T> Gets<V>(string col, V val, params string[] args)
        {
            var _Params = new List<DbParameter>();
            var _sql = new StringBuilder(string.Concat("SELECT ", args.Length == 0 ? SelectColumnString : GetSelectColumnStringByArgs(args), FromTableString, " WHERE ", GetCondition(col, val, _Params)));

            return _Gets(_sql.ToString(), _Params);
        }
        /// <summary>
        /// 读取缓存的数据列表(缺省读取前1000个)
        /// (只用于有唯一主键ID的数据表或视图)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public List<T> GetCaches<V>(string col, V val, string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From, params string[] args)
        {
            var ids = GetCacheIds(col, val, orderby, count, from);
            if (ids == null) return null;
            var _list = new List<T>();
            T _obj = null;
            foreach (var id in ids)
            {
                _obj = GetCache(id, from, args);
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
            if (cond == null) { return GetIds(); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetGroupbyString());
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return _GetIds(_sql, cond.Params);
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 读取缓存的ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public long[] GetCacheIds(Sql cond = null, CacheTypes from = CacheTypes.From)
        {
            try
            {
                if (from == CacheTypes.None) { return GetIds(cond); }

                long[] ids = null;
                string ck = CombineCacheKey(cond);
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

                ids_lock.key = ck;
                lock (ids_lock)
                {
                    if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;
                    ids = GetIds(cond);
                    if (ids == null)
                        ListCache.Add(ck, null, 5);
                    else
                        ListCache.Add(ck, ids);
                    // 让等待中的线程直接读取缓存
                    from = CacheTypes.From;
                }
                return ids;
            }
            catch { throw; }
            finally { }
        }

        /// <summary>
        /// 读取ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public long[] GetIds<V>(string col, V val, string orderby = null, uint count = 1000)
        {
            if (col == null || col.Length < 1 || val == null) { return GetIds(); }

            var _Params = new List<DbParameter>();
            var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, " WHERE ", GetCondition(col, val, _Params)));
            if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
            if (count > 0) _sql.Append(" LIMIT " + count);

            return _GetIds(_sql, _Params);
        }
        /// <summary>
        /// 读取缓存的ids(缺省读取前1000个)
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public long[] GetCacheIds<V>(string col, V val, string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From)
        {
            if (from == CacheTypes.None) { return GetIds(col, val, orderby, count); }

            long[] ids = null;
            string ck = CombineCacheKey(col, val, orderby, count);
            if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

            ids_lock.key = ck;
            lock (ids_lock)
            {
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;
                ids = GetIds(col, val, orderby, count);
                if (ids == null)
                    ListCache.Add(ck, null, 5);
                else
                    ListCache.Add(ck, ids);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return ids;
        }


        public long[] GetIds(string orderby = null, uint count = 1000)
        {
            var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString));
            if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
            if (count > 0) _sql.Append(" LIMIT " + count);
            return _GetIds(_sql, null);
        }
        public long[] GetCacheIds(string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From)
        {
            if (from == CacheTypes.None) { return GetIds(orderby, count); }

            long[] ids = null;
            string ck = CombineCacheKey(orderby, count);
            if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

            ids_lock.key = ck;
            lock (ids_lock)
            {
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;
                ids = GetIds(orderby, count);
                if (ids == null)
                    ListCache.Add(ck, null, 5);
                else
                    ListCache.Add(ck, ids);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return ids;
        }

        /// <summary>
        /// 读取前size个ID
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="orderby">排序</param>
        /// <param name="count"></param>
        /// <returns>数量</returns>
        public long[] GetIds(T bean, string orderby = null, uint count = 1000)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }
            var _Params = new List<DbParameter>();
            var _sql = new StringBuilder(string.Concat("SELECT ", DB.GetName("id"), FromTableString, GetWhereCondition(bean, _Params)));
            if (!string.IsNullOrWhiteSpace(orderby)) _sql.Append(" ORDER BY " + orderby);
            if (count > 0) _sql.Append(" LIMIT " + count);

            return _GetIds(_sql, _Params);
        }
        public long[] GetCacheIds(T bean, string orderby = null, uint count = 1000, CacheTypes from = CacheTypes.From)
        {
            if (bean == null) { throw new Exception("bean 不能为 NULL"); }
            if (from == CacheTypes.None) { return GetIds(bean, orderby, count); }

            long[] ids = null;
            string ck = CombineCacheKey(bean, orderby, count);
            if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;

            rows_lock.key = ck;
            lock (rows_lock)
            {
                if (from == CacheTypes.From && ListCache.Get(ck, ref ids)) return ids;
                ids = GetIds(bean, orderby, count);
                if (ids == null) ListCache.Add(ck, null, 5);
                else ListCache.Add(ck, ids);
                // 让等待中的线程直接读取缓存
                from = CacheTypes.From;
            }
            return ids;
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
                var _sql = string.Concat("SELECT ", GetSelectColumnString(cond), FromTableString, cond.GetWhereString(), cond.GetGroupbyString(), cond.GetOrderbyString(), " limit 1;");
                return Db.ReadSingle(_sql, cond.Params);
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
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
        public object GetSingle(string sql)
        {
            return Db.ReadSingle(sql);
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public bool Exists(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = string.Concat("SELECT ", DB.GetName("id"), FromTableString, cond.GetWhereString());
                //return _Exists(_sql, cond.Params);
                return Db.ReadSingle(_sql, cond.Params) != null;
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        public bool Exists<V>(string col, V val)
        {
            try
            {
                return Db.ReadSingle(string.Concat("SELECT ", DB.GetName("id"), FromTableString, " where ", DB.GetCondition(col)), DB.GetParam(col, val)) != null;
            }
            catch { throw; }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(long id)
        {
            if (GetCache(id, CacheTypes.From) == null)
            {
                var _sql = string.Concat("SELECT ", DB.GetName("id"), FromTableString, " WHERE ", DB.GetName("id"), "=", id, " limit 1;");
                return Db.ReadSingle(_sql) != null;
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
                {
                    _sql.Append(cond.GetWhereString());
                    _sql.Append(cond.GetGroupbyString());
                }

                return Db.ReadSingle(_sql.ToString(), cond.Params).ToInt();
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        #endregion

        #region 原生查询
        /// <summary>
        /// 原生SQL方式读取实时数据对象
        /// 例如：select * from users where id=?
        /// </summary>
        /// <param name="sqlString">原生sql字符串</param>
        /// <param name="args">查询条件值,和sql字符串中的？号对应</param>
        /// <returns></returns>
        public T GetRaw(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
            {
                throw new Exception("数据库查询字符串不能为空");
            }

            var cmd = DB.GetRawSql(sqlString, args);
            try
            {
                return _Get(cmd.SqlString, cmd.SqlParams);
            }
            catch (Exception)
            {
                throw;
            }
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

            var cmd = DB.GetRawSql(sqlString, args);
            try
            {
                return _Gets(cmd.SqlString, cmd.SqlParams);
            }
            catch { throw; }
        }
        public DataTable Query(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var _sql = new StringBuilder(string.Concat("SELECT ", GetSelectColumnString(cond), FromTableString, cond.GetWhereString()));
                _sql.Append(cond.GetGroupbyString());
                _sql.Append(cond.GetOrderbyString());
                _sql.Append(cond.GetPageString());

                return Db.Query(_sql.ToString(), cond.Params);
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 原生查询
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public DataTable Query(string sqlString)
        {
            return Db.Query(sqlString);
        }
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
        #endregion
    }

    public class RowLock
    {
        public long key { set; get; }
    }
    public class CondRowLock
    {
        public string key { set; get; }
    }
    public class RowsLock
    {
        public string key { set; get; }
    }
    public class IdsLock
    {
        public string key { set; get; }
    }
}
