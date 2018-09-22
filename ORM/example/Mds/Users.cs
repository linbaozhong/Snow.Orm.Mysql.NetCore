using Snow.Orm;
using System;

namespace Api.Mds
{
    /// <summary>
    /// 用户
    /// </summary>
    public class Users : BaseEntity
    {
        public static Table<Users> Table = new Table<Users>(dbs.Db);
        /// <summary>
        /// ID
        /// </summary>
        public int ID { set { Set(value); } get { return Get<int>(); } }
        /// <summary>
        /// 手机号码
        /// </summary>
        public long Mobile { set { Set(value); } get { return Get<long>(); } }
        /// <summary>
        /// 登录次数
        /// </summary>
        public int LoginTimes { set { Set(value); } get { return Get<int>(); } }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set { Set(value); } get { return Get<DateTime>(); } }
    }
    /// <summary>
    /// 服务商
    /// </summary>
    [OrmTable("sellers")]
    public class Service : BaseEntity
    {
        public static Table<Service> Table = new Table<Service>(dbs.Db);
        /// <summary>
        /// ID
        /// </summary>
        public int ID { set { Set(value); } get { return Get<int>(); } }
        /// <summary>
        /// 服务商名称
        /// </summary>
        public long Name { set { Set(value); } get { return Get<long>(); } }
    }
}
