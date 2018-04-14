using System;
using System.Collections.Generic;
using System.Text;
using Snow.Orm;

namespace Example.Mds
{
    public class Users : BaseEntity
    {
        public static Table<Users> Table = new Table<Users>(dbs.Db, TableTypes.Default);
        public Users() : base(Table.Name) { ID = 0; }

        public int ID { set { Set(value); } get { return Get<int>(); } }
        public long Mobile { set { Set(value); } get { return Get<long>(); } }
        public DateTime CreateTime { set { Set(value); } get { return Get<DateTime>(); } }
    }
}
