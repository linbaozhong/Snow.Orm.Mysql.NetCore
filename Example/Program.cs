using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);
            // Snow.Orm.dbs.Db.Exec("update user set id=?,name=?,sex=?",12,"linbaozhong",1);      
            var sql = Snow.Orm.Sql.Factory.Where("select * from user where name=? or age!=?","linbaozhong",12);
            sql.ShowSQLString();
            sql.Dispose();
            Console.ReadKey();

        }
    }
}
