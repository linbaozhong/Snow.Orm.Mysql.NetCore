using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Sql
    {
        List<string> _Join = new List<string>();
        /// <summary>
        /// 内联结
        /// </summary>
        /// <param name="table"></param>
        /// <param name="on"></param>
        /// <returns></returns>
        public Sql Inner(string table, string on)
        {
            _Join.Add(string.Format(" inner join {0} on {1}", DB.GetName(table), on));
            return this;
        }
        public Sql Left(string table, string on)
        {
            _Join.Add(string.Format(" left join {0} on {1}", DB.GetName(table), on));
            return this;
        }
        public Sql Right(string table, string on)
        {
            _Join.Add(string.Format(" right join {0} on {1}", DB.GetName(table), on));
            return this;
        }
    }
}
