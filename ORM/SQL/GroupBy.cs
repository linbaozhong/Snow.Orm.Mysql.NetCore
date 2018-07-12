using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Sql
    {
        string _GroupBy = null;
        string _Having;

        public string GetGroupbyString()
        {
            if (string.IsNullOrWhiteSpace(_GroupBy)) return "";
            return string.Concat(" GROUP BY ", _GroupBy, string.IsNullOrWhiteSpace(_Having) ? "" : " HAVING " + _Having);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Sql GroupBy(params string[] args)
        {
            _GroupBy = string.Join(",",args);
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public Sql Having(string having)
        {
            _Having = having;
            return this;
        }
    }
}
