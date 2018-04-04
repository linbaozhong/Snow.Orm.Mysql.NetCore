using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Snow.Orm
{
    public class Dal : Sql, IDisposable
    {
        #region 前置准备
        DB Db;
        string TableName;
        #endregion

        static ObjectPool<Dal> pool = new ObjectPool<Dal>((d) =>
        {
            return new Dal(d);
        }, x => { x.Reset(); }, 100);

        protected override void Reset()
        {
            base.Reset();
        }
        public new static Dal Factory(DB db)
        {
            return pool.GetObject(db); 
        }

        public Dal(DB db)
        {
            ShowSQL = db.IsDebug;
            Db = db;
        }

        public Dal Table(string name)
        {
            TableName = name;
            return this;
        }
        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetTableName(BaseEntity bean)
        {
            return DB.GetName(string.IsNullOrWhiteSpace(TableName) ? bean.TableName : TableName);
        }

        #region 数据访问
        /// <summary>
        /// Insert一条数据
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        bool Insert(BaseEntity bean)
        {
            if (bean == null || bean.Count == 0) { throw new Exception("bean 不能为 NULL"); }

            List<string> _Values = new List<string>();
            _Columns.Clear();
            Params.Clear();
            foreach (var item in bean)
            {
                _Columns.Add(DB.GetName(item.Key));
                _Values.Add(DB._ParameterPrefix + item.Key);
                Params.Add(DB.GetParam(item.Key, item.Value.ToString()));
            }
            var sql = "INSERT INTO " + GetTableName(bean) + " (" + string.Join(",", _Columns) + ") VALUES (" + string.Join(",", _Values) + "); select ROW_COUNT(),LAST_INSERT_ID();";
            var id = -1L;
            try
            {
                if (Db.Insert(sql, Params, ref id))
                {
                    if (id > 0) bean["ID"] = id;
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ShowSQL) ShowSQLString(sql, Params);
            }
        }

        bool Update(params Sql[] args)
        {
            if (args.Length == 0) { throw new Exception("args 不能为 NULL"); }

            return true;
        }
 
        #endregion
    }
}
