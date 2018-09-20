这是基于MySQL数据库的.Net Core版本;
如果使用的是其他DBMS,仿照MysqlDB类新建一个类,例如:新建一个基于MSSQL的类MSSQLDB,将DB类继承自MSSQLDB;
如果使用.Net Framework框架,自行创建一个基于.Net Framework的类库,将该项目下的文件链接或复制到新类库中,可能需要根据框架特性进行微调;