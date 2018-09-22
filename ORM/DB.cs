using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Snow.Orm
{
    /// <summary>
    /// 数据库
    /// </summary>
	public sealed partial class DB : MySqlDB
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
        //public bool IsDebug { private set; get; }
        private uint TimeOut = 0;       //数据库连接超时，单位秒，默认0表示由系统控制
        private string ReadConnStr = null;  //数据库读连接字符串
        public string WriteConnStr = null;	//数据库写连接字符串


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connstr">数据库连接字符串</param>
        /// <param name="timeout">超时</param>
        /// <param name="isdebug">是否调试</param>
		public DB(string connstr, uint timeout = 0, log4net.ILog log = null)
        {
            if (connstr == null || connstr.Length < 10)
            {
                throw new Exception("配置错误：数据库没有设置连接字符串！");
            }
            ReadConnStr = connstr;
            WriteConnStr = connstr;

            Log = log;
            //IsDebug = isdebug;
            if (timeout > 0) this.TimeOut = timeout;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="readconnstr"></param>
        /// <param name="writeconnstr"></param>
        /// <param name="timeout"></param>
		public DB(string readconnstr, string writeconnstr, uint timeout = 0, log4net.ILog log = null)
        {
            if (readconnstr == null || readconnstr.Length < 10)
            {
                throw new Exception("配置错误：数据库没有设置连接字符串！");
            }
            ReadConnStr = readconnstr;
            WriteConnStr = writeconnstr;
            Log = log;
            //IsDebug = isdebug;
            if (this.WriteConnStr == null || this.WriteConnStr.Length < 3) this.WriteConnStr = this.ReadConnStr;
            if (timeout > 0) this.TimeOut = timeout;
        }


        public const string _ParameterPrefix = "@";

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
            var cmd = this.Command(sql, this.Connection(ReadConnStr), CommandType.Text, TimeOut);
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
            using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
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
            using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
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
            using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
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
            using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
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
        public DalResult Write(Session sess, string sql, DbParameter param = null)
        {
            var result = DalResult.Factory;
            if (sql == null || sql.Length < 3) return result;

            var rows = 0;
            if (sess == null)
            {
                using (var conn = this.Connection(WriteConnStr))
                using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
                {
                    result.Success = _write(cmd, param, out rows);
                }
            }
            else
            {
                sess._Command.CommandText = sql;
                sess._Command.Parameters.Clear();
                result.Success = _write(sess._Command, param, out rows);
                if (!result.Success) sess.Rollback();
            }
            result.Rows = rows;
            return result;
        }
        bool _write(DbCommand cmd, DbParameter param, out int _num)
        {
            if (param != null) cmd.Parameters.Add(param);
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed) cmd.Connection.Open();
                _num = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Log.Debug(Debug(cmd.CommandText, param), e);
#endif
                _num = 0;
                return false;
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

            var rows = 0;
            if (sess == null)
            {
                using (var conn = this.Connection(WriteConnStr))
                using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
                {
                    result.Success = _write(cmd, parames, out rows);
                }
            }
            else
            {
                sess._Command.CommandText = sql;
                sess._Command.Parameters.Clear();
                result.Success = _write(sess._Command, parames, out rows);
                if (!result.Success) sess.Rollback();
            }
            result.Rows = rows;
            return result;
        }
        bool _write(DbCommand cmd, List<DbParameter> parames, out int _num)
        {
            try
            {
                if (parames != null)
                {
                    for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
                }
                if (cmd.Connection.State == ConnectionState.Closed) cmd.Connection.Open();
                _num = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Log.Debug(Debug(cmd.CommandText, parames), e);
#endif
                _num = 0;
                return false;
            }
        }

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
            using (var cmd = this.Command(proc, conn, CommandType.StoredProcedure,TimeOut))
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
            if (string.IsNullOrWhiteSpace(sql) || parames == null || parames.Count < 1) return result;

            sql += _InsertReturn;

            if (sess == null)
            {
                using (var conn = this.Connection(WriteConnStr))
                using (var cmd = this.Command(sql, conn, CommandType.Text, TimeOut))
                {
                    return _insert(cmd, parames);
                }
            }
            else
            {
                sess._Command.CommandText = sql;
                sess._Command.Parameters.Clear();
                result = _insert(sess._Command, parames);
                if (!result.Success) sess.Rollback();
                return result;
            }
        }
        DalResult _insert(DbCommand cmd, List<DbParameter> parames)
        {
            var result = DalResult.Factory;
            for (int i = 0; i < parames.Count; i++) { cmd.Parameters.Add(parames[i]); }
            if (cmd.Parameters.Count < 1) return result;

            var dap = DataAdapter();
            dap.SelectCommand = cmd;
            var tb = new DataTable();
            try { dap.Fill(tb); }
            catch (Exception e)
            {
#if DEBUG
                Log.Debug(Debug(cmd.CommandText, parames), e);
#endif
                tb.Dispose();
                return result;
            }
            finally
            {
                if (dap != null) dap.Dispose();
            }
            if (tb == null || tb.Rows.Count < 1 || tb.Columns.Count < 2) return result;
            result.Rows = tb.Rows[0][0].ToInt(0);
            result.Id = tb.Rows[0][1].ToInt(-1);
            tb.Dispose();
            if (result.Id > 0) { result.Success = true; return result; }
            if (result.Rows > 0) { result.Success = true; return result; }
            return result;
        }

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
        public object ReadSingle(string sqlString, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(sqlString)) { throw new Exception("数据库查询字符串不能为空"); }
            var cmd = GetRawSql(sqlString, args);
            try { return ReadSingle(cmd.SqlString, cmd.SqlParams); }
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
            if (SqlDict.TryGetValue(sqlString, out string sqlStr))
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
            using (var cmd = this.Command("", conn, CommandType.Text, TimeOut))
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
                                cmd.Parameters.Add(LastIdParameter);
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
