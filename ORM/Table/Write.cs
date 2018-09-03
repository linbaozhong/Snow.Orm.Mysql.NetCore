﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using adeway;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        #region Insert
        public bool Insert(T bean, ref long id)
        {
            if (bean == null || bean.Count == 0) { throw new Exception("bean 不能为 NULL"); }
            List<string> _Fields = new List<string>();
            List<string> _Values = new List<string>();
            var _Params = new List<DbParameter>();
            foreach (var item in bean)
            {
                if (_TColumns.Contains(item.Key))
                {
                    _Fields.Add(DB.GetName(item.Key));
                    _Values.Add(DB._ParameterPrefix + item.Key);
                    _Params.Add(DB.GetParam(item.Key, item.Value));
                }
            }
            var sql = "INSERT INTO " + TableString + " (" + string.Join(",", _Fields) + ") VALUES(" + string.Join(",", _Values) + "); select ROW_COUNT(),LAST_INSERT_ID();";
            if (Db.Insert(sql, _Params, ref id))
            {
                OnInsert();
                return true;
            }
            return false;
        }
        public bool Insert(T bean)
        {
            long id = 0;
            return Insert(bean, ref id);
        }
        #endregion

        #region Update
        public bool Update(T bean)
        {
            if (bean == null || bean.Count == 0) { throw new Ex("bean 不能为 NULL", Ex.Null); }
            var _SetColumns = new List<string>();
            var _Params = new List<DbParameter>();
            var id = 0L;
            if (bean.ContainsKey("id")) { id = bean["id"].ToLong(); }
            if (id == 0) { throw new Ex("id = 0 错误", Ex.BadParameter); }
            //var _old = GetCache(id);
            //if (_old == null) { throw new Ex("目标数据不存在", Ex.NotFound); }

            foreach (var item in bean)
            {
                if (item.Key.Equals("ID", StringComparison.OrdinalIgnoreCase)) continue;
                if (_TColumns.Contains(item.Key))
                {
                    //// 数据没有变化
                    //if (_old[item.Key] == item.Value) continue;

                    _SetColumns.Add(DB.GetCondition(item.Key));
                    _Params.Add(DB.GetParam(item.Key, item.Value));
                }
            }

            var sql = "UPDATE " + TableString + " SET " + string.Join(",", _SetColumns) + $" WHERE {DB.GetName("ID")}={id};";

            if (_SetColumns.Count == 0) { throw new Ex("SQL语法错误", Ex.Syntax); }

            if (Db.Write(sql, _Params))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }

        public bool Update(long id, T bean)
        {
            if (id == 0) { throw new Ex("id = 0 错误", Ex.BadParameter); }
            if (bean == null || bean.Count == 0) { throw new Ex("bean 不能为 NULL", Ex.Null); }
            //var _old = GetCache(id);
            //if (_old == null) { throw new Ex("目标数据不存在", Ex.NotFound); }

            var _SetColumns = new List<string>();
            var _Params = new List<DbParameter>();
            foreach (var item in bean)
            {
                if (item.Key.Equals("ID", StringComparison.OrdinalIgnoreCase)) continue;
                if (_TColumns.Contains(item.Key))
                {
                    //// 数据没有变化
                    //if (_old[item.Key] == item.Value) continue;

                    _SetColumns.Add(DB.GetCondition(item.Key));
                    _Params.Add(DB.GetParam(item.Key, item.Value));
                }
            }
            var sql = "UPDATE " + TableString + " SET " + string.Join(",", _SetColumns) + $" WHERE {DB.GetName("ID")}={id};";
            //if (Db.IsDebug) Db.ShowSqlString(sql, _Params);

            if (_SetColumns.Count == 0) { throw new Ex("SQL语法错误", Ex.Syntax); }

            if (Db.Write(sql, _Params))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 原生UPDATE
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="setString">SET字符串,"a=? and b=?"</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public bool Update(long id, string setString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(setString)) { throw new Ex("数据库操作命令不能为空", Ex.BadParameter); }
            var sql = new StringBuilder(200);
            sql.Append("UPDATE " + TableString);
            if (!setString.Trim().StartsWith("SET ", StringComparison.CurrentCultureIgnoreCase)) sql.Append(" SET ");

            var cmd = DB.GetRawSql(setString, args);
            sql.Append(cmd.SqlString);
            sql.Append($" WHERE {DB.GetName("ID")}={id};");

            if (Db.Write(sql.ToString(), cmd.SqlParams))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }
        public bool Update<V>(long id, string col, V val)
        {
            if (val == null || string.IsNullOrWhiteSpace(col)) { throw new Ex("参数不能为 NULL", Ex.BadParameter); }

            DbParameter _Param = null;
            string sql = null;
            if (_ColumnDictionary.ContainsKey(col))
            {
                _Param = DB.GetParam(col, val);
                sql = "UPDATE " + TableString + " SET " + DB.GetCondition(col) + $" WHERE {DB.GetName("ID")}={id};";
            }
            else { throw new Ex(col + "列不存在", Ex.NotFound); }

            if (Db.Write(sql, _Param))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }
        public bool Update(Sql cond)
        {
            if (cond == null) { throw new Ex("cond 不能为 NULL", Ex.Null); }

            try
            {
                var _setColumn = cond.GetSetColumn();
                var sql = "UPDATE " + TableString + " SET " + _setColumn + cond.GetWhereString() + ";";
                if (_setColumn == "")
                {
                    throw new Ex("SQL语法错误", Ex.Syntax);
                }
                if (Db.Write(sql, cond.Params))
                {
                    OnUpdate(GetCacheIds(cond));
                    return true;
                }
                return false;
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 指定列递增
        /// </summary>
        /// <param name="id"></param>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Incr(long id, string col, int val = 1)
        {
            if (val == 0) return true;
            return IncrDecr(id, col, val);
        }
        /// <summary>
        /// 指定列递减
        /// </summary>
        /// <param name="id"></param>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Decr(long id, string col, int val = 1)
        {
            if (val == 0) return true;
            return IncrDecr(id, col, val, "-");
        }
        bool IncrDecr(long id, string col, int val, string op = "+")
        {
            string sql = null;
            if (_ColumnDictionary.ContainsKey(col))
            {
                sql = "UPDATE " + TableString + " SET " + DB.GetName(col) + Op.Eq + DB.GetName(col) + $"{op}{val} WHERE {DB.GetName("ID")}={id};";
            }
            else { throw new Ex(col + "列不存在", Ex.NotFound); }

            if (Db.Write(sql))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }
        #endregion

        #region Delete
        public bool Delete(long id)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")}={id};";
            if (Db.Write(sql))
            {
                OnUpdate(id);
                return true;
            }
            return false;
        }
        public bool Delete(IEnumerable<long> ids)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")} in ({string.Join(",", ids)});";
            if (Db.Write(sql))
            {
                OnUpdate(ids as long[]);
                return true;
            }
            return false;
        }
        public bool Delete(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var sql = "DELETE FROM " + TableString + cond.GetWhereString() + ";";
                if (Db.Write(sql))
                {
                    OnUpdate(GetCacheIds(cond));
                    return true;
                }
                return false;
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        #endregion
    }
}
