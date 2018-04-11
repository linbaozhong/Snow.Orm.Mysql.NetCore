using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T> where T : BaseEntity, new()
    {
        log4net.ILog Log;

        DB Db;
        static string SelectColumnString;

        Cache<long, T> RowCache;
        Cache<string, long[]> ListCache;

        bool _ShowSQL = false;
        /// <summary>
        /// (属性名-列)字典
        /// </summary>
        public Dictionary<string, string> Columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// (列名-属性名)字典
        /// </summary>
        public Dictionary<string, string> Propertys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 主键字典
        /// </summary>
        public List<string> PrimaryKey = new List<string>();


        /// <summary>
        /// 表字段字典
        /// </summary>
        static List<string> _ColumnDictionary = new List<string>();
        /// <summary>
        /// 表字段
        /// </summary>
        protected static List<string> _Columns = new List<string>();
        /// <summary>
        /// T 属性
        /// </summary>
        protected static List<string> _Jsons = new List<string>();

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// JSON表名
        /// </summary>
        public string JsonName { private set; get; }

        public Table<T> ShowSQL
        {
            get
            {
                _ShowSQL = true;
                return this;
            }
        }
        /// <summary>
        /// 控制台打印SQL命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="Params"></param>
        void ShowSQLString(string sql, List<DbParameter> Params)
        {
            if (Log == null) return;
            Log.Debug($"------------{DateTime.Now}--------------");
            Log.Debug(sql);
            if (Params != null)
            {
                StringBuilder mess = new StringBuilder(300);
                string s = null;

                foreach (DbParameter p in Params)
                {
                    if (p == null || p.Value == null) continue;
                    s = p.Value.ToString();
                    mess.Append(p.ParameterName);
                    mess.Append(": ");
                    if (s != null && s.Length > 1000) mess.AppendLine(s.Substring(0, 1000));
                    else mess.AppendLine(s);
                }
                Log.Debug(mess.ToString());
            }
            _ShowSQL = false;
        }
        void ShowSQLString(string sql, DbParameter Param = null)
        {
            if (Log == null) return;
            Log.Debug($"------------{DateTime.Now}--------------");
            Log.Debug(sql);
            if (Param != null)
            {
                StringBuilder mess = new StringBuilder(300);
                string s = Param.Value.ToString();
                mess.Append(Param.ParameterName);
                mess.Append(": ");
                if (s != null && s.Length > 1000) mess.AppendLine(s.Substring(0, 1000));
                else mess.AppendLine(s);

                Log.Debug(mess.ToString());
            }
            _ShowSQL = false;
        }

        /// <summary>
        /// 固有公共属性(继承类必须排除)
        /// </summary>
        private static HashSet<string> OmitProperties = new HashSet<string>(
            new string[] { "Item", "Comparer", "Keys", "Values", "Count" }, StringComparer.OrdinalIgnoreCase);

        public Table(DB db, TableTypes type = TableTypes.Default, log4net.ILog log = null)
        {
            Log = log;
            Db = db;
            Type objType = typeof(T);
            Name = objType.Name;
            //取类的自定义特性
            object objAttr = objType.GetCustomAttribute(typeof(OrmTableAttribute), true);
            JsonName = objAttr == null ? Name : (objAttr as OrmTableAttribute).Json;

            string _propName, _jsonName;
            OrmColumnAttribute attr;
            var _properties = objType.GetProperties();
            //遍历类的全部公共属性
            foreach (PropertyInfo propInfo in _properties)
            {
                _propName = propInfo.Name;
                // 忽略固有公共属性
                if (OmitProperties.Contains(_propName)) continue;

                // 取属性上的自定义特性
                objAttr = propInfo.GetCustomAttribute(typeof(OrmColumnAttribute), false);
                if (objAttr == null)
                {
                    _jsonName = _propName;
                }
                else
                {
                    attr = objAttr as OrmColumnAttribute;
                    _jsonName = string.IsNullOrWhiteSpace(attr.Json) ? _propName : attr.Json;
                    //// 记录主键
                    //if (attr.IsKey)
                    //    PrimaryKey.Add(_propName);
                }
                //Propertys.Add(_colName, _propName); //列名
                //Columns.Add(_propName, _colName);
                //
                _Columns.Add(_propName);
                _ColumnDictionary.Add(_propName.ToLower());
                _Jsons.Add(_jsonName);
            }

            SelectColumnString = GetColumnString(_Columns);

            switch (type)
            {
                case TableTypes.Dict:
                    this.RowCache = new Cache<long, T>(3000, 15);
                    this.ListCache = new Cache<string, long[]>(3000, 10);
                    break;
                case TableTypes.List:
                    this.RowCache = new Cache<long, T>(2000, 100);
                    this.ListCache = new Cache<string, long[]>(1000, 20);
                    break;
                case TableTypes.Large:
                    this.RowCache = new Cache<long, T>(60, 1000);
                    this.ListCache = new Cache<string, long[]>(30, 50);
                    break;
                case TableTypes.Small:
                    this.RowCache = new Cache<long, T>(15, 0);
                    this.ListCache = new Cache<string, long[]>(15, 0);
                    break;
                default:
                    this.RowCache = new Cache<long, T>(60, 300);
                    this.ListCache = new Cache<string, long[]>(30, 50);
                    break;
            }
        }

        public static bool Exec()
        {
            return true;
        }
    }

    /// <summary>
    /// 数据表类型
    /// </summary>
    public enum TableTypes
    {
        /// <summary>
        /// 默认，普通表
        /// </summary>
        Default = 0,
        /// <summary>
        /// 字典表
        /// 行数小于300的不变更表，结构极简（无层级和关联）
        /// 就是ID,Name,EName,Descr,Enable
        /// </summary>
        Dict = 1,
        /// <summary>
        /// 列表
        /// 行数小于1000的不变更表，结构简单，可有一个层级且无外部关联(如上级到本表的另一行)
        /// 就是ID,Name,EName,Descr,Parent,Order,Enable
        /// </summary>
        List = 2,
        /// <summary>
        /// 日志表
        /// 可以无ID列，必有CreateTime列
        /// 只可读不可写
        /// </summary>
        Log = 3,
        /// <summary>
        /// 大数据表
        /// 数据量级在10万以上
        /// </summary>
        Large = 4,
        /// <summary>
        /// 小数据表，或者不需要缓存的表
        /// </summary>
        Small = 5
    }
    //public sealed partial class DB
    //{
    //    /// <summary>
    //    /// 固有公共属性(继承类必须排除)
    //    /// </summary>
    //    private static HashSet<string> OmitProperties = new HashSet<string>(
    //        new string[] { "Item", "Comparer", "Keys", "Values", "Count" }, StringComparer.OrdinalIgnoreCase);

    //    //public static ConcurrentDictionary<int, Table<Object>> Tables = new ConcurrentDictionary<int, Table<Object>>();

    //    //public static int InitTables<T>() where T : class, new()
    //    //{
    //    //    Type objType = typeof(T);
    //    //    var className = objType.Name;
    //    //    //取类上的自定义特性
    //    //    object objAttr = objType.GetCustomAttribute(typeof(OrmTableAttribute), true);
    //    //    string tableName = string.Empty;

    //    //    if (objAttr != null)
    //    //    {
    //    //        tableName = (objAttr as OrmTableAttribute).Table;
    //    //    }

    //    //    var table = new Table<Object>(string.IsNullOrEmpty(tableName) ? className : tableName);

    //    //    string _propName, _colName;
    //    //    OrmColumnAttribute attr;
    //    //    var _properties = objType.GetProperties();
    //    //    //遍历全部公共属性
    //    //    foreach (PropertyInfo propInfo in _properties)
    //    //    {
    //    //        _propName = propInfo.Name;
    //    //        // 忽略固有公共属性
    //    //        if (OmitProperties.Contains(_propName)) continue;

    //    //        // 取属性上的自定义特性
    //    //        objAttr = propInfo.GetCustomAttribute(typeof(OrmColumnAttribute), false);
    //    //        if (objAttr != null)
    //    //        {
    //    //            attr = objAttr as OrmColumnAttribute;

    //    //            _colName = string.IsNullOrWhiteSpace(attr.Column) ? _propName : attr.Column;
    //    //            table.Propertys.Add(_colName, _propName); //列名
    //    //            table.Columns.Add(_propName, _colName);
    //    //            // 记录主键
    //    //            if (attr.IsKey)
    //    //                table.PrimaryKey.Add(_propName);

    //    //        }
    //    //        else
    //    //        {
    //    //            _colName = _propName;
    //    //            table.Propertys.Add(_colName, _propName); //列名
    //    //            table.Columns.Add(_propName, _colName);
    //    //        }
    //    //    }
    //    //    var index = DB.Tables.Count;
    //    //    DB.Tables.AddOrUpdate(index, table, (s, t) => { return table; });

    //    //    return index;
    //    //}
    //}
}
