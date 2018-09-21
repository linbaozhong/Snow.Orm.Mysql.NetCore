using System.Collections.Generic;

namespace Snow.Orm
{
    public partial class Sql
    {
        /// <summary>
        /// 包含列
        /// </summary>
        public List<string> Columns = new List<string>();
        /// <summary>
        /// 排除列
        /// </summary>
        public List<string> OmitColumns = new List<string>();

        /// <summary>
        /// 包含列
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public Sql Cols(params string[] cols)
        {
            if (cols == null || OmitColumns.Count > 0)
            {
                return this;
            }
            for (int i = 0; i < cols.Length; i++)
            {
                Columns.Add(cols[i]);
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
            if (cols == null || Columns.Count > 0)
            {
                return this;
            }
            for (int i = 0; i < cols.Length; i++)
            {
                OmitColumns.Add(cols[i]);
            }
            return this;
        }

    }
}
