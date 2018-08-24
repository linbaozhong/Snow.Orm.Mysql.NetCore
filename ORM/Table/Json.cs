using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using adeway;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        T _Clone(T bean)
        {
            if (bean == null) return null;
            var _bean = new T();
            foreach (var item in bean)
            {
                _bean.Add(item.Key, item.Value);
            }
            return _bean;
        }
        /// <summary>
        /// 获取一个实体的指定列
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        //public T _Get(T bean, params string[] args)
        //{
        //    if (bean == null) return null;
        //    if (args.Length == 0) return _Clone(bean);

        //    return _Get(bean, (IEnumerable<string>)args);
        //}
        /// <summary>
        /// 获取一个LIST的指定列
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> Gets(List<T> list, params string[] args)
        {
            if (args.Length == 0) return list;
            if (list == null || list.Count == 0) return null;
            var _jsons = new List<T>(list.Count);
            for (var j = 0; j < list.Count; j++)
            {
                var bean = list[j] as T;
                _jsons.Add(_Get(bean, (IEnumerable<string>)args));
            }
            return _jsons;
        }

        /// <summary>
        /// 获取一个实体的指定列
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T _Get(T bean, IEnumerable<string> args)
        {
            if (bean == null) return null;
            if (args.Count() == 0) return _Clone(bean);

            var _json = new T();
            foreach (var key in args)
            {
                _json.Add(key, bean.TryGet(key, DBNull.Value));
            }
            return _json as T;
        }
        /// <summary>
        /// 获取一个实体的指定列的JSON表示(如果实体类有定义json属性)
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T GetJson(T bean, params string[] args)
        {
            if (bean == null) return null;
            if (args.Length == 0) args = _TColumns.ToArray();

            return _GetJson(bean as BaseEntity, args);
        }
        /// <summary>
        /// 获取一个LIST的指定列的JSON表示(如果实体类有定义json属性)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> GetJsons(List<T> list, params string[] args)
        {
            if (list == null || list.Count == 0) return null;
            var _jsons = new List<T>(list.Count);
            if (args.Length == 0) args = _TColumns.ToArray();
            for (var i = 0; i < list.Count; i++)
            {
                var bean = list[i] as BaseEntity;
                _jsons.Add(_GetJson(bean, args));
            }
            return _jsons;
        }
        static T _GetJson(BaseEntity bean, IEnumerable<string> args)
        {
            var _json = new T() as BaseEntity;
            var _jsonName = string.Empty;
            foreach (var key in args)
            {
                if (_ColumnDictionary.TryGetValue(key, out _jsonName))
                    _json.Add(_jsonName, bean.TryGet(key, DBNull.Value));
                else
                    _json.Add(key, DBNull.Value);
            }
            return _json as T;
        }
    }
}
