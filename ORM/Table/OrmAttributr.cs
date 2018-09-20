using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class OrmTableAttribute : Attribute
    {
        public OrmTableAttribute(string name)
        {
            Name = name;
        }
        ///// <summary>
        ///// 表名
        ///// </summary>
        public string Name { set; get; }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class OrmColumnAttribute : Attribute
    {
        /// <summary>
        /// json列名
        /// </summary>
        /// <param name="name"></param>
        public OrmColumnAttribute(string name)
        {
            JsonName = name;
        }

        ///// <summary>
        ///// 是主键
        ///// </summary>
        //public bool IsKey { set; get; }

        /// <summary>
        /// 列名
        /// </summary>
        public string JsonName { set; get; }

    }

}
