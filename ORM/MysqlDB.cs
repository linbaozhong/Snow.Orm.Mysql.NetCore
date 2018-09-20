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
        /// <summary>
        /// 列名约束符-前缀
        /// </summary>
        public const string _RestrictPrefix = "`";
        /// <summary>
        /// 列名约束符-后缀
        /// </summary>
        public const string _RestrictPostfix = "`";
        /// <summary>
        /// insert成功后返回受影响的行数和自增id
        /// </summary>
        protected const string _InsertReturn = "select ROW_COUNT(),LAST_INSERT_ID();";
        public static DbParameter GetParam<T>(string col, T val, int maxsize = 0)
        {
            if (string.IsNullOrWhiteSpace(col)) return null;
            MySqlParameter sp = new MySqlParameter
            {
                ParameterName = col,
                MySqlDbType = MySqlDbTypeMap[typeof(T)]
            };
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
        /// 生成查询操作参数
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
        public static DbParameter GetParam(string col, string val = null, int maxsize = 0)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter
            {
                ParameterName = col
            };
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
        /// 生成查询操作参数
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
		public static DbParameter GetParam(string col, byte[] val, int maxsize = 0)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter
            {
                ParameterName = col
            };
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
        /// 生成查询操作参数
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
		public static DbParameter GetParam(string col, int val)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter(col, MySqlDbType.Int32)
            {
                Value = val
            };
            return sp;
        }
        /// <summary>
        /// 生成查询操作参数
        /// </summary>
        /// <param name="col"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DbParameter GetParam(string col, decimal val)
        {
            if (col == null || col.Length < 1) return null;
            MySqlParameter sp = new MySqlParameter(col, MySqlDbType.Decimal)
            {
                Value = val
            };
            return sp;
        }
        protected MySqlConnection Connection(string connectionString) { return new MySqlConnection(connectionString); }
        protected DbCommand Command(string sql, MySqlConnection conn, CommandType type = CommandType.Text, uint timeout = 0)
        {
            var cmd = new MySqlCommand(sql, conn)
            {
                CommandType = type
            };
            if (timeout > 0) cmd.CommandTimeout = (int)timeout;
            return cmd;
        }
        protected DbDataAdapter DataAdapter() { return new MySqlDataAdapter(); }
        private static readonly Dictionary<Type, MySqlDbType> MySqlDbTypeMap = GetTypeMap();
        private static Dictionary<Type, MySqlDbType> GetTypeMap()
        {
            var typeMap = new Dictionary<Type, MySqlDbType>
            {
                [typeof(byte)] = MySqlDbType.Byte,
                [typeof(sbyte)] = MySqlDbType.Byte,
                [typeof(short)] = MySqlDbType.Int16,
                [typeof(ushort)] = MySqlDbType.UInt16,
                [typeof(int)] = MySqlDbType.Int32,
                [typeof(uint)] = MySqlDbType.UInt32,
                [typeof(long)] = MySqlDbType.Int64,
                [typeof(ulong)] = MySqlDbType.UInt64,
                [typeof(float)] = MySqlDbType.Float,
                [typeof(double)] = MySqlDbType.Double,
                [typeof(decimal)] = MySqlDbType.Decimal,
                [typeof(bool)] = MySqlDbType.Bit,
                [typeof(string)] = MySqlDbType.String,
                [typeof(Object)] = MySqlDbType.String,
                [typeof(char)] = MySqlDbType.VarChar,
                [typeof(Guid)] = MySqlDbType.Guid,
                [typeof(DateTime)] = MySqlDbType.DateTime,
                [typeof(DateTimeOffset)] = MySqlDbType.Timestamp,
                [typeof(byte[])] = MySqlDbType.Binary,
                [typeof(byte?)] = MySqlDbType.Byte,
                [typeof(sbyte?)] = MySqlDbType.Byte,
                [typeof(short?)] = MySqlDbType.Int16,
                [typeof(ushort?)] = MySqlDbType.UInt16,
                [typeof(int?)] = MySqlDbType.Int32,
                [typeof(uint?)] = MySqlDbType.UInt32,
                [typeof(long?)] = MySqlDbType.Int64,
                [typeof(ulong?)] = MySqlDbType.UInt64,
                [typeof(float?)] = MySqlDbType.Float,
                [typeof(double?)] = MySqlDbType.Double,
                [typeof(decimal?)] = MySqlDbType.Decimal,
                [typeof(bool?)] = MySqlDbType.Bit,
                [typeof(char?)] = MySqlDbType.VarChar,
                [typeof(Guid?)] = MySqlDbType.Guid,
                [typeof(DateTime?)] = MySqlDbType.DateTime,
                [typeof(DateTimeOffset?)] = MySqlDbType.Timestamp
            };
            return typeMap;
        }
    }
    #region 会话
    /// <summary>
    /// 事物会话
    /// </summary>
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
