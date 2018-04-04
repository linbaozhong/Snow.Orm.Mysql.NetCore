using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public sealed partial class Dal
    {
        StringBuilder _IDCondition = new StringBuilder();
        StringBuilder _Condition = new StringBuilder();
        List<DbParameter> _Params = new List<DbParameter>();

        #region 表
        public Dal Table(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return this;
            this._tableName = tableName;
            return this;
        }
        #endregion

        #region 主键(失效)
        /// <summary>
        /// 主键等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IDCol">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal ID<T>(Column<T> IDCol)
        //{
        //    if (_IDCondition.Length > 0) return this;
        //    _IDCondition.Append(dbs.Db.GetCondition(IDCol));
        //    _Params.Add(dbs.Db.GetParam(IDCol));
        //    return this;
        //}
        /// <summary>
        /// 联合主键等于
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="IDCol1">Column<T> 列对象</param>
        /// <param name="IDCol2">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal ID<T1, T2>(Column<T1> IDCol1, Column<T2> IDCol2)
        //{
        //    if (_IDCondition.Length > 0) return this;
        //    _IDCondition.Append(dbs.Db.GetCondition(IDCol1));
        //    _Params.Add(dbs.Db.GetParam(IDCol1));
        //    _IDCondition.Append(" AND " + dbs.Db.GetCondition(IDCol2));
        //    _Params.Add(dbs.Db.GetParam(IDCol2));
        //    return this;
        //}
        #endregion

        #region 主键
        /// <summary>
        /// 主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal ID<T>(string col, T val)
        {
            if (_IDCondition.Length > 0) return this;
            _IDCondition.Append(dbs.Db.GetCondition(col));
            _Params.Add(dbs.Db.GetParam(col, val));
            return this;
        }

        #endregion

        #region 等于(失效)
        /// <summary>
        /// 等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Eq<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Eq(col.Name, col.Value);
        //}
        /// <summary>
        /// 或等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrEq<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrEq(col.Name, col.Value);
        //}
        /// <summary>
        /// 不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal UnEq<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return UnEq(col.Name, col.Value);
        //}
        /// <summary>
        /// 或不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrUnEq<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrUnEq(col.Name, col.Value);
        //}

        #endregion

        #region 等于
        /// <summary>
        /// 等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal Eq<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.Eq);
            return this;
        }
        /// <summary>
        /// 或等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrEq<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.Eq);
            return this;
        }
        /// <summary>
        /// 不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal UnEq<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.UnE);
            return this;
        }
        /// <summary>
        /// 或不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrUnEq<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.UnE);
            return this;
        }

        #endregion

        #region 大于(失效)
        /// <summary>
        /// 大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Gt<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Gt(col.Name,col.Value);
        //}
        /// <summary>
        /// 或大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrGt<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrGt(col.Name, col.Value);
        //}
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Gte<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Gte(col.Name, col.Value);
        //}
        /// <summary>
        /// 或大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrGte<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrGte(col.Name, col.Value);
        //}

        #endregion

        #region 大于
        /// <summary>
        /// 大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal Gt<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.Gt);
            return this;
        }
        /// <summary>
        /// 或大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrGt<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.Gt);
            return this;
        }
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal Gte<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.GtE);
            return this;
        }
        /// <summary>
        /// 或大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrGte<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.GtE);
            return this;
        }
        #endregion

        #region 小于(失效)
        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Lt<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Lt(col.Name, col.Value);
        //}
        /// <summary>
        /// 或小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrLt<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrLt(col.Name, col.Value);
        //}
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Lte<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Lte(col.Name, col.Value);
        //}
        /// <summary>
        /// 或小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrLte<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrLte(col.Name, col.Value);
        //}
        #endregion

        #region 小于

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal Lt<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.Lt);
            return this;
        }
        /// <summary>
        /// 或小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrLt<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.Lt);
            return this;
        }
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal Lte<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.And, Op.LtE);
            return this;
        }
        /// <summary>
        /// 或小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Dal OrLte<T>(string col, T val)
        {
            GetCondition(col, val, AndOr.Or, Op.LtE);
            return this;
        }
        #endregion

        #region 空(失效)
        /// <summary>
        /// 为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Null<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Null(col.Name);
        //}
        /// <summary>
        /// 或为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrNull<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrNull(col.Name);
        //}
        /// <summary>
        /// 不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal NotNull<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return NotNull(col.Name);
        //}
        /// <summary>
        /// 或不为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal OrNotNull<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return OrNotNull(col.Name);
        //}
        #endregion

        #region 空
        /// <summary>
        /// 为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Dal Null(string col)
        {
            GetNullCondition(col, AndOr.And, true);
            return this;
        }
        /// <summary>
        /// 或为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Dal OrNull(string col)
        {
            GetNullCondition(col, AndOr.Or, true);
            return this;
        }
        /// <summary>
        /// 不为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Dal NotNull(string col)
        {
            GetNullCondition(col, AndOr.And, false);
            return this;
        }
        /// <summary>
        /// 或不为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Dal OrNotNull(string col)
        {
            GetNullCondition(col, AndOr.Or, false);
            return this;
        }
        #endregion

        #region Like(失效)
        /// <summary>
        /// 包含
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal Like<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return Like(col.Name, col.Value.ToStr());
        //}
        /// <summary>
        /// 包含以……开始
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal StartWith<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return StartWith(col.Name, col.Value.ToStr());
        //}
        /// <summary>
        /// 包含以……结束
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">Column<T> 列对象</param>
        /// <returns></returns>
        //public Dal EndWith<T>(Column<T> col)
        //{
        //    if (col == null) return this;
        //    return EndWith(col.Name, col.Value.ToStr());
        //}
        #endregion

        #region Like

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Dal Like(string col, string val)
        {
            if (GetLikeCondition(col, val))
                _Params.Add(dbs.Db.GetParam(col, "%" + val + "%"));
            return this;
        }
        /// <summary>
        /// 包含以……开始
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Dal StartWith(string col, string val)
        {
            if (GetLikeCondition(col, val))
                _Params.Add(dbs.Db.GetParam(col, val + "%"));
            return this;
        }
        /// <summary>
        /// 包含以……结束
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Dal EndWith(string col, string val)
        {
            if (GetLikeCondition(col, val))
                _Params.Add(dbs.Db.GetParam(col, "%" + val));
            return this;
        }
        #endregion

        #region In
        public Dal In<T>(string colName, params T[] args)
        {
            GetInCondition(colName, AndOr.And, Op.In, args);
            return this;
        }
        public Dal NotIn<T>(string colName, params T[] args)
        {
            GetInCondition(colName, AndOr.And, Op.NotIn, args);
            return this;
        }
        public Dal OrIn<T>(string colName, params T[] args)
        {
            GetInCondition(colName, AndOr.Or, Op.In, args);
            return this;
        }
        public Dal OrNotIn<T>(string colName, params T[] args)
        {
            GetInCondition(colName, AndOr.Or, Op.NotIn, args);
            return this;
        }
        #endregion

    }
}
