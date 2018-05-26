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
        #region 私有字段
        DB Db;
        /// <summary>
        /// SELECT 语句中的 Fields
        /// </summary>
        static string SelectColumnString;

        Cache<long, T> RowCache;
        Cache<string, long[]> ListCache;
        /// <summary>
        /// 表字段字典(列名的小写字典)
        /// </summary>
        static List<string> _ColumnDictionary = new List<string>();
        #endregion

        ///// <summary>
        ///// (属性名-列)字典
        ///// </summary>
        //public Dictionary<string, string> Columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ///// <summary>
        ///// (列名-属性名)字典
        ///// </summary>
        //public Dictionary<string, string> Propertys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ///// <summary>
        ///// 主键字典
        ///// </summary>
        //public List<string> PrimaryKey = new List<string>();


        /// <summary>
        /// T 属性(表字段)
        /// </summary>
        protected static List<string> _Columns = new List<string>();
        /// <summary>
        /// Json名
        /// </summary>
        protected static List<string> _Jsons = new List<string>();

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string Name { private set; get; }

        public log4net.ILog Log { get { return Db.Log; } }
        /// <summary>
        /// 固有公共属性(继承类必须排除)
        /// </summary>
        private static HashSet<string> OmitProperties = new HashSet<string>(
            new string[] { "Item", "Comparer", "Keys", "Values", "Count" }, StringComparer.OrdinalIgnoreCase);

        public Table(DB db, TableTypes type = TableTypes.Default)
        {
            Db = db;
            Type objType = typeof(T);
            //取类的自定义特性
            var objAttr = objType.GetCustomAttribute(typeof(OrmTableAttribute), true);
            Name = objAttr == null ? objType.Name : (objAttr as OrmTableAttribute).Name;

            string _jsonName, _colName;
            OrmColumnAttribute attr;
            var _properties = objType.GetProperties();
            //遍历类的全部公共属性
            foreach (PropertyInfo propInfo in _properties)
            {
                _colName = propInfo.Name;
                // 忽略固有公共属性
                if (OmitProperties.Contains(_colName)) continue;

                // 取属性上的自定义特性
                objAttr = propInfo.GetCustomAttribute(typeof(OrmColumnAttribute), false);
                if (objAttr == null)
                {
                    _jsonName = _colName;
                }
                else
                {
                    attr = objAttr as OrmColumnAttribute;
                    _jsonName = string.IsNullOrWhiteSpace(attr.JsonName) ? _colName : attr.JsonName;
                }
                _Columns.Add(_colName);
                _ColumnDictionary.Add(_colName.ToLower());
                _Jsons.Add(_jsonName);
            }

            SelectColumnString = GetSelectColumnString(_Columns);

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
}
