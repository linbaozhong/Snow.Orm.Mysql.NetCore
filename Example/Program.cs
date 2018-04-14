using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            //using (var sql = Snow.Orm.Sql.Factory)
            //{
            //    sql.Where("select * from user where name=? or age!=?", "linbaozhong", 12);
            //    sql.ShowSQLString();
            //}
            var md = Mds.Users.Table.Gets("select * from users where mobile=? or pwd=?",13051015768, "linbaozhong");
            Console.ReadKey();
        }
    }
}
