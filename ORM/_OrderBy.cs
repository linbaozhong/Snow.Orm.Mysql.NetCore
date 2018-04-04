using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        HashSet<string> _OrderBy = new HashSet<string>();

        public Dal OrderBy(string col)
        {
            return Asc(col);
        }
        //public Dal OrderBy<T>(Column<T> col)
        //{
        //    return Asc(col);
        //}
        public Dal Asc(string col)
        {
            if (string.IsNullOrWhiteSpace(col)) return this;
            _OrderBy.Add(col);
            return this;
        }
        //public Dal Asc<T>(Column<T> col)
        //{
        //    if (col == null || string.IsNullOrWhiteSpace(col.Name)) return this;
        //    return Asc(dbs.Db.GetColumn(col.Name));
        //}
        public Dal Desc(string col)
        {
            if (string.IsNullOrWhiteSpace(col)) return this;
            _OrderBy.Add(col + " DESC");
            return this;
        }

        //public Dal Desc<T>(Column<T> col)
        //{
        //    if (col == null || string.IsNullOrWhiteSpace(col.Name)) return this;
        //    return Desc(dbs.Db.GetColumn(col.Name));
        //}
    }
}
