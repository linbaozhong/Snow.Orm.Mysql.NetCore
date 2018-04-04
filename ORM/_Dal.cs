using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Collections;

namespace Snow.Orm
{
    public sealed partial class Dal : IDisposable
    {
        #region 构造函数
        private static ObjectPool<Dal> Pool = new ObjectPool<Dal>(() =>
        {
            return Activator.CreateInstance<Dal>();
        }, d =>
        {
            d._tableName = null;
            d._Join.Clear();
            d._Columns.Clear();
            d._Condition.Clear();
            d._IDCondition.Clear();
            d._OmitColumns.Clear();
            d._OrderBy.Clear();
            d._Params.Clear();
            d._SetColumns.Clear();
            d._Page = new uint[2];
            d._Having = string.Empty;
            d._GroupBy = string.Empty;
        }, 10);

        /// <summary>
        /// 表名
        /// </summary>
        private string _tableName;
        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <returns></returns>
        public static Dal New()
        {
            return Pool.GetObject();
        }
        /// <summary>
        /// 是否打印sql语句
        /// </summary>
        public static bool ShowSQL;

        private void Finish(string sql, List<DbParameter> param, BaseEntity bean = null)
        {
            if (ShowSQL)
            {
                Debug(sql, param);
            }
        }

        public void Dispose()
        {
            Pool.PutObject(this);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 条件

        void GetCondition<T>(string col, T val, string andor = AndOr.And, string op = Op.Eq)
        {
            if (_IDCondition.Length > 0) return;
            if (string.IsNullOrWhiteSpace(col)) return;
            if (_Condition.Length > 0)
            {
                _Condition.Append(andor);
            }
            _Condition.Append(dbs.Db.GetCondition(col, op));
            _Params.Add(dbs.Db.GetParam(col, val));
        }
        void GetNullCondition(string col, string andor = AndOr.And, bool isnull = true)
        {
            if (_IDCondition.Length > 0) return;
            if (string.IsNullOrWhiteSpace(col)) return;
            if (_Condition.Length > 0)
            {
                _Condition.Append(andor);
            }
            _Condition.Append(string.Concat(DB._RestrictPrefix, col, DB._RestrictPostfix, isnull ? Op.Null : Op.NotNull));
        }
        void GetInCondition<T>(string colName, string andor = AndOr.And, string op = Op.In, params T[] args)
        {
            if (_IDCondition.Length > 0) return;
            if (string.IsNullOrWhiteSpace(colName) || args == null || args.Length == 0) return;
            if (_Condition.Length > 0)
            {
                _Condition.Append(andor);
            }
            _Condition.Append(string.Concat(DB._RestrictPrefix, colName, DB._RestrictPostfix, op, "(", string.Join(",", args), ")"));
        }
        bool GetLikeCondition(string col, string val, string andor = AndOr.And, string op = Op.Like)
        {
            if (_IDCondition.Length > 0) return false;
            if (string.IsNullOrWhiteSpace(col) || string.IsNullOrWhiteSpace(val)) return false;
            if (_Condition.Length > 0)
            {
                _Condition.Append(andor);
            }
            _Condition.Append(string.Concat(DB._RestrictPrefix, col, DB._RestrictPostfix, op, DB._ParameterPrefix, col));
            return true;
        }
        #endregion

        #region 日志
        void Error(string sql, List<DbParameter> param, Exception e = null)
        {
            Console.WriteLine(DB.Error(e, sql, param));
        }
        void Error(string msg)
        {
            Console.WriteLine(msg);
        }
        void Debug(string sql, List<DbParameter> param)
        {
            Console.WriteLine(DB.Debug(sql, param));
        }
        #endregion

        #region 操作数据库
        /// <summary>
        /// 获取查询语句
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetSelectSql(BaseEntity bean)
        {
            return string.Concat("SELECT ",
                GetColumns(bean),
                GetFrom(bean),
                (_Join.Count > 0 ? string.Join(" ", _Join) : string.Empty),
                GetSelectCondition(bean),
                GetOrderBy(),
                GetGroupBy()
                );
        }
        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        string GetGroupBy()
        {
            StringBuilder _sql = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(_GroupBy))
            {
                _sql.Append(string.Format(" groupby {0}", _GroupBy));
                //Having
                if (!string.IsNullOrWhiteSpace(_Having))
                {
                    _sql.Append(string.Format(" having {0}", _Having));
                }
            }
            return _sql.ToString();
        }
        /// <summary>
        /// 获取分页查询语句
        /// </summary>
        /// <returns></returns>
        string GetPage()
        {
            if (_Page.Length == 0) return string.Empty;
            var _offset = _Page[0] * _Page[1];
            if (_offset == 0)
            {
                if (_Page[1] > 0)
                    return $" limit {_Page[1]};";
                else
                    return string.Empty;
            }
            else if (_offset > 0)
            {
                if (_Page[1] > 0)
                    return $" offset {_offset} limit {_Page[1]};";
                else
                    return string.Empty;
            }
            return string.Empty;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetSelectCondition(BaseEntity bean)
        {
            if (_IDCondition.Length == 0 && _Condition.Length == 0)
            {
                var _primaryKey = DB.Tables[bean.Index].PrimaryKey;
                var _cols = DB.Tables[bean.Index].Columns;
                string _key;
                foreach (var item in bean)
                {
                    _key = _cols.TryGet(item.Key, item.Key);
                    if (_primaryKey.Contains(item.Key))
                        ID(_key, item.Value);
                    else
                        GetCondition(_key, item.Value);
                }
            }
            if (_IDCondition.Length > 0)
                return " WHERE " + _IDCondition.ToString();
            if (_Condition.Length > 0)
                return " WHERE " + _Condition.ToString();

            return "";
        }
        /// <summary>
        /// From
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetFrom(BaseEntity bean)
        {
            return " FROM " + GetTableName(bean);
        }
        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetTableName(BaseEntity bean)
        {
            return dbs.Db.GetName(string.IsNullOrWhiteSpace(_tableName) ? DB.Tables[bean.Index].Name : _tableName);
        }
        string GetSetColumn(BaseEntity bean)
        {
            if (_SetColumns.Count > 0)
            {
                return string.Join(",", _SetColumns);
            }
            if (_Columns.Count > 0)
            {
                foreach (var key in _Columns)
                {
                    Set(key, bean[key]);
                }
            }
            else
            {
                var _primaryKey = DB.Tables[bean.Index].PrimaryKey;
                var _cols = DB.Tables[bean.Index].Columns;
                string _col;
                foreach (var item in bean)
                {
                    _col = _cols.TryGet(item.Key, item.Key);
                    if (_primaryKey.Contains(item.Key))
                        ID(_col, item.Value);
                    else
                        Set(_col, item.Value);
                }
            }
            if (_SetColumns.Count > 0)
            {
                return string.Join(",", _SetColumns);
            }
            return "";
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        string GetOrderBy()
        {
            if (_OrderBy.Count > 0)
            {
                return " ORDER BY " + string.Join(",", _OrderBy);
            }
            return "";
        }
        /// <summary>
        /// 获取列
        /// </summary>
        /// <param name="bean"></param>
        /// <returns></returns>
        string GetColumns(BaseEntity bean)
        {
            if (_Columns.Count > 0)
            {
                return GetColumns(_Columns.ToArray());
            }
            var cols = DB.Tables[bean.Index].Columns;
            if (_OmitColumns.Count > 0)
            {
                foreach (var col in cols.Keys)
                {
                    if (_OmitColumns.Contains(cols[col]))
                    {
                        continue;
                    }
                    _Columns.Add(cols[col]);
                }
                return GetColumns(_Columns.ToArray());
            }

            foreach (var col in cols.Keys)
            {
                _Columns.Add(cols[col]);
            }
            return GetColumns(_Columns.ToArray());
        }

        string GetColumns(string[] cols)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i] = dbs.Db.GetName(cols[i]);
            }
            return string.Join(",", cols);
        }

        void GetParameter(string sql, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sql)) return;

            if (args == null || args.Length == 0)
            {
                _Condition.Append(sql);
                return;
            }

            string _prefix = "p_", _pname = "";
            int _pos = 0;
            for (int i = 0; i < args.Length; i++)
            {
                _pname = _prefix + i;
                _pos = sql.IndexOf("?");
                if (_pos < 0) break;

                sql = string.Concat(sql.Substring(0, _pos), DB._ParameterPrefix + _pname, sql.Substring(_pos + 1));
                _Params.Add(dbs.Db.GetParam(_pname, args[i].ToStr()));
            }
            _Condition.Append(sql);
        }
        #endregion
    }
}
