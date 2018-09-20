using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Snow.Orm;

namespace Api.Mds
{
    public class Users : BaseEntity
    {
        public static Table<Users> Table = new Table<Users>(dbs.Db);
        [OrmColumn("id")]
        public int ID { set { Set(value); } get { return Get<int>(); } }
        [OrmColumn("mob")]
        public long Mobile { set { Set(value); } get { return Get<long>(); } }
        public DateTime CreateTime { set { Set(value); } get { return Get<DateTime>(); } }
    }
    [OrmTable("sellers")]
    public class Service:BaseEntity
    {
        public static Table<Service> Table = new Table<Service>(dbs.Db);
        [OrmColumn("id")]
        public int ID { set { Set(value); } get { return Get<int>(); } }
        [OrmColumn("name")]
        public long Name { set { Set(value); } get { return Get<long>(); } }
    }
}
