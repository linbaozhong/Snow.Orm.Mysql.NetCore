using System;
using System.Collections;

namespace Snow.Orm
{
    //public class Column<T>
    //{
    //    private string _name;
    //    private string _jsonName;
    //    //private bool _isPrimary;
    //    //private bool _isAuto;
    //    Entry _owner;

    //    public Column(Entry owner, string name, string jsonName = null, bool isPrimary = false, bool isAuto = false)
    //    {
    //        if (owner == null || string.IsNullOrWhiteSpace(name))
    //        {
    //            return;
    //        }
    //        _owner = owner;
    //        _name = name;
    //        _jsonName = string.IsNullOrWhiteSpace(jsonName) ? name : jsonName;

    //        //_owner.Table.Columns.Add(name);
    //        DB.Tables[_owner.TableName].Columns.Add("",name);
    //        if (isPrimary)
    //            DB.Tables[_owner.TableName].PrimaryKey = new orm.PrimaryKey(name, isAuto);
    //        //_owner.Table.PrimaryKey = new orm.PrimaryKey(name, isAuto);
    //    }
    //    public Column(string name, string jsonName = null, bool isPrimary = false, bool isAuto = false)
    //    {
    //        _name = name;
    //        _jsonName = string.IsNullOrWhiteSpace(jsonName) ? name : jsonName;
    //    }
    //    /// <summary>
    //    /// 对应的数据表列名
    //    /// </summary>
    //    public string Name
    //    {
    //        get { return _name; }
    //    }
    //    /// <summary>
    //    /// 列值
    //    /// </summary>
    //    public T Value
    //    {
    //        set
    //        {
    //            _owner.OnColumnChange(_name, value);
    //        }
    //        get
    //        {
    //            return (T)_owner[_name];
    //            //if (_owner.Contains(_name))
    //            //{
    //            //    return (T)_owner[_name];
    //            //}
    //            //return default(T);
    //        }
    //    }
    //    /// <summary>
    //    /// json名
    //    /// </summary>
    //    protected string JsonName
    //    {
    //        get { return _jsonName; }
    //    }
    //    ///// <summary>
    //    ///// 是否主键
    //    ///// </summary>
    //    //protected bool IsPrimary
    //    //{
    //    //    get { return _isPrimary; }
    //    //}
    //    ///// <summary>
    //    ///// 是否由DB自动生成
    //    ///// </summary>
    //    //protected bool IsAuto
    //    //{
    //    //    get { return _isAuto; }
    //    //}
    //}

    public class Column
    {
        private string _name;
        private Type _type;
        public Column(string name)
        {
            _name = name;
        }
        public Column(string name, Type type) : this(name)
        {
            _type = type;
        }
        /// <summary>
        /// 列名
        /// </summary>
        public string Name
        {
            get { return _name; }
        }


        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }
    }

}
