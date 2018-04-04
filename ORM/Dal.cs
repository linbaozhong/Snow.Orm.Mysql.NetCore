using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public sealed partial class Dal
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public bool Insert(BaseEntity bean)
        {
            if (bean == null || bean.Count == 0)
            {
                Error("bean 不能为 NULL");
                return false;
            }
            HashSet<string> _Values = new HashSet<string>();
            _Columns.Clear();
            _Params.Clear();
            string key;
            var _cols = DB.Tables[bean.Index].Columns;
            foreach (var item in bean)
            {
                key = _cols.TryGet(item.Key, item.Key);
                _Columns.Add(key);
                _Values.Add(DB._ParameterPrefix + key);
                _Params.Add(dbs.Db.GetParam(key, item.Value.ToString()));
            }
            var sql = "INSERT INTO " + GetTableName(bean) + " (" + GetColumns(bean) + ") VALUES(" + string.Join(",", _Values) + "); select ROW_COUNT(),LAST_INSERT_ID();";
            var id = -1L;
            try
            {
                if (dbs.Db.Insert(sql, _Params, ref id))
                {
                    bean["ID"] = id;
                    return true;
                }
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
            }
            finally
            {
                Finish(sql, _Params);
            }
            return false;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public bool Update(BaseEntity bean)
        {
            if (bean == null)
            {
                Error("bean 不能为 NULL");
                return false;
            }
            var sql = "UPDATE " + GetTableName(bean) + " SET " + GetSetColumn(bean) + GetSelectCondition(bean) + ";";
            bool ok = false;
            try
            {
                ok = dbs.Db.Write(sql, _Params);
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
            }
            finally
            {
                Finish(sql, _Params);
            }
            return ok;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        public bool Delete(BaseEntity bean)
        {
            if (bean == null)
            {
                Error("bean 不能为 NULL");
                return false;
            }
            var sql = "DELETE " + GetFrom(bean) + GetSelectCondition(bean) + ";";
            bool ok = false;
            try
            {
                ok = dbs.Db.Write(sql, _Params);
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
            }
            finally
            {
                Finish(sql, _Params);
            }
            return ok;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="beans"></param>
        /// <returns></returns>
        //public bool Find_<T>(List<T> beans) where T : class, new()
        //{
        //    if (beans == null)
        //    {
        //        Error("beans 不能为 NULL");
        //        return false;
        //    }
        //    var bean = new T() as BaseEntity;
        //    var sql = GetSelectSql(bean) + " limit 20;";
        //    DataTable tb = null;
        //    try
        //    {
        //        tb = dbs.Db.Read(sql, _Params);
        //        if (tb != null)
        //        {
        //            int i;
        //            foreach (DataRow row in tb.Rows)
        //            {
        //                i = 0;
        //                bean = new T() as BaseEntity;
        //                foreach (var item in _Columns)
        //                {
        //                    bean[item] = row[i++];
        //                }
        //                beans.Add(bean as T);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Error(sql, _Params, e);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (tb != null) tb.Dispose();
        //        Finish(sql, _Params, bean);
        //    }
        //    return true;
        //}

        public bool Find<T>(List<T> beans) where T : class, new()
        {
            if (beans == null)
            {
                Error("beans 不能为 NULL");
                return false;
            }
            var bean = new T() as BaseEntity;
            var sql = GetSelectSql(bean) + GetPage();

            DbDataReader dr = null;
            try
            {
                dr = dbs.Db.Read(sql, _Params);
                if (dr.HasRows)
                {
                    int i;
                    var _propertys = DB.Tables[bean.Index].Propertys;
                    while (dr.Read())
                    {
                        i = 0;
                        bean = new T() as BaseEntity;
                        foreach (var item in _Columns)
                        {
                            bean[_propertys.TryGet(item, item)] = dr[i++];
                        }
                        beans.Add(bean as T);
                    }
                }
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
                return false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                Finish(sql, _Params, bean);
            }
            return true;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        //public bool Get(BaseEntity bean)
        //{
        //    if (bean == null)
        //    {
        //        Error("bean 不能为 NULL");
        //        return false;
        //    }

        //    var sql = GetSelectSql(bean) + " limit 1;";
        //    DataTable tb = null;
        //    try
        //    {
        //        tb = dbs.Db.Read(sql, _Params);
        //        if (tb != null)
        //        {
        //            var i = 0;
        //            foreach (var item in _Columns)
        //            {
        //                bean[item] = tb.Rows[0][i++];
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Error(sql, _Params, e);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (tb != null) tb.Dispose();
        //        Finish(sql, _Params, bean);
        //    }
        //    return true;
        //}
        public bool Get(BaseEntity bean)
        {
            if (bean == null)
            {
                Error("bean 不能为 NULL");
                return false;
            }

            var sql = GetSelectSql(bean) + " limit 1;";
            DbDataReader dr = null;
            try
            {
                dr = dbs.Db.Read(sql, _Params);
                if (dr.HasRows && dr.Read())
                {
                    var i = 0;
                    var _propertys = DB.Tables[bean.Index].Propertys;
                    foreach (var item in _Columns)
                    {
                        bean[_propertys.TryGet(item, item)] = dr[i++];
                    }
                }
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
                return false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                Finish(sql, _Params, bean);
            }
            return true;
        }
        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        //public int? Count(BaseEntity bean)
        //{
        //    if (bean == null)
        //    {
        //        Error("bean 不能为 NULL");
        //        return null;
        //    }

        //    var sql = "SELECT COUNT(1) " + GetFrom(bean) + GetSelectCondition(bean) + GetOrderBy();
        //    DataTable tb = null;
        //    try
        //    {
        //        tb = dbs.Db.Read(sql, _Params);
        //        if (tb != null)
        //        {
        //            return tb.Rows[0][0].ToInt();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Error(sql, _Params, e);
        //        return null;
        //    }
        //    finally
        //    {
        //        if (tb != null) tb.Dispose();
        //        Finish(sql, _Params, bean);
        //    }
        //    return 0;
        //}

        public int? Count(BaseEntity bean)
        {
            if (bean == null)
            {
                Error("bean 不能为 NULL");
                return null;
            }

            var sql = "SELECT COUNT(1) " + GetFrom(bean) + GetSelectCondition(bean) + GetOrderBy();
            DbDataReader dr = null;
            try
            {
                dr = dbs.Db.Read(sql, _Params);
                if (dr.HasRows && dr.Read())
                {
                    return dr[0].ToInt();
                }
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
                return null;
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                Finish(sql, _Params, bean);
            }
            return 0;
        }

        /// <summary>
        /// 原生查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable Query(string sql, params object[] args)
        {
            _Params.Clear();
            _Condition.Clear();
            //
            GetParameter(sql, args);

            sql = _Condition.ToString();
            DataTable dt = new DataTable();
            DbDataReader dr = null;
            try
            {
                dr = dbs.Db.Read(sql, _Params);
                if (dr.HasRows)
                {
                    var len = dr.FieldCount;
                    DataRow newrow;
                    if (dr.Read())
                    {
                        newrow = dt.NewRow();
                        for (int i = 0; i < len; i++)
                        {
                            dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                            newrow[i] = dr[i];
                        }
                        dt.Rows.Add(newrow);
                    }
                    while (dr.Read())
                    {
                        newrow = dt.NewRow();
                        for (int i = 0; i < len; i++)
                        {
                            newrow[i] = dr[i];
                        }
                        dt.Rows.Add(newrow);
                    }
                    dt.AcceptChanges();
                }
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
                return null;
            }
            finally
            {
                if (dr != null && !dr.IsClosed) dr.Close();
                Finish(sql, _Params);
            }

            return dt;
        }
        /// <summary>
        /// 原生命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns>受影响的行数</returns>
        public bool Exec(string sql, params object[] args)
        {
            _Params.Clear();
            _Condition.Clear();
            //
            GetParameter(sql, args);

            sql = _Condition.ToString();
            bool ok = false;
            try
            {
                ok = dbs.Db.Write(sql, _Params);
            }
            catch (Exception e)
            {
                Error(sql, _Params, e);
            }
            finally
            {
                Finish(sql, _Params);
            }
            return ok;
        }
    }
}
