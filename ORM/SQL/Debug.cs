using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    public partial class Sql
    {
        protected bool ShowSQL = false;

        public void ShowSQLString()
        {
            ShowSQLString(OtherCondition.ToString(), Params);
        }
        /// <summary>
        /// 控制台打印SQL命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        void ShowSQLString(string sql, List<DbParameter> param)
        {
            Console.WriteLine($"------------{DateTime.Now}--------------");
            Console.WriteLine(DB.Debug(sql, param));
        }
        void ShowSQLString(string sql, DbParameter param)
        {
            Console.WriteLine($"------------{DateTime.Now}--------------");
            Console.WriteLine(DB.Debug(sql, param));
        }

    }
}
