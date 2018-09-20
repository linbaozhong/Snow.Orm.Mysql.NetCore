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
        /// 分页{ 页码,页容量 }
        /// </summary>
        uint[] _Page = new uint[2];
        internal string GetPageString()
        {
            if (_Page.Length > 1)
            {
                var _offset = _Page[0] * _Page[1];
                if (_offset == 0)
                {
                    if (_Page[1] > 0)
                        return $" LIMIT {_Page[1]};";
                }
                else if (_offset > 0)
                {
                    if (_Page[1] > 0)
                        return $" LIMIT {_offset}, {_Page[1]};";
                }
            }
            return " LIMIT 1000;";
        }
        /// <summary>
        /// 分页读取
        /// </summary>
        /// <param name="index">页码(缺省从0开始)</param>
        /// <param name="size">页容量(缺省每页20条记录)</param>
        /// <returns></returns>
        public Sql Page(uint index = 0, uint size = 20)
        {
            _Page[0] = index;
            _Page[1] = size;
            return this;
        }
        /// <summary>
        /// 读取前n条数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Sql Top(uint size)
        {
            return Page(0, size);
        }
    }
}
