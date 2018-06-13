using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Snow.Orm
{
    public partial class Table<T>
    {
        T _Get(string sql, List<DbParameter> Params, IEnumerable<string> cols = null)
        {
            using (var dr = Db.Read(sql, Params))
            {
                if (dr.Read())
                {
                    var _obj = new T() as BaseEntity;
                    var i = 0;
                    if (cols == null || cols.Count() == 0) cols = _Columns;
                    foreach (var item in cols)
                    {
                        _obj[item] = dr.IsDBNull(i) ? null : (dr.GetDataTypeName(i) == "TINYINT" ? dr.GetInt16(i) : dr[i]);
                        i++;
                    }
                    return _obj as T;
                }
            }
            return null;
        }

        List<T> _Gets(string sql, List<DbParameter> Params, IEnumerable<string> cols = null)
        {
            using (var dr = Db.Read(sql, Params))
            {
                if (dr.HasRows)
                {
                    var _list = new List<T>();
                    if (cols == null || cols.Count() == 0) cols = _Columns;
                    while (dr.Read())
                    {
                        var _obj = new T() as BaseEntity;
                        var i = 0;
                        foreach (var item in cols)
                        {
                            _obj[item] = dr.IsDBNull(i) ? null : (dr.GetDataTypeName(i) == "TINYINT" ? dr.GetInt16(i) : dr[i]);
                            i++;
                        }
                        _list.Add(_obj as T);
                    }
                    return _list;
                }
            }
            return null;
        }

        long[] _GetIds(StringBuilder sql, List<DbParameter> Params)
        {
            using (var dr = Db.Read(sql.ToString(), Params))
            {
                if (dr.HasRows)
                {
                    var _ids = new List<long>();
                    while (dr.Read())
                    {
                        _ids.Add(dr[0].ToLong());
                    }
                    return _ids.ToArray();
                }
            }
            return null;
        }

        bool _Exist(StringBuilder sql, List<DbParameter> Params)
        {
            //if (Db.IsDebug) Db.ShowSqlString(sql.ToString(), Params);
            using (var dr = Db.Read(sql.ToString(), Params))
            {
                if (dr.Read())
                {
                    return !dr.IsDBNull(0);
                }
            }
            return false;
        }
        int _Count(StringBuilder sql, List<DbParameter> Params)
        {
            //if (Db.IsDebug) Db.ShowSqlString(sql.ToString(), Params);
            using (var dr = Db.Read(sql.ToString(), Params))
            {
                if (dr.Read())
                {
                    return dr[0].ToInt();
                }
            }
            return 0;
        }

    }
}
