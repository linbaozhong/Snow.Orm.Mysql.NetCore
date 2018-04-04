using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        HashSet<string> _SetColumns = new HashSet<string>();
        //public Dal Set<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Set(col.Name, col.Value);
        //}
        public Dal Set<T>(string col, T val)
        {
            _SetColumns.Add(dbs.Db.GetCondition(col));
            _Params.Add(dbs.Db.GetParam(col, val));
            return this;
        }
    }
}
