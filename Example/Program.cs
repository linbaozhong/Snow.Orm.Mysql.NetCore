using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var sql = Snow.Orm.Sql.Factory)
            //{
            //    sql.Where("select * from user where name=? or age!=?", "linbaozhong", 12);
            //    sql.ShowSQLString();
            //}
            for (int i = 0; i < 120; i++)
            {
                var _sql = Snow.Orm.Sql.Factory.Eq(nameof(Mds.Users.Mobile), 13051015768);
                Console.WriteLine(_sql.GetHashCode());
                var md = Mds.Users.Table.Gets(_sql);
            }
            Console.ReadKey();
        }
    }
}
