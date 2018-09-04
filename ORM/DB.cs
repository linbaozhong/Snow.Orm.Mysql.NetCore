using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using adeway;

namespace Snow.Orm
{
    /// <summary>
    /// 数据库
    /// </summary>
	public sealed partial class DB
    {
        public class SqlCommand
        {
            public string SqlString { set; get; }
            public List<DbParameter> SqlParams = new List<DbParameter>();
        }

        public log4net.ILog Log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sql"></param>
        /// <param name="parames"></param>
		public static Exception Error(Exception e, string sql, List<DbParameter> parames = null)
        {
            return new Exception(Debug(sql, parames), e);
        }
        public static string Debug(string sql, DbParameter parame = null)
        {
            StringBuilder mess = new StringBuilder(300);
            if (sql != null) mess.AppendLine(sql);
            string s = null;
            if (parame != null)
            {
                s = parame.Value.ToString();
                mess.Append(parame.ParameterName);
                mess.Append(": ");
                if (s != null && s.Length > 3000) mess.AppendLine(s.Substring(0, 3000));
                else mess.AppendLine(s);
            }
            return mess.ToString();
        }
        public static string Debug(string sql, List<DbParameter> parames = null)
        {
            StringBuilder mess = new StringBuilder(300);
            if (sql != null) mess.AppendLine(sql);
            string s = null;
            if (parames != null)
            {
                for (int i = 0; i < parames.Count; i++)
                {
                    var p = parames[i];
                    if (p == null || p.Value == null) continue;
                    s = p.Value.ToString();
                    mess.Append(p.ParameterName);
                    mess.Append(": ");
                    if (s != null && s.Length > 3000) mess.AppendLine(s.Substring(0, 3000));
                    else mess.AppendLine(s);
                }

            }
            return mess.ToString();
        }
        public bool IsDebug { private set; get; }
        private uint TimeOut = 0;       //数据库连接超时，单位秒，默认0表示由系统控制
        private string ReadConnStr = null;  //数据库读连接字符串
        public string WriteConnStr = null;	//数据库写连接字符串

        public static Dictionary<Type, MySqlDbType> MySqlDbTypeMap = GetTypeMap();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connstr">数据库连接字符串</param>
        /// <param name="timeout">超时</param>
        /// <param name="isdebug">是否调试</param>
		public DB(string connstr, uint timeout = 0, log4net.ILog log = null, bool isdebug = false)
        {
            if (connstr == null || connstr.Length < 10)
            {
                throw new Exception("配置错误：数据库没有设置连接字符串！");
            }
            ReadConnStr = connstr;
            WriteConnStr = connstr;

            Log = log;
            IsDebug = isdebug;
            if (timeout > 0) this.TimeOut = timeout;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readconnstr"></param>
        /// <param name="writeconnstr"></param>
        /// <param name="timeout"></param>
		public DB(string readconnstr, string writeconnstr, uint timeout = 0, log4net.ILog log = null, bool isdebug = false)
        {
            if (readconnstr == null || readconnstr.Length < 10)
            {
                throw new Exception("配置错误：数据库没有设置连接字符串！");
            }
            ReadConnStr = readconnstr;
            WriteConnStr = writeconnstr;
            Log = log;
            IsDebug = isdebug;
            if (this.WriteConnStr == null || this.WriteConnStr.Length < 3) this.WriteConnStr = this.ReadConnStr;
            if (timeout > 0) this.TimeOut = timeout;
        }

        private MySqlCommand Command(string sql, MySqlConnection conn, CommandType type = CommandType.Text)
        {
            var cmd = new MySqlCommand(sql, conn);
            cmd.CommandType = type;
            if (TimeOut > 0) cmd.CommandTimeout = (int)TimeOut;
            return cmd;
        }
        private MySqlConnection Connection(string connectionString) { return new MySqlConnection(connectionString); }
        private MySqlDataAdapter DataAdapter() { return new MySqlDataAdapter(); }

        public const string _ParameterPrefix = "@";
        public const string _RestrictPrefix = "`";
        public const string _RestrictPostfix = "`";

        public static string GetCondition(string col, string op = "=")
        {
            return string.Concat(_RestrictPrefix, col, _RestrictPostfix, op, _ParameterPrefix, col);
        }

        public static string SetColumnFunc<T>(string col, T val)
        {
            return string.Concat(_RestrictPrefix, col, _RestrictPostfix, "=", val);
        }
        /// <summary>
        /// 为列名附加约束符号(``[])
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string GetName(string col)
        {
            return string.Concat(_RestrictPrefix, col, _RestrictPostfix);
        }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
		public static List<DbParameter> GetParames(IEnumerable<KeyValuePair<string, string>> cols)
        {
            if (cols == null) return null;
            List<DbParameter> parames = new List<DbParameter>(10); DbParameter p;
            foreach (KeyValuePair<string, string> kv in cols)
            {
                if ((p = GetParam(kv.Key, kv.Value)) != null) parames.Add(p);
            }
            return parames;
        }

        public DbDataReader Read(string sql, List<DbParameter> parames)
        {
            if (sql == null || sql.Length < 3) return null;
            var cmd = this.Command(sql, this.Connection(ReadConnStr));
            DbDataReader dr = null;
            try
            {
                if (parames != null)
                {
                    for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                }
                cmd.Connection.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (Exception e)
            {
                if (dr != null && !dr.IsClosed) dr.Close(); cmd.Dispose();
#if DEBUG
                Log.Debug(Debug(sql, parames));
#endif
                throw e;
            }
        }
        public object ReadSingle(string sql, List<DbParameter> parames)
        {
            if (sql == null || sql.Length < 3) return null;

            using (var conn = this.Connection(ReadConnStr))
            using (var cmd = this.Command(sql, conn))
            {
                try
                {
                    if (parames != null)
                    {
                        for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                    }
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
#if DEBUG
                    Log.Debug(Debug(sql, parames));
#endif
                    throw e;
                }
            }
        }
        public object ReadSingle(string sql, DbParameter param = null)
        {
            if (sql == null || sql.Length < 3) return null;

            using (var conn = this.Connection(ReadConnStr))
            using (var cmd = this.Command(sql, conn))
            {
                if (param != null)
                {
                    cmd.Parameters.Add(param);
                }

                try
                {
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
#if DEBUG
                    Log.Debug(Debug(sql, param));
#endif
                    throw e;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataTable Query(string sql, DbParameter param = null)
        {
            if (sql == null || sql.Length < 3) return null;

            using (var conn = this.Connection(ReadConnStr))
            using (var cmd = this.Command(sql, conn))
            {
                if (param != null) cmd.Parameters.Add(param);
                DbDataAdapter dap = DataAdapter();
                dap.SelectCommand = cmd;

                DataTable tb = new DataTable();
                try
                {
                    dap.Fill(tb);
                    if (tb.Rows.Count > 0) return tb;
                    tb.Dispose();
                    return null;
                }
                catch
                {
                    tb.Dispose();
#if DEBUG
                    Log.Debug(Debug(sql, param));
#endif
                    throw;
                }
                finally
                {
                    if (dap != null) dap.Dispose();
                }
            }
        }
        public DataTable Query(string sql, List<DbParameter> parames)
        {
            if (sql == null || sql.Length < 3) return null;

            using (var conn = this.Connection(ReadConnStr))
            using (var cmd = this.Command(sql, conn))
            {
                DbDataAdapter dap = DataAdapter();
                dap.SelectCommand = cmd;

                DataTable tb = new DataTable();
                try
                {
                    if (parames != null)
                    {
                        for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                    }
                    dap.Fill(tb);
                    if (tb.Rows.Count > 0) return tb;
                    tb.Dispose();
                    return null;
                }
                catch
                {
                    tb.Dispose();
#if DEBUG
                    Log.Debug(Debug(sql, parames));
#endif
                    throw;
                }
                finally
                {
                    if (dap != null) dap.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DalResult Write(string sql, DbParameter param = null)
        {
            var result = DalResult.Factory;
            if (sql == null || sql.Length < 3) return result;
            using (var conn = this.Connection(WriteConnStr))
            using (var cmd = this.Command(sql, conn))
            {
                if (param != null) cmd.Parameters.Add(param);
                try
                {
                    cmd.Connection.Open();
                    result.Rows = cmd.ExecuteNonQuery();
                    result.Success = true;
                    return result;
                }
                catch
                {
#if DEBUG
                    Log.Debug(Debug(sql, param));
#endif
                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parames"></param>
        /// <param name="rows">返回受影响的行数</param>
        /// <returns></returns>
        public DalResult Write(Session sess, string sql, List<DbParameter> parames)
        {
            var result = DalResult.Factory;
            if (sql == null || sql.Length < 3) return result;

            FuncEx<MySqlCommand, List<DbParameter>, int, bool> func = (MySqlCommand cmd, List<DbParameter> _parames, out int _num) =>
            {
                try
                {
                    if (_parames != null)
                    {
                        for (int i = 0; i < _parames.Count; i++) { cmd.Parameters.Add(_parames[i]); }
                    }
                    if (cmd.Connection.State == ConnectionState.Closed) cmd.Connection.Open();
                    _num = cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
#if DEBUG
                    Log.Debug(Debug(sql, _parames));
#endif
                    _num = 0;
                    return false;
                }
            };
            var rows = 0;
            if (sess == null)
            {
                using (var conn = this.Connection(WriteConnStr))
                using (var cmd = this.Command(sql, conn))
                {
                    result.Success = func(cmd, parames, out rows);
                }
            }
            else
            {
                sess._Command.CommandText = sql;
                result.Success = func(sess._Command, parames, out rows);
                if (!result.Success) sess.Rollback();
            }
            result.Rows = rows;
            return result;
        }
        delegate TResult FuncEx<in T1, in T2, T3, out TResult>(T1 t1, T2 t2, out T3 t3);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="parames"></param>
        /// <param name="returnVal"></param>
        /// <returns></returns>
        public bool Procedure(string proc, List<DbParameter> parames, out int returnVal)
        {
            returnVal = 0;

            if (string.IsNullOrWhiteSpace(proc))
            {
                return false;
            }
            using (var conn = this.Connection(WriteConnStr))
            using (var cmd = this.Command(proc, conn, CommandType.StoredProcedure))
            {
                try
                {
                    if (parames != null)
                    {
                        for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                    }
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    returnVal = (int)cmd.Parameters["p_return"].Value;
                    return returnVal == 0;
                }
                catch
                {
#if DEBUG
                    Log.Debug(Debug(proc, parames));
#endif
                    throw;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parames"></param>
        /// <returns></returns>
        internal DalResult Insert(Session sess, string sql, List<DbParameter> parames)
        {
            var result = DalResult.Factory;
            if (sql == null || parames == null || sql.Length < 3 || parames.Count < 1) return result;

            Func<MySqlCommand, DalResult> func = (MySqlCommand cmd) =>
            {
                for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                if (cmd.Parameters.Count < 1) return result;

                var dap = this.DataAdapter();
                dap.SelectCommand = cmd;
                var tb = new DataTable();
                try { dap.Fill(tb); }
                catch (Exception e)
                {
#if DEBUG
                    Log.Debug(Debug(sql, parames), e);
#endif
                    tb.Dispose();
                    return result;
                }
                finally
                {
                    if (cmd != null)
                    {
                        if (cmd.Connection != null) cmd.Connection.Close();
                        cmd.Dispose();
                    }
                    if (dap != null) dap.Dispose();
                }
                if (tb == null || tb.Rows.Count < 1 || tb.Columns.Count < 2) return result;
                result.Rows = tb.Rows[0][0].ToInt(0);
                result.Id = tb.Rows[0][1].ToInt(-1);
                tb.Dispose();
                if (result.Id > 0) { result.Success = true; return result; }
                if (result.Rows > 0) { result.Success = true; return result; }
                return result;

            };
            if (sess == null)
            {
                using (var conn = this.Connection(WriteConnStr))
                using (var cmd = this.Command(sql, conn))
                {
                    return func(cmd);
                }
            }
            else
            {
                sess._Command.CommandText = sql;
                result = func(sess._Command);
                if (!result.Success) sess.Rollback();
                return result;
            }
        }
        delegate TResult FuncEx<in T1, T2, out TResult>(T1 t1, out T2 t2);

        #region 原生SQL
        static ConcurrentDictionary<string, string> SqlDict = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 执行原生数据库操作命令
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DalResult Exec(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString)) { throw new Exception("数据库操作命令不能为空"); }
            var cmd = GetRawSql(sqlString, args);
            try { return Write(null, cmd.SqlString, cmd.SqlParams); }
            catch (Exception) { throw; }
            finally
            {
            }
        }
        /// <summary>
        /// 执行原生数据库查询,并返回DataTable
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public DataTable Query(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString)) { throw new Exception("数据库查询字符串不能为空"); }
            var cmd = GetRawSql(sqlString, args);
            try { return Query(cmd.SqlString, cmd.SqlParams); }
            catch (Exception) { throw; }
            finally
            {
            }
        }
        /// <summary>
        /// 原生sql命令
        /// 例如：例如：select * from users where age>=? and sex=?
        /// </summary>
        /// <param name="sqlString">例如：select * from users where age>=? and sex=?</param>
        /// <param name="dbParams">返回的 List<DbParameter></param>
        /// <param name="args">查询条件值,和sql字符串中的？号对应</param>
        /// <returns>sql字符串</returns>
        public static SqlCommand GetRawSql(string sqlString, params object[] args)
        {
            SqlCommand cmd = new SqlCommand();
            var len = args.Length;
            if (len == 0)
            {
                cmd.SqlString = sqlString;
                return cmd;
            }

            var i = 0;
            string sqlStr = null;
            if (SqlDict.TryGetValue(sqlString, out sqlStr))
            {
                cmd.SqlString = sqlStr;
                if (len > 0)
                {
                    foreach (var arg in args)
                    {
                        cmd.SqlParams.Add(DB.GetParam("_cols_" + i, args[i]));
                        i++;
                    }
                }
                return cmd;
            }

            var sql = new StringBuilder(200);
            string col = string.Empty;
            foreach (var c in sqlString)
            {
                if (c == '?' && i < len)
                {
                    col = "_cols_" + i;
                    sql.Append(DB._ParameterPrefix + col);
                    cmd.SqlParams.Add(DB.GetParam(col, args[i]));
                    i++;
                    continue;
                }
                sql.Append(c);
            }
            cmd.SqlString = sql.ToString(); ;
            SqlDict.AddOrUpdate(sqlString, cmd.SqlString, (x, y) => cmd.SqlString);

            return cmd;
        }

        static DbParameter LastIdParameter = GetParam("_LAST_INSERT_ID_", 0);
        /// <summary>
        /// 事务中多条命令共用的主键参数
        /// </summary>
        public static string ReturnID = _ParameterPrefix + LastIdParameter.ParameterName;
        /// <summary>
        /// 生成事务中的原生insert命令,并返回自增id,供后面的子命令使用
        /// 子命令例子:insert user (id,name) values(DB.ReturnID,?)
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static SqlCommand GetInsertAndReturnIDRawSql(string sqlString, params object[] args)
        {
            var cmd = GetRawSql(sqlString, args);
            if (!cmd.SqlString.TrimEnd().EndsWith(";")) cmd.SqlString += ";";
            cmd.SqlString += "select ROW_COUNT(),LAST_INSERT_ID();";
            cmd.SqlParams.Add(LastIdParameter);
            return cmd;
        }
        #endregion

        #region 事务
        public bool ExecTrans(List<SqlCommand> sqls)
        {
            if (sqls == null) return false;
            using (var conn = this.Connection(WriteConnStr))
            using (var cmd = this.Command("", conn))
            {
                cmd.Connection.Open();
                cmd.Transaction = cmd.Connection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    var last_id = 0L;
                    foreach (var sql in sqls)
                    {
                        if (string.IsNullOrWhiteSpace(sql.SqlString))
                        {
                            cmd.Transaction.Rollback();
                            return false;
                        }
                        cmd.CommandText = sql.SqlString;
                        cmd.Parameters.Clear();
                        if (sql.SqlParams != null)
                            for (int i = 0; i < sql.SqlParams.Count; i++) { cmd.Parameters.Add(sql.SqlParams[i]); }

                        if (sql.SqlParams.Contains(LastIdParameter))
                        {
                            var dr = cmd.ExecuteReader();
                            if (dr.Read())
                            {
                                if (dr[0].ToInt() == 0) { dr.Close(); cmd.Transaction.Rollback(); return false; }

                                last_id = dr[1].ToLong(-1);
                            }
                            dr.Close();
                        }
                        else
                        {
                            if (last_id > 0L)
                            {
                                LastIdParameter.Value = last_id;
                                cmd.Parameters.Add(LastIdParameter as MySqlParameter);
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.Transaction.Commit();
                    return true;
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }
        }
        #endregion
    }
}
