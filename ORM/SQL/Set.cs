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
        /// 更新列
        /// </summary>
        List<string> _SetColumns = new List<string>();
        /// <summary>
        /// 将列更新为指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Sql Set<T>(string col, T val)
        {
            _SetColumns.Add(DB.GetCondition(col));
            Params.Add(DB.GetParam(col, val));
            return this;
        }
        /// <summary>
        /// 列值递增
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">递增值(缺省为1)</param>
        /// <returns></returns>
        public Sql Incr(string col, int val = 1)
        {
            _SetColumns.Add(string.Concat(DB.GetName(col), "=", DB.GetName(col), "+", val));
            return this;
        }
        /// <summary>
        /// 列值递减
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">递减值(缺省为1)</param>
        /// <returns></returns>
        public Sql Decr(string col, int val = 1)
        {
            _SetColumns.Add(string.Concat(DB.GetName(col), "=", DB.GetName(col), "-", val));
            return this;
        }
        /// <summary>
        /// 将列更新为计算值
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="formula">计算公式</param>
        /// <returns></returns>
        public Sql SetFunc(string col, string formula)
        {
            _SetColumns.Add(DB.SetColumnFunc(col, formula));
            return this;
        }
        public string GetSetColumn()
        {
            if (_SetColumns.Count > 0)
            {
                return string.Join(",", _SetColumns);
            }
            return "";
        }

    }
}
