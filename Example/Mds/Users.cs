using System;
using System.Collections.Generic;
using System.Text;
using Snow.Orm;

namespace Example.Mds
{
    public class Users : BaseEntity
    {
        public static Table<Users> Table = new Table<Users>(dbs.Db, TableTypes.Default);
        public Users() : base(Table.Name) {}
    }
}
