using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    public partial class Sql
    {
        /// <summary>
        /// 排序列
        /// </summary>
        List<string> _OrderBy = new List<string>();
        public string GetOrderbyString()
        {
            if (_OrderBy.Count > 0)
            {
                return " ORDER BY " + string.Join(" , ", _OrderBy);
            }
            return string.Empty;
        }
        public Sql OrderBy(string col)
        {
            return Asc(col);
        }
        public Sql Asc(string col)
        {
            if (string.IsNullOrWhiteSpace(col)) return this;
            _OrderBy.Add(DB.GetName(col));
            return this;
        }
        public Sql Desc(string col)
        {
            if (string.IsNullOrWhiteSpace(col)) return this;
            _OrderBy.Add(DB.GetName(col) + " DESC");
            return this;
        }
    }
}
