using System;
using System.Collections.Generic;
using System.Text;

namespace Snow.Orm
{
    public partial class Sql
    {
        public string GetWhereString()
        {
            var _cond = IsKeyCondition ? IDCondition : OtherCondition;
            if (_cond.Length > 0)
            {
                return " WHERE " + _cond;
            }
            return "";
        }
        /// <summary>
        /// 原生条件字符串
        /// </summary>
        /// <param name="sqlString"> and id=? or name=?</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Sql Where(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString))
            {
                throw new Exception("数据库操作命令不能为空");
            }
            var cmd = DB.GetRawSql(" " + sqlString + " ", args);
            if (cmd.SqlParams != null) Params.AddRange(cmd.SqlParams);
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(AndOr.And);
            }

            OtherCondition.Append(cmd.SqlString);
            return this;
        }

        #region 主键
        public Sql ID<T>(T val)
        {
            if (IsKeyCondition) IDCondition.Append(" and ");
            IDCondition.Append(DB.GetCondition("id"));
            Params.Add(DB.GetParam("id", val));
            IsKeyCondition = true;
            return this;
        }
        public Sql ID<T>(string col, T val)
        {
            if (IsKeyCondition) IDCondition.Append(" and ");
            IDCondition.Append(DB.GetCondition(col));
            Params.Add(DB.GetParam(col, val));
            IsKeyCondition = true;
            return this;
        }
        #endregion

        #region 等于
        /// <summary>
        /// 等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql Eq<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.Eq, isFunc);
            return this;
        }
        /// <summary>
        /// 或等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrEq<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.Eq, isFunc);
            return this;
        }
        /// <summary>
        /// 不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql UnEq<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.UnE, isFunc);
            return this;
        }
        /// <summary>
        /// 或不等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrUnEq<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.UnE, isFunc);
            return this;
        }

        #endregion

        #region 大于
        /// <summary>
        /// 大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql Gt<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.Gt, isFunc);
            return this;
        }
        /// <summary>
        /// 或大于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrGt<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.Gt, isFunc);
            return this;
        }
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql Gte<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.GtE, isFunc);
            return this;
        }
        /// <summary>
        /// 或大于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrGte<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.GtE, isFunc);
            return this;
        }
        #endregion

        #region 小于

        /// <summary>
        /// 小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql Lt<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.Lt, isFunc);
            return this;
        }
        /// <summary>
        /// 或小于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrLt<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.Lt, isFunc);
            return this;
        }
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql Lte<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.And, Op.LtE, isFunc);
            return this;
        }
        /// <summary>
        /// 或小于等于
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col">列名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public Sql OrLte<T>(string col, T val, bool isFunc = false)
        {
            GetCondition(col, val, AndOr.Or, Op.LtE, isFunc);
            return this;
        }
        #endregion

        #region 空
        /// <summary>
        /// 为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Sql Null(string col)
        {
            GetNullCondition(col, AndOr.And, true);
            return this;
        }
        /// <summary>
        /// 或为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Sql OrNull(string col)
        {
            GetNullCondition(col, AndOr.Or, true);
            return this;
        }
        /// <summary>
        /// 不为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Sql NotNull(string col)
        {
            GetNullCondition(col, AndOr.And, false);
            return this;
        }
        /// <summary>
        /// 或不为空
        /// </summary>
        /// <param name="col">列名</param>
        /// <returns></returns>
        public Sql OrNotNull(string col)
        {
            GetNullCondition(col, AndOr.Or, false);
            return this;
        }
        #endregion

        #region Like

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Sql Like(string col, string val)
        {
            if (GetLikeCondition(col, val))
                Params.Add(DB.GetParam(col, "%" + val + "%"));
            return this;
        }
        /// <summary>
        /// 包含以……开始
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Sql StartWith(string col, string val)
        {
            if (GetLikeCondition(col, val))
                Params.Add(DB.GetParam(col, val + "%"));
            return this;
        }
        /// <summary>
        /// 包含以……结束
        /// </summary>
        /// <param name="col">列名</param>
        /// <param name="val">列值</param>
        /// <returns></returns>
        public Sql EndWith(string col, string val)
        {
            if (GetLikeCondition(col, val))
                Params.Add(DB.GetParam(col, "%" + val));
            return this;
        }
        #endregion

        #region In
        public Sql In<T>(string col, params T[] args)
        {
            GetInCondition(col, AndOr.And, Op.In, args);
            return this;
        }
        public Sql NotIn<T>(string col, params T[] args)
        {
            GetInCondition(col, AndOr.And, Op.NotIn, args);
            return this;
        }
        public Sql OrIn<T>(string col, params T[] args)
        {
            GetInCondition(col, AndOr.Or, Op.In, args);
            return this;
        }
        public Sql OrNotIn<T>(string col, params T[] args)
        {
            GetInCondition(col, AndOr.Or, Op.NotIn, args);
            return this;
        }
        #endregion

        #region Between
        public Sql Between<T>(string col, T t1, T t2)
        {
            GetBetweenCondition(col, t1, t2);
            return this;
        }
        public Sql NotBetween<T>(string col, T t1, T t2)
        {
            GetBetweenCondition(col, t1, t2, AndOr.And, Op.NotBetween);
            return this;
        }
        public Sql OrBetween<T>(string col, T t1, T t2)
        {
            GetBetweenCondition(col, t1, t2, AndOr.Or);
            return this;
        }
        public Sql OrNotBetween<T>(string col, T t1, T t2)
        {
            GetBetweenCondition(col, t1, t2, AndOr.Or, Op.NotBetween);
            return this;
        }
        #endregion

        #region 条件

        void GetCondition<T>(string col, T val, string andor = AndOr.And, string op = Op.Eq, bool isFunc = false)
        {
            if (IsKeyCondition) return;
            if (string.IsNullOrWhiteSpace(col)) return;
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(andor);
            }
            if (isFunc)
            {
                OtherCondition.Append(DB.SetColumnFunc(col, val));
            }
            else
            {
                OtherCondition.Append(DB.GetCondition(col, op));
                Params.Add(DB.GetParam(col, val));
            }
        }
        void GetNullCondition(string col, string andor = AndOr.And, bool isnull = true)
        {
            if (IsKeyCondition) return;
            if (string.IsNullOrWhiteSpace(col)) return;
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(andor);
            }
            OtherCondition.Append(string.Concat("(", DB._RestrictPrefix, col, DB._RestrictPostfix, isnull ? Op.Null : Op.NotNull, ")"));
        }
        void GetInCondition<T>(string colName, string andor = AndOr.And, string op = Op.In, params T[] args)
        {
            if (IsKeyCondition) return;
            if (string.IsNullOrWhiteSpace(colName) || args == null || args.Length == 0) return;
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(andor);
            }
            OtherCondition.Append(string.Concat("(", DB._RestrictPrefix, colName, DB._RestrictPostfix, op, "(", string.Join(",", args), "))"));
        }
        void GetBetweenCondition<T>(string colName, T t1, T t2, string andor = AndOr.And, string op = Op.Between)
        {
            if (IsKeyCondition) return;
            if (string.IsNullOrWhiteSpace(colName) || t1 == null || t2 == null) return;
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(andor);
            }
            OtherCondition.Append(string.Concat("(", DB._RestrictPrefix, colName, DB._RestrictPostfix, op, t1, " AND ", t2, ")"));
        }
        bool GetLikeCondition(string col, string val, string andor = AndOr.And, string op = Op.Like)
        {
            if (IsKeyCondition) return false;
            if (string.IsNullOrWhiteSpace(col) || string.IsNullOrWhiteSpace(val)) return false;
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(andor);
            }
            OtherCondition.Append(string.Concat("(", DB._RestrictPrefix, col, DB._RestrictPostfix, op, DB._ParameterPrefix, col, ")"));
            return true;
        }
        #endregion

        #region 括号
        /// <summary>
        /// 有左括号
        /// </summary>
        bool hasParenthesis = false;
        public Sql LP()
        {
            // 如果有左括号,忽略逻辑运算符
            if (hasParenthesis) { hasParenthesis = false; }
            else if (OtherCondition.Length > 0)
            {
                OtherCondition.Append(AndOr.And);
            }
            OtherCondition.Append(" ( ");
            hasParenthesis = true;
            return this;
        }
        public Sql RP()
        {
            OtherCondition.Append(" ) ");
            return this;
        }
        #endregion
    }
}
