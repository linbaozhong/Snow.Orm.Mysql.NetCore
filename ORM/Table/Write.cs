using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        #region Insert
        /// <summary>
        /// Insert 一条新数据
        /// </summary>
        /// <param name="bean">表实体对象</param>
        /// <returns></returns>
        public DalResult Insert(T bean)
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
            var sql = "INSERT INTO " + TableString + " (" + string.Join(",", _Fields) + ") VALUES(" + string.Join(",", _Values) + ");";

            var result = Db.Insert(_Session, sql, _Params);
            if (result.Success)
            {
                _OnInsert.Invoke(result.Id);
            }
            return result;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update 一条数据
        /// </summary>
        /// <param name="bean">表实体对象(要更新的字段值),关键字段ID必须赋值,</param>
        /// <returns></returns>
        public DalResult Update(T bean)
        {
            if (bean == null) { throw new Ex("bean 不能为 NULL", Ex.Null); }
            if (bean.Count < 2) { throw new Ex("缺少更新字段", Ex.Null); }
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
            if (_SetColumns.Count == 0) { throw new Ex("缺少更新字段", Ex.Null); }

            var sql = "UPDATE " + TableString + " SET " + string.Join(",", _SetColumns) + $" WHERE {DB.GetName("ID")}={id};";
            var result = Db.Write(_Session, sql, _Params);
            if (result.Success)
            {
                _OnUpdate.Invoke(id);
            }
            return result;
        }
        /// <summary>
        /// Update 一条数据
        /// </summary>
        /// <param name="id">表关键字段ID值</param>
        /// <param name="bean">表实体对象(要更新的字段值)</param>
        /// <returns></returns>
        public DalResult Update(long id, T bean)
        {
            if (id == 0) { throw new Ex("id = 0 错误", Ex.BadParameter); }
            if (bean == null) { throw new Ex("bean 不能为 NULL", Ex.Null); }
            if (bean.Count < 1) { throw new Ex("缺少更新字段", Ex.Null); }
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
            if (_SetColumns.Count == 0) { throw new Ex("缺少更新字段", Ex.Null); }
            var sql = "UPDATE " + TableString + " SET " + string.Join(",", _SetColumns) + $" WHERE {DB.GetName("ID")}={id};";
            var result = Db.Write(_Session, sql, _Params);
            if (result.Success)
            {
                _OnUpdate.Invoke(id);
            }
            return result;
        }
        /// <summary>
        /// 原生UPDATE
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="setString">SET字符串,"a=?,b=?"</param>
        /// <param name="args">?对应的参数值</param>
        /// <returns></returns>
        public DalResult Update(long id, string setString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(setString)) { throw new Ex("数据库操作命令不能为空", Ex.BadParameter); }
            var sql = new StringBuilder(200);
            sql.Append("UPDATE " + TableString);
            if (!setString.Trim().StartsWith("SET ", StringComparison.CurrentCultureIgnoreCase)) sql.Append(" SET ");

            var cmd = DB.GetRawSql(setString, args);
            sql.Append(cmd.SqlString);
            sql.Append($" WHERE {DB.GetName("ID")}={id};");

            var result = Db.Write(_Session, sql.ToString(), cmd.SqlParams);
            if (result.Success)
            {
                _OnUpdate.Invoke(id);
            }
            return result;
        }
        /// <summary>
        /// 更新指定字段
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="id">ID值</param>
        /// <param name="col">字段名</param>
        /// <param name="val">字段值</param>
        /// <returns></returns>
        public DalResult Update<V>(long id, string col, V val)
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
            var result = Db.Write(_Session, sql, _Param);
            if (result.Success)
            {
                _OnUpdate.Invoke(id);
            }
            return result;
        }
        /// <summary>
        /// 根据指定的条件 Update数据
        /// </summary>
        /// <param name="cond">Sql 查询条件对象</param>
        /// <returns></returns>
        public DalResult Update(Sql cond)
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
                var result = Db.Write(_Session, sql, cond.Params);
                if (result.Success)
                {
                    OnUpdate(GetCacheIds(cond));
                }
                return result;
            }
            catch { throw; }
            finally { if (!cond.Disposed) cond.Dispose(); }
        }
        /// <summary>
        /// 指定列递增
        /// </summary>
        /// <param name="id">ID值</param>
        /// <param name="col">列名</param>
        /// <param name="val">递增值</param>
        /// <returns></returns>
        public DalResult Incr(long id, string col, int val = 1)
        {
            if (val == 0) return DalResult.Factory;
            return IncrDecr(id, col, val);
        }
        /// <summary>
        /// 指定列递减
        /// </summary>
        /// <param name="id">ID值</param>
        /// <param name="col">列名</param>
        /// <param name="val">递减值</param>
        /// <returns></returns>
        public DalResult Decr(long id, string col, int val = 1)
        {
            if (val == 0) return DalResult.Factory;
            return IncrDecr(id, col, val, "-");
        }

        DalResult IncrDecr(long id, string col, int val, string op = "+")
        {
            string sql = null;
            if (_ColumnDictionary.ContainsKey(col))
            {
                sql = "UPDATE " + TableString + " SET " + DB.GetName(col) + Op.Eq + DB.GetName(col) + $"{op}{val} WHERE {DB.GetName("ID")}={id};";
            }
            else { throw new Ex(col + "列不存在", Ex.NotFound); }
            var result = Db.Write(_Session, sql);
            if (result.Success)
            {
                _OnUpdate.Invoke(id);
            }
            return result;
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除指定行
        /// </summary>
        /// <param name="id">ID值</param>
        /// <returns></returns>
        public DalResult Delete(long id)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")}={id};";
            var result = Db.Write(_Session, sql);
            if (result.Success)
            {
                _OnDelete.Invoke(id);
            }
            return result;
        }
        /// <summary>
        /// 删除指定的若干行
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public DalResult Delete(IEnumerable<long> ids)
        {
            var sql = "DELETE FROM " + TableString + $" WHERE {DB.GetName("ID")} in ({string.Join(",", ids)});";
            var result = Db.Write(_Session, sql);
            if (result.Success)
            {
                OnDelete(ids as long[]);
            }
            return result;
        }
        /// <summary>
        /// 按指定的条件删除行
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public DalResult Delete(Sql cond)
        {
            if (cond == null) { throw new Exception("cond 不能为 NULL"); }
            try
            {
                var sql = "DELETE FROM " + TableString + cond.GetWhereString() + ";";
                var result = Db.Write(_Session, sql);
                if (result.Success)
                {
                    OnUpdate(GetCacheIds(cond));
                }
                return result;
            }
            catch { throw; }
            finally { if (cond != null && !cond.Disposed) cond.Dispose(); }
        }
        #endregion
    }
}
