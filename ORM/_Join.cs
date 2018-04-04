using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        List<string> _Join = new List<string>();
        /// <summary>
        /// 内联结
        /// </summary>
        /// <param name="table"></param>
        /// <param name="on"></param>
        /// <returns></returns>
        public Dal Inner(string table, string on)
        {
            _Join.Add(string.Format(" inner join {0} on {1}", dbs.Db.GetName(table), on));
            return this;
        }
        public Dal Left(string table, string on)
        {
            _Join.Add(string.Format(" left join {0} on {1}", dbs.Db.GetName(table), on));
            return this;
        }
        public Dal Right(string table, string on)
        {
            _Join.Add(string.Format(" right join {0} on {1}", dbs.Db.GetName(table), on));
            return this;
        }
    }
}
