using System.Collections.Generic;

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
        /// <summary>
        /// 随机排序
        /// </summary>
        /// <returns></returns>
        public Sql OrderByRandom()
        {
            _OrderBy.Add("rand()");
            return this;
        }
        /// <summary>
        /// 按指定的运算字符串排序
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Sql OrderByString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return this;
            _OrderBy.Add(s);
            return this;
        }
    }
}
