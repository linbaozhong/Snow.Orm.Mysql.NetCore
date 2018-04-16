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
        }, x => { x.ResetFunc(); }, 100);

        void ResetFunc()
        {
            IDCondition.Clear();
            OtherCondition.Clear();
            Params.Clear();
            IsKeyCondition = false;
            OtherCondition.Clear();
            ShowSQL = false;
            _Columns.Clear();
            _GroupBy = string.Empty;
            _Having = string.Empty;
            _Join.Clear();
            _OmitColumns.Clear();
            _OrderBy.Clear();
            _SetColumns.Clear();
            Array.Clear(_Page, 0, _Page.Length);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                pool.PutObject(this);
            }
        }

        public static Sql Factory
        {
            get { return pool.GetObject(); }
        }
        ~Sql()
        {
            this.Dispose(false);
        }
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
