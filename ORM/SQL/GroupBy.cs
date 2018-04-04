using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Sql
    {
        string _GroupBy;
        string _Having;

        public string GetGroupbyString()
        {
            if (string.IsNullOrWhiteSpace(_GroupBy)) return "";
            return string.Concat(" GROUP BY ", _GroupBy, string.IsNullOrWhiteSpace(_GroupBy) ? "" : " HAVING " + _Having);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public Sql GroupBy(string groupby)
        {
            _GroupBy = groupby;
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
