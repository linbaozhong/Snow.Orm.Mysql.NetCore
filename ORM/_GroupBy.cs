using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        string _GroupBy;
        string _Having;

        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public Dal GroupBy(string groupby)
        {
            _GroupBy = groupby;
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public Dal Having(string having)
        {
            _Having = having;
            return this;
        }
    }
}
