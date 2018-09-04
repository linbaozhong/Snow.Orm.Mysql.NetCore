using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    #region 会话

    public class Session
    {
        internal MySqlConnection _Connection = null;
        internal MySqlCommand _Command = null;
        private MySqlTransaction _Transaction = null;
        public Session(string connstring)
        {
            if (_Connection.State == ConnectionState.Closed) _Connection = new MySqlConnection(connstring);
        }
        public void BeginTransaction()
        {
            _Connection.Open();
            _Transaction = _Connection.BeginTransaction();
            _Command = new MySqlCommand(null, _Connection, _Transaction);
        }
        public void Rollback()
        {
            _Transaction.Rollback();
            _Transaction.Dispose();
            if (_Connection.State != ConnectionState.Closed) _Connection.Close();
        }
        public void Commit()
        {
            _Transaction.Commit();
            _Transaction.Dispose();
            if (_Connection.State != ConnectionState.Closed) _Connection.Close();
        }
    }
    #endregion

    #region DAL 返回对象
    public class DalResult : IDisposable
    {
        static ObjectPool<DalResult> pool = new ObjectPool<DalResult>(() =>
        { return new DalResult(); }, x => {
            x.Id = 0;
            x.Rows = 0;
            x.Success = false;
        }, 100);

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && pool.PutObject(this))
            {
                this.Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        public bool Disposed { set; get; } = false;
        public static DalResult Factory
        {
            get
            {
                var obj = pool.GetObject();
                obj.Disposed = false;
                return obj;
            }
        }
        /// <summary>
        /// 成功与否
        /// </summary>
        public bool Success { set; get; } = false;
        /// <summary>
        /// 受影响的行数
        /// </summary>
        public int Rows { set; get; } = 0;
        /// <summary>
        /// insert的自增id
        /// </summary>
        public long Id { set; get; } = 0;
    }
    #endregion
}
