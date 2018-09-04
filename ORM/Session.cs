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
}
