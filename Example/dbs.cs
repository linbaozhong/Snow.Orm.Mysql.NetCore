using System.IO;

namespace Snow.Orm
{
    /// <summary>
    /// 库集合
    /// </summary>
    public static class dbs
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
            Db = new Snow.Orm.DB("Data Source=localhost;database=snow_shop;user id=root;password=123456;Pooling=true;Max Pool Size=100;Min Pool Size=1;default command timeout=20;characterset=utf8", 0, Log);
        }

    }
}
