using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
