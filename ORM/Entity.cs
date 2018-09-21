using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Snow.Orm
{
    /// <summary>
    /// 数据库表实体类的基类
    /// 约定:
    ///     键:ID int类型或long类型(通常情况下是自增型字段)
    /// </summary>
    public abstract class BaseEntity : Dictionary<string, object>
    {
        /// <summary>
        /// 实体抽象类
        /// </summary>
        public BaseEntity() : base(StringComparer.OrdinalIgnoreCase) { }

        public new object this[string key]
        {
            set { base[key] = value; }
            get
            {
                if (TryGetValue(key, out object _val)) return _val;
                return null;
            }
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public new BaseEntity Add(string key, object value)
        {
            base[key] = value;
            return this;
        }

        #region 事件
        ///// <summary>
        ///// 属性改变事件处理句柄
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected delegate void PropertyChangedHandler(object sender, PropertyChangedEventArgs e);

        ///// <summary>
        ///// 属性委托处理句柄
        ///// </summary>
        //private PropertyChangedHandler _OnPropertyChanged = null;

        ///// <summary>
        ///// 对象属性改变时发生事件
        ///// </summary>
        //protected event PropertyChangedHandler OnPropertyChanged
        //{
        //    add { _OnPropertyChanged += value; }
        //    remove { _OnPropertyChanged -= value; }
        //}
        #endregion

        #region 扩展方法

        /// <summary>
        /// 写入器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        protected void Set<T>(T value, [CallerMemberName]string name = null)
        {
            base[name] = value;
            ////属性改变事件
            //if (_OnPropertyChanged != null)
            //{
            //    PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
            //    _OnPropertyChanged(this, e);
            //}
        }
        /// <summary>
        /// 读取器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T Get<T>([CallerMemberName]string name = null)
        {
            if (TryGetValue(name, out object _val))
            {
                if (_val == null) return default(T);
                try { return (T)Convert.ChangeType(_val, typeof(T)); }
                catch (Exception e) {
                    throw e;
                }
            }
            return default(T);
        }
        #endregion
    }
}
