using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 120; i++)
            {
                var _sql = Snow.Orm.Sql.Factory.Eq(nameof(Mds.Users.Mobile), 13051015768);
                var md = Mds.Users.Table.Gets(_sql);
            }
            
            Console.ReadKey();
        }
    }
}
