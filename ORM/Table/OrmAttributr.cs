using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class OrmTableAttribute : Attribute
    {
        public OrmTableAttribute(string JsonName)
        {
            Json = JsonName;
        }
        ///// <summary>
        ///// 表名
        ///// </summary>
        //public string Table { set; get; }
        /// <summary>
        /// Json名
        /// </summary>
        public string Json { set; get; }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class OrmColumnAttribute : Attribute
    {
        public OrmColumnAttribute(string JsonName)
        {
            Json = JsonName;
        }
        ///// <summary>
        ///// 表名
        ///// </summary>
        //public string Table { set; get; }
        ///// <summary>
        ///// 列名
        ///// </summary>
        //public string Column { set; get; }
        ///// <summary>
        ///// 是主键
        ///// </summary>
        //public bool IsKey { set; get; }
        /// <summary>
        /// Json名
        /// </summary>
        public string Json { set; get; }

    }

}
