using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public partial class Sql
    {
        /// <summary>
        /// 包含列
        /// </summary>
        protected List<string> _Columns = new List<string>();
        /// <summary>
        /// 排除列
        /// </summary>
        protected List<string> _OmitColumns = new List<string>();

        public List<string> Columns { get { return _Columns; } }
        /// <summary>
        /// 包含列
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public Sql Cols(params string[] cols)
        {
            if (cols == null || _OmitColumns.Count > 0)
            {
                return this;
            }
            for (int i = 0; i < cols.Length; i++)
            {
                _Columns.Add(cols[i]);
            }
            return this;
        }
        /// <summary>
        /// 排除列
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public Sql Omit(params string[] cols)
        {
            if (cols == null || _Columns.Count > 0)
            {
                return this;
            }
            for (int i = 0; i < cols.Length; i++)
            {
                _OmitColumns.Add(cols[i]);
            }
            return this;
        }

    }
}
