﻿using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // 新增一个用户
            var u = new Mds.Users();
            u.Mobile = 13901012345;
            u.CreateTime = DateTime.Now;
            var result = Mds.Users.Table.Insert(u);
            if (result.Success)
            {
                Console.WriteLine($"新用户ID:{result.Id}");
            }
            result = Mds.Users.Table.Update(Snow.Orm.Sql.Factory
                .Gte(nameof(Mds.Users.LoginTimes),10)
                .Set(nameof(Mds.Users.CreateTime),DateTime.Now)
                );
            // 更新用户id=123的部分字段
            result = Mds.Users.Table.Update(123, $"{nameof(Mds.Users.LoginTimes)}=?,{nameof(Mds.Users.CreateTime)}=?", 10, DateTime.Now);
            if (!result.Success)
            {
                Console.WriteLine($"更新失败");
            }
            for (int i = 0; i < 120; i++)
            {
                var _sql = Snow.Orm.Sql.Factory.Eq(nameof(Mds.Users.Mobile), 13051015768);
                var md = Mds.Users.Table.Gets(_sql);
            }
            
            Console.ReadKey();
        }
    }
}
