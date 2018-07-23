using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    /// <summary>
    /// 查询条件对象
    /// </summary>
    public partial class Sql : IDisposable
    {
        static ObjectPool<Sql> pool = new ObjectPool<Sql>(() =>
        {
            return new Sql();
        }, x =>
        {
            x.Disposed = false;
            x._Columns.Clear();
            x._GroupBy = string.Empty;
            x._Having = string.Empty;
            x._Join.Clear();
            x._OmitColumns.Clear();
            x._OrderBy.Clear();
            x._SetColumns.Clear();
            Array.Clear(x._Page, 0, x._Page.Length);
            x.IDCondition.Clear();
            x.OtherCondition.Clear();
            x.IsKeyCondition = false;
            x.OtherCondition.Clear();
            x.Params.Clear();
        }, 100);

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disposed = true;
                if (pool.PutObject(this))
                    GC.SuppressFinalize(this);
            }
        }

        public static Sql Factory
        {
            get
            {
                var _sql = pool.GetObject();
                //_sql.Disposed = false;
                return _sql;
                //return new Sql();
            }
        }
        public bool Disposed { set; get; } = false;
        /// <summary>
        /// 主键条件
        /// </summary>
        protected internal StringBuilder IDCondition = new StringBuilder();
        /// <summary>
        /// 其它条件
        /// </summary>
        StringBuilder OtherCondition = new StringBuilder();
        /// <summary>
        /// SQL参数
        /// </summary>
        protected internal List<DbParameter> Params = new List<DbParameter>();
        /// <summary>
        /// 是否主键查询
        /// </summary>
        public bool IsKeyCondition { private set; get; } = false;
    }

    public sealed class AndOr
    {
        /// <summary>
        /// 与
        /// </summary>
        public const string And = " And ";
        /// <summary>
        /// 或
        /// </summary>
        public const string Or = " Or ";
        /// <summary>
        /// 非
        /// </summary>
        public const string Not = " Not ";
    }
    public sealed class Op
    {
        /// <summary>
        /// 等于
        /// </summary>
        public const string Eq = " = ";
        /// <summary>
        /// 不等于
        /// </summary>
        public const string UnE = " != ";
        /// <summary>
        /// 大于
        /// </summary>
        public const string Gt = " > ";
        /// <summary>
        /// 大于等于
        /// </summary>
        public const string GtE = " >= ";
        /// <summary>
        /// 小于
        /// </summary>
        public const string Lt = " < ";
        /// <summary>
        /// 小于等于
        /// </summary>
        public const string LtE = " <= ";
        /// <summary>
        /// 包含
        /// </summary>
        public const string Like = " LIKE ";
        /// <summary>
        /// IN
        /// </summary>
        public const string In = " IN ";
        /// <summary>
        /// NOT IN
        /// </summary>
        public const string NotIn = " NOT IN ";
        public const string Between = " BETWEEN ";
        public const string NotBetween = " NOT BETWEEN ";
        /// <summary>
        /// NULL
        /// </summary>
        public const string Null = " IS NULL ";
        /// <summary>
        /// NOT NULL
        /// </summary>
        public const string NotNull = " IS NOT NULL ";
    }

}
