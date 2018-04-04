using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        HashSet<string> _Columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> _OmitColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 选中列
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public Dal Cols(params string[] cols)
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
        public Dal Omit(params string[] cols)
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
