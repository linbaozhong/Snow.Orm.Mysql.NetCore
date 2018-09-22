# Snow.Orm.Mysql.NetCore 使用指南
## 配置
* 日志 -- log4net.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <log4net>
    <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
      <file value="logfile/" />
      <appendToFile value="true" />
      <rollingStyle value="Composite" />
      <staticLogFileName value="false" />
      <datePattern value="yyyyMMdd'.log'" />
      <maxSizeRollBackups value="10" />
      <maximumFileSize value="1MB" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date | %message" />
      </layout>
    </appender>
    <!-- Setup the root category, add the appenders and set the default level %newline-->
    <root>
      <level value="ALL" />
      <appender-ref ref="RollingLogFileAppender" />
    </root>
  </log4net>
</configuration>
```
* 数据库连接 -- dbs.cs
```c#
using System.IO;
namespace Api
{
    public class dbs
    {
        public static Snow.Orm.DB Db;
        static log4net.ILog Log;
        static dbs()
        {
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.CreateRepository("NETCoreRepository");
            var fileInfo = new FileInfo("config/log4net.config");
            log4net.Config.XmlConfigurator.Configure(repository, fileInfo);
            log4net.Config.BasicConfigurator.Configure(repository);
            Log = log4net.LogManager.GetLogger(repository.Name, "NETCorelog4net");
            Db = new Snow.Orm.DB("Server=localhost;Database=snow_shop;Uid=root;Pwd=123456;Max Pool Size=50;", 0, Log, true);
        }
    }
}
```

## 数据实体类
* 数据实体类都必须继承自BaseEntity抽象类.BaseEntity抽象类实际上是一个字典类Dictionary,所以,基于此派生出的类,都具有Dictionary的全部成员.
* 数据实体类都必须实现静态的Table<T>类型的字段.
```c#
using Snow.Orm;
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
    [OrmTable("sellers")]   // 如果类名和数据表名不相同,用类属性来说明数据表名或视图名
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
        public string Name { set { Set(value); } get { return Get<string>(); } }
    }
}
```

## 数据库操作方法
### Insert
废话少说,直接上示例.
```c#
    // 新增一个用户
    var u = new Mds.Users();  // 实例化
    u.Mobile = 13901012345;
    u.CreateTime = DateTime.Now;
    // 所有的写操作都会返回一个DalResult对象{Success=true或false,Id=自增ID,Rows=受影响的行数}
    var result = Mds.Users.Table.Insert(u);
    if (result.Success)
    {
        Console.WriteLine($"新用户ID:{result.Id}");
    }
```
### Update
Update操作包含4个方法:
1. 使用实例化对象更新多个字段.注意,这个方法必须要为对象的主键ID进行赋值
```c#
    // 修改一个用户的手机号码
    var u = new Mds.Users();  // 实例化
    u.ID = 123;
    u.Mobile = 13901012345;
    // 所有的写操作都会返回一个DalResult对象{Success=true或false,Id=自增ID,Rows=受影响的行数}
    var result = Mds.Users.Table.Update(u);
    if (result.Success)
    {
        Console.WriteLine($"用户ID{u.ID}的新手机号码:{u.Mobile}");
    }
```
2. 使用主键ID值和实例化对象更新多个字段.
```c#
    // 修改一个用户的手机号码
    var u = new Mds.Users();  // 实例化
    u.Mobile = 13901012345;
    // 所有的写操作都会返回一个DalResult对象{Success=true或false,Id=自增ID,Rows=受影响的行数}
    var result = Mds.Users.Table.Update(123,u);
    if (result.Success)
    {
        Console.WriteLine($"用户的新手机号码:{u.Mobile}");
    }
```
3. 使用主键ID值更新1个字段.
```c#
    // 修改用户ID为123的手机号码
    // 所有的写操作都会返回一个DalResult对象{Success=true或false,Id=自增ID,Rows=受影响的行数}
    var result = Mds.Users.Table.Update(123,nameof(Mds.Users.Mobile),13001012345);
    if (!result.Success)
    {
        Console.WriteLine($"用户的手机号码更新失败");
    }
```
4. 指定字段递增/递减.
```c#
    // 用户ID为123的登录次数递增1次
    var result = Mds.Users.Table.Incr(123,nameof(Mds.Users.LoginTimes));
    if (!result.Success)
    {
        Console.WriteLine($"失败");
    }
    // 用户ID为123的登录次数递减2次
    var result = Mds.Users.Table.Decr(123,nameof(Mds.Users.LoginTimes),2);
    if (!result.Success)
    {
        Console.WriteLine($"失败");
    }
```
5. 复杂条件的字段更新.
```c#
    // 将全部登录次数大于等于10的用户的CreateTime修改为当前时间
    var result = Mds.Users.Table.Update(Snow.Orm.Sql.Factory
                .Gte(nameof(Mds.Users.LoginTimes),10)
                .Set(nameof(Mds.Users.CreateTime),DateTime.Now)
                );
    if (!result.Success)
    {
        Console.WriteLine($"更新失败");
    }
```
6. 使用原生SQL字符串更新字段.
```c#
    // 更新用户id=123的部分字段
    var result = Mds.Users.Table.Update(123,$"{nameof(Mds.Users.LoginTimes)}=?,{nameof(Mds.Users.CreateTime)}=?",10,DateTime.Now);
    if (!result.Success)
    {
        Console.WriteLine($"更新失败");
    }
```
