using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using Base.Utility; 
using Base.Utility;

namespace Base.Utility
{
    /// <summary>
    /// Access数据操作通用类
    /// </summary>
    public abstract class AccessHelper
    { 
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public static readonly string DB_CONNECTION_STRING = ConfigHelper.DbConnectionString.Replace("|RootPath|", HttpHelper.CurrentServer.MapPath("/"));

        #region 执行操作，返回受影响的行数

        /// <summary>
        /// 执行操作，返回受影响的行数
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="commandParameters">执行命令需要的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 执行操作，返回受影响的行数
        /// </summary>
        /// <param name="tran">已存在的事务</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="commandParameters">执行命令需要的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(OleDbTransaction trans, string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteNonQuery(trans, CommandType.Text, cmdText, commandParameters);

        }

        /// <summary>
        /// 执行操作，返回受影响的行数(存储过程)
        /// </summary>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="commandParameters">执行命令需要的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQueryProc(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        /// <summary>
        /// 执行操作，返回受影响的行数(存储过程)
        /// </summary>
        /// <param name="tran">已存在的事务</param>
        /// <param name="cmdText">存储过程名称</param>
        /// <param name="commandParameters">执行命令需要的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQueryProc(OleDbTransaction trans, string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteNonQuery(trans, CommandType.StoredProcedure, cmdText, commandParameters);
        }

        private static int ExecuteNonQuery(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(DB_CONNECTION_STRING))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                int val = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();

                return val;
            }
        }

        private static int ExecuteNonQuery(OleDbTransaction trans, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);

            int val = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            return val;

        }

        #endregion

        #region 执行查询，返回OleDbDataReader

        /// <summary>
        /// 执行查询，返回OleDbDataReader
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>一个结果集对象OleDbDataReader</returns>
        public static OleDbDataReader ExecuteReader(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 执行查询，返回OleDbDataReader(存储过程)
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>一个结果集对象OleDbDataReader</returns>
        public static OleDbDataReader ExecuteReaderProc(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        public static OleDbDataReader ExecuteReader(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            OleDbConnection conn = new OleDbConnection(DB_CONNECTION_STRING);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                cmd.Parameters.Clear();

                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        #endregion

        #region 执行操作，返回表中第一行，第一列的值

        /// <summary>
        /// 执行操作，返回表中第一行，第一列的值
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">执行命令的参数集</param>        
        /// <returns>返回的对象</returns>
        public static object ExecuteScalar(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, cmdText, commandParameters);
        }

        /// <summary>
        /// 执行操作，返回表中第一行，第一列的值(存储过程)
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="commandParameters">执行命令的参数集</param>        
        /// <returns>返回的对象</returns>
        public static object ExecuteScalarProc(string cmdText, params OleDbParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.StoredProcedure, cmdText, commandParameters);
        }

        public static object ExecuteScalar(CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection connection = new OleDbConnection(DB_CONNECTION_STRING))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                object val = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return val;
            }
        }

        #endregion

        #region 执行一个命令，返回数据集或数据表

        /// <summary>
        /// 执行一个命令，返回数据表
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>返回数据表</returns>
        public static DataTable ExecuteDataTable(string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteDataSet(commandText, commandParameters).Tables[0];
        }

        /// <summary>
        /// 执行一个命令，返回数据表(存储过程)
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>返回数据表</returns>
        public static DataTable ExecuteDataTableProc(string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteDataSetProc(commandText, commandParameters).Tables[0];
        }

        /// <summary>
        /// 执行一个命令，返回数据集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>返回数据集</returns>
        public static DataSet ExecuteDataSet(string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行一个命令，返回数据集(存储过程)
        /// </summary>
        /// <param name="commandText">存储过程名称</param>
        /// <param name="commandParameters">执行命令的参数集</param>
        /// <returns>返回数据集</returns>
        public static DataSet ExecuteDataSetProc(string commandText, params OleDbParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public static DataSet ExecuteDataSet(CommandType cmdType, string commandText, params OleDbParameter[] commandParameters)
        {

            OleDbCommand cmd = new OleDbCommand();

            using (OleDbConnection conn = new OleDbConnection(DB_CONNECTION_STRING))
            {
                PrepareCommand(cmd, conn, null, cmdType, commandText, commandParameters);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                cmd.Parameters.Clear();
                return ds;
            }
        }

        #endregion

        /// <summary>
        /// 设置OleDbCommand对象
        /// </summary>
        /// <param name="cmd">OleDbCommand对象</param>
        /// <param name="conn">OleDbConnection 对象</param>
        /// <param name="trans">OleDbTransaction 对象</param>
        /// <param name="cmdType">CommandType(执行存储过程或SQL语句)</param>
        /// <param name="cmdText">存储过程名称或SQL语句</param>
        /// <param name="cmdParms">命令中用到的参数集</param>
        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
