using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        /// <summary>
        /// Select Fields 字典
        /// </summary>
        static ConcurrentDictionary<string, string> SelectColumns = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string TableString
        {
            get { return DB.GetName(Name); }
        }
        string FromTableString
        {
            get { return " FROM " + TableString; }
        }
        /// <summary>
        /// 构造Select语句中的Fields
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        static string GetSelectColumnString(IEnumerable<string> cols)
        {
            var _strings = new List<string>();
            foreach (var col in cols)
            {
                _strings.Add(DB.GetName(col));  // 列
            }
            return string.Join(",", _strings);
        }
        /// <summary>
        /// 构造Select语句中的Fields
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static string GetSelectColumnStringFromArgs(IEnumerable<string> args)
        {
            var _selectColumns = string.Empty;
            var _key = args.Join();
            if (SelectColumns.TryGetValue(_key, out _selectColumns)) return _selectColumns;

            var _strings = new List<string>();
            //var _index = -1;
            foreach (var arg in args)
            {
                //_index = _Columns.IndexOf(arg);
                if (_Columns.Contains(arg))
                {
                    _strings.Add(DB.GetName(arg));   // 聚合函数
                    continue;
                }
                _strings.Add(arg);
            }
            _selectColumns = _strings.Join();
            SelectColumns.TryAdd(_key, _selectColumns);
            return _selectColumns;
        }

        static string GetWhereCondition(T bean, List<DbParameter> Params)
        {
            var _cond = new List<string>();
            foreach (var item in bean)
            {
                _cond.Add(GetCondition(item.Key, item.Value, Params));
            }
            if (_cond.Count > 0)
                return " WHERE " + string.Join(" and ", _cond);

            return "";
        }
        static string GetCondition<V>(string col, V val, List<DbParameter> Params)
        {
            Params.Add(DB.GetParam(col, val));
            return DB.GetCondition(col, Op.Eq);
        }

        static string CombineCacheKey(string col, string val, string orderby, uint count)
        {
            StringBuilder ck = new StringBuilder(100);
            ck.Append(count);
            ck.Append("\n");
            ck.Append(orderby);
            ck.Append("\n");
            if (col == null || col.Length < 1 || val == null) return ck.ToString();
            ck.Append(col);
            ck.Append("\n");
            ck.Append(val);
            ck.Append("\n");
            return ck.ToString();
        }
        static string CombineCacheKey(T bean, string orderby, uint count)
        {
            StringBuilder ck = new StringBuilder(100);
            ck.Append(count);
            ck.Append("\n");
            ck.Append(orderby);
            ck.Append("\n");
            if (bean == null) return ck.ToString();
            foreach (var kv in bean)
            {
                if (kv.Key == null || kv.Key.Length < 1) continue;
                ck.Append(kv.Key);
                ck.Append("\n");
                ck.Append(kv.Value);
                ck.Append("\n");
            }
            return ck.ToString();
        }
        static string CombineCacheKey(Sql cond)
        {
            if (cond == null) return null;
            StringBuilder ck = new StringBuilder(100);
            ck.Append(cond.GetWhereString());
            ck.Append("\n");
            ck.Append(cond.GetGroupbyString());
            ck.Append("\n");
            ck.Append(cond.GetPageString());
            ck.Append("\n");
            if (cond.Params != null)
            {
                string s = null;
                foreach (DbParameter p in cond.Params)
                {
                    if (p == null || p.Value == null) continue;
                    s = p.Value.ToString();
                    ck.Append(p.ParameterName);
                    ck.Append("=");
                    if (s != null && s.Length > 3000) ck.AppendLine(s.Substring(0, 3000));
                    else ck.AppendLine(s);
                }
            }
            return ck.ToString();
        }
    }
}
