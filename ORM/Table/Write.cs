using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        public bool Insert(T bean, ref long id)
        {
            if (bean == null || bean.Count == 0) { throw new Exception("bean 不能为 NULL"); }
            List<string> _Fields = new List<string>();
            List<string> _Values = new List<string>();
            var _Params = new List<DbParameter>();
            foreach (var item in bean)
            {
                if (_Columns.Contains(item.Key))
                {
                    _Fields.Add(DB.GetName(item.Key));
                    _Values.Add(DB._ParameterPrefix + item.Key);
                    _Params.Add(DB.GetParam(item.Key, item.Value.ToString()));
                }
            }
            var sql = "INSERT INTO " + TableString + " (" + string.Join(",", _Fields) + ") VALUES(" + string.Join(",", _Values) + "); select ROW_COUNT(),LAST_INSERT_ID();";
            try
            {
                if (Db.Insert(sql, _Params, ref id))
                {
                    ListCache.Clear();
                    return true;
                }
                return false;
            }
            catch { throw; }
            finally {
                //if (Db.IsDebug) Db.ShowSqlString(sql, _Params);
            }
        }
        public bool Insert(T bean)
        {
            long id = 0;
            return Insert(bean, ref id);
        }

        public bool Update(long id, T bean)
        {
            if (bean == null || bean.Count == 0) { throw new Exception("bean 不能为 NULL"); }
            var _SetColumns = new List<string>();
            var _Params = new List<DbParameter>();
            foreach (var item in bean)
            {
                if (item.Key == "ID") continue;
                if (_Columns.Contains(item.Key))
                {
                    _SetColumns.Add(DB.GetCondition(item.Key));
                    _Params.Add(DB.GetParam(item.Key, item.Value));
                }
            }
            var sql = "UPDATE " + TableString + " SET " + string.Join(",", _SetColumns) + $" WHERE {DB.GetName("ID")}={id};";
            //if (Db.IsDebug) Db.ShowSqlString(sql, _Params);

            if (_SetColumns.Count == 0) { throw new Exception("SQL语法错误"); }
            try
            {
                if (Db.Write(sql, _Params))
                {
                    RowCache.Remove(id);
                    return true;
                }
                return false;
            }
            catch { throw; }
        }
        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="setString">SET字符串,"a=? and b=?"</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public bool Update(long id, string setString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(setString)) { throw new Exception("数据库操作命令不能为空"); }
            var sql = new StringBuilder(200);
            sql.Append("UPDATE " + TableString);
            if (!setString.Trim().StartsWith("SET ", StringComparison.CurrentCultureIgnoreCase)) sql.Append(" SET ");

            var Params = new List<DbParameter>();
            sql.Append(DB.GetRawSql(setString, ref Params, args));
            sql.Append($" WHERE {DB.GetName("ID")}={id};");
            try { return Db.Write(sql.ToString(), Params); }
            catch (Exception) { throw; }
            finally {
                //if (Db.IsDebug) Db.ShowSqlString(sql.ToString(), Params);
            }
        }
        public bool Update<V>(long id, string col, V val)
        {
            if (val == null || string.IsNullOrWhiteSpace(col)) { throw new Exception("必要的参数不能为 NULL"); }

            DbParameter _Param = null;
            string sql = null;
            if (_ColumnDictionary.Contains(col.ToLower()))
            {
                _Param = DB.GetParam(col, val);
                sql = "UPDATE " + TableString + " SET " + DB.GetCondition(col) + $" WHERE {DB.GetName("ID")}={id};";
                //if (Db.IsDebug) Db.ShowSqlString(sql, _Param);
            }
            else { throw new Exception(col + "列不存在"); }

            try
            {
                if (Db.Write(sql, _Param))
                {
                    RowCache.Remove(id);
                    return true;
                }
                return false;
            }
            catch { throw; }
        }
        public bool Update(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }

            try
            {
                var _setColumn = cond.GetSetColumn();
                var sql = "UPDATE " + TableString + " SET " + _setColumn + cond.GetWhereString() + ";";
                //if (Db.IsDebug) Db.ShowSqlString(sql, cond.Params);

                if (_setColumn == "")
                {
                    throw new Exception("SQL语法错误");
                }
                if (Db.Write(sql, cond.Params))
                {
                    Task.Run(() => { RowCache.Remove(GetCacheIds(cond)); });
                    return true;
                }
                return false;
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }

        public bool Delete(long id)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")}={id};";
            //if (Db.IsDebug) Db.ShowSqlString(sql);
            try
            {
                if (Db.Write(sql))
                {
                    Task.Run(() =>
                    {
                        RowCache.Remove(id);
                        ListCache.Clear();
                    });
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete(IEnumerable<long> ids)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")} in ({string.Join(",", ids)});";
            //if (Db.IsDebug) Db.ShowSqlString(sql);
            try
            {
                if (Db.Write(sql))
                {
                    Task.Run(() =>
                    {
                        RowCache.Remove(ids as long[]);
                        ListCache.Clear();
                    });
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
        }

        public bool Delete(Sql cond)
        {
            if (cond == null)
            {
                throw new Exception("cond 不能为 NULL");
            }

            try
            {
                var sql = "DELETE FROM " + TableString + cond.GetWhereString() + ";";
                //if (Db.IsDebug) Db.ShowSqlString(sql, cond.Params);
                if (Db.Write(sql))
                {
                    Task.Run(() =>
                    {
                        RowCache.Remove(GetCacheIds(cond));
                        ListCache.Clear();
                    });
                    return true;
                }
                return false;
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }

        public void RemoveCache(long id)
        {
            RowCache.Remove(id);
        }
        public void RemoveCache(long[] ids)
        {
            RowCache.Remove(ids);
        }
        public void RemoveListCache(T bean, string orderby = null, uint count = 1000)
        {
            ListCache.Remove(CombineCacheKey(bean, orderby, count));
        }
        public void RemoveListCache(Sql cond)
        {
            ListCache.Remove(CombineCacheKey(cond));
            if (!cond.Disposed) cond.Dispose();
        }
    }
}
