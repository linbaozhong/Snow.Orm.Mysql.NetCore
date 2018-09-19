using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;

namespace Snow.Orm
{
    public abstract class MySqlDB
    {

        public const string _RestrictPrefix = "`";
        public const string _RestrictPostfix = "`";
        /// <summary>
        /// insert成功后返回受影响的行数和自增id
        /// </summary>
        protected const string _InsertReturn = "select ROW_COUNT(),LAST_INSERT_ID();";
        public static DbParameter GetParam<T>(string col, T val, int maxsize = 0)
        {
            if (string.IsNullOrWhiteSpace(col)) return null;
            MySqlParameter sp = new MySqlParameter();
            sp.ParameterName = col;
            sp.MySqlDbType = MySqlDbTypeMap[typeof(T)];
            if (val == null)
            {
                sp.IsNullable = true;
                sp.Value = DBNull.Value;
                return sp;
            }
            if (sp.MySqlDbType == MySqlDbType.VarChar)
            {
                var len = val.ToString().Length;
                if (maxsize < 0 || len > 65500)
                    sp.MySqlDbType = MySqlDbType.LongText;
                else
                {
                    sp.MySqlDbType = MySqlDbType.VarChar; sp.Size = Math.Max(maxsize, len);
                }
            }
            sp.Value = val;
            return sp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
        public static DbParameter GetParam(string col, string val = null, int maxsize = 0)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter();
            sp.ParameterName = col;
            if (val == null)
            {
                sp.MySqlDbType = MySqlDbType.VarChar;
                sp.IsNullable = true;
                sp.Value = DBNull.Value;
                return sp;
            }
            if (maxsize < 0 || val.Length > 65500) sp.MySqlDbType = MySqlDbType.LongText;
            else { sp.MySqlDbType = MySqlDbType.VarChar; sp.Size = Math.Max(maxsize, val.Length); }
            sp.Value = val;
            return sp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
		public static DbParameter GetParam(string col, byte[] val, int maxsize = 0)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter();
            sp.ParameterName = col;
            if (val == null)
            {
                sp.MySqlDbType = MySqlDbType.VarBinary;
                sp.IsNullable = true;
                sp.Value = DBNull.Value;
                return sp;
            }
            if (maxsize < 0 || val.Length > 65500) sp.MySqlDbType = MySqlDbType.LongBlob;
            else { sp.MySqlDbType = MySqlDbType.VarBinary; sp.Size = Math.Max(maxsize, val.Length); }
            sp.Value = val;
            return sp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
		public static DbParameter GetParam(string col, int val)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter(col, MySqlDbType.Int32);
            sp.Value = val;
            return sp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DbParameter GetParam(string col, decimal val)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter(col, MySqlDbType.Decimal);
            sp.Value = val;
            return sp;
        }
        protected MySqlConnection Connection(string connectionString) { return new MySqlConnection(connectionString); }
        protected DbCommand Command(string sql, MySqlConnection conn, CommandType type = CommandType.Text, uint timeout = 0)
        {
            var cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;
            if (timeout > 0) cmd.CommandTimeout = (int)timeout;
            return cmd;
        }
        protected DbDataAdapter DataAdapter() { return new MySqlDataAdapter(); }
        private static Dictionary<Type, MySqlDbType> MySqlDbTypeMap = GetTypeMap();
        private static Dictionary<Type, MySqlDbType> GetTypeMap()
        {
            var typeMap = new Dictionary<Type, MySqlDbType>();
            typeMap[typeof(byte)] = MySqlDbType.Byte;
            typeMap[typeof(sbyte)] = MySqlDbType.Byte;
            typeMap[typeof(short)] = MySqlDbType.Int16;
            typeMap[typeof(ushort)] = MySqlDbType.UInt16;
            typeMap[typeof(int)] = MySqlDbType.Int32;
            typeMap[typeof(uint)] = MySqlDbType.UInt32;
            typeMap[typeof(long)] = MySqlDbType.Int64;
            typeMap[typeof(ulong)] = MySqlDbType.UInt64;
            typeMap[typeof(float)] = MySqlDbType.Float;
            typeMap[typeof(double)] = MySqlDbType.Double;
            typeMap[typeof(decimal)] = MySqlDbType.Decimal;
            typeMap[typeof(bool)] = MySqlDbType.Bit;
            typeMap[typeof(string)] = MySqlDbType.String;
            typeMap[typeof(Object)] = MySqlDbType.String;
            typeMap[typeof(char)] = MySqlDbType.VarChar;
            typeMap[typeof(Guid)] = MySqlDbType.Guid;
            typeMap[typeof(DateTime)] = MySqlDbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = MySqlDbType.Timestamp;
            typeMap[typeof(byte[])] = MySqlDbType.Binary;
            typeMap[typeof(byte?)] = MySqlDbType.Byte;
            typeMap[typeof(sbyte?)] = MySqlDbType.Byte;
            typeMap[typeof(short?)] = MySqlDbType.Int16;
            typeMap[typeof(ushort?)] = MySqlDbType.UInt16;
            typeMap[typeof(int?)] = MySqlDbType.Int32;
            typeMap[typeof(uint?)] = MySqlDbType.UInt32;
            typeMap[typeof(long?)] = MySqlDbType.Int64;
            typeMap[typeof(ulong?)] = MySqlDbType.UInt64;
            typeMap[typeof(float?)] = MySqlDbType.Float;
            typeMap[typeof(double?)] = MySqlDbType.Double;
            typeMap[typeof(decimal?)] = MySqlDbType.Decimal;
            typeMap[typeof(bool?)] = MySqlDbType.Bit;
            typeMap[typeof(char?)] = MySqlDbType.VarChar;
            typeMap[typeof(Guid?)] = MySqlDbType.Guid;
            typeMap[typeof(DateTime?)] = MySqlDbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = MySqlDbType.Timestamp;
            return typeMap;
        }
    }
    #region 会话

    public sealed class Session
    {
        internal MySqlConnection _Connection = null;
        internal MySqlCommand _Command = null;
        private MySqlTransaction _Transaction = null;
        internal Session(string connstring)
        {
            _Connection = new MySqlConnection(connstring);
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
