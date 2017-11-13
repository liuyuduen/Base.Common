using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Base.Utility;

namespace Base.Utility
{
    /// <summary>
    /// SQL分页辅助类
    /// </summary>
    public class SqlPageHelper
    {
        /// <summary>
        /// 根据SQL语句获得相应的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>记录数</returns>
        public static int GetRecordCount(string sql)
        {
            // 得到from所在的位置
            int fromIndex = sql.LastIndexOf(" from ", StringComparison.OrdinalIgnoreCase);

            // 得到from后面的语句
            string from = sql.Substring(fromIndex + 1);

            string newSql = "select count(*) " + from;

            return Convert.ToInt32(SqlHelper.ExecuteScalar(newSql));
        }

        /// <summary>
        /// 获得某一页的内容(不返回记录数)
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        /// <param name="primaryKey">主键(比如：p.Id，只在SQLServer2000时才产生作用)</param>
        /// <param name="fromLocation">主from的位置数(从1开始)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页后的数据</returns>
        public static DataTable GetPagedInfo(string sql, string orderBy, string primaryKey,int fromLocation, int startRowIndex, int pageSize)
        {
            int rc = 0;
            DataTable dt = null;

            switch (ConfigHelper.SQLDbType)
            {
                case SQLDbType.SQL2000:
                    dt = GetPagedInfoBySQL2000(sql, orderBy, primaryKey,fromLocation, startRowIndex, pageSize, out rc, false);
                    break;
                case SQLDbType.SQL2005:
                    dt = GetPagedInfoBySQL2005(sql, orderBy,fromLocation, startRowIndex, pageSize ,out rc, false);
                    break;
            }
            return dt;
        }


        /// <summary>
        /// 获得某一页的内容(不返回记录数)
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        ///  <param name="primaryKey">主键(比如：p.Id，只在SQLServer2000时才产生作用)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页后的数据</returns>
        public static DataTable GetPagedInfo(string sql, string orderBy, string primaryKey, int startRowIndex, int pageSize)
        {
            int rc = 0;
            DataTable dt = null;

            switch (ConfigHelper.SQLDbType)
            {
                case SQLDbType.SQL2000:
                    dt= GetPagedInfoBySQL2000(sql, orderBy,primaryKey, 0,startRowIndex, pageSize, out rc, false);
                    break;
                case SQLDbType.SQL2005:
                    dt= GetPagedInfoBySQL2005(sql, orderBy,0,startRowIndex, pageSize, out rc, false);
                    break;
            }
            return dt;
        }

        /// <summary>
        /// 获得某一页的内容(返回记录数)
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        /// <param name="primaryKey">主键(比如：p.Id，只在SQLServer2000时才产生作用)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="recordCount">记录数(返回值)</param>
        /// <returns>分页后的数据</returns>
        public static DataTable GetPagedInfo(string sql, string orderBy,string primaryKey, int startRowIndex, int pageSize, out int recordCount)
        {
            DataTable dt = null;
            switch (ConfigHelper.SQLDbType)
            {
                case SQLDbType.SQL2000:
                    dt= GetPagedInfoBySQL2000(sql, orderBy, primaryKey,0, startRowIndex, pageSize, out recordCount, true);
                    break;
                case SQLDbType.SQL2005:
                    dt= GetPagedInfoBySQL2005(sql, orderBy,0, startRowIndex, pageSize, out recordCount, true);
                    break;
                default:
                    recordCount = 0;
                    break;
            }
            return dt;
        }

        #region 获得某一页的内容(SQL2000)

        /// <summary>
        /// 获得某一页的内容(SQL2000)
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        /// <param name="primaryKey">主键(比如：p.Id)</param>
        /// <param name="fromLocation">主from的位置数(从1开始)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="recordCount">记录总数(如果不返回记录数，则为0；如果返回记录数，则为实际的记录数)</param>
        /// <param name="isReutrnRecordCount">是否返回记录数</param>
        /// <returns>分页后的数据</returns>
        private static DataTable GetPagedInfoBySQL2000(string sql, string orderBy, string primaryKey, int fromLocation , int startRowIndex, int pageSize, out int recordCount, bool isReutrnRecordCount)
        {
            // 得到from所在的位置
            int fromIndex = 0;
            if (fromLocation == 0)
                fromIndex = sql.LastIndexOf(" from ", StringComparison.OrdinalIgnoreCase);
            else
            {
                int lastStart = -1;
                int fromCount = 0;

                while ( (lastStart = sql.IndexOf(" from ", lastStart+1, StringComparison.OrdinalIgnoreCase)) > -1)
                {
                    fromCount++;
                    if (fromCount == fromLocation)
                    {
                        fromIndex = lastStart;
                        break;
                    }
                }

                if (lastStart == -1)
                {
                    throw new ApplicationException("在指定位置没有找到from关键字");
                }
            }

            // 得到where所在地位置
            int whereIndex = sql.LastIndexOf(" where ", StringComparison.OrdinalIgnoreCase);

            // 得到查询的字段
            string fields = sql.Substring(7, fromIndex - 7);

            // 得到from后面的语句
            string from = sql.Substring(fromIndex, whereIndex - fromIndex);

            // 得到where后面的语句
            string where = (whereIndex == -1 ? "where 1=1" : sql.Substring(whereIndex));

            // 拼凑要执行的SQL语句
            string sqlAll = string.Format("select top {0} {1} {2} {3} and {4} not in(select top {5} {6} {7} {8} {9}) {10}",
                pageSize, fields, from, where, primaryKey, startRowIndex, primaryKey, from, where, orderBy, orderBy);

            if (isReutrnRecordCount)
            {
                sqlAll += ";select count(*) " + from + " " + where;
            }

            DataSet ds = SqlHelper.ExecuteDataSet(sqlAll);

            if (isReutrnRecordCount)
            {
                // 设置输出记录数
                recordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            }
            else
            {
                recordCount = 0;
            }

            return ds.Tables[0];
        }

        #endregion

        #region 获得某一页的内容(SQL2005)

        /// <summary>
        /// 获得某一页的内容(SQL2005)
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        /// <param name="fromLocation">主from的位置数(从1开始)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="recordCount">记录总数(如果不返回记录数，则为0；如果返回记录数，则为实际的记录数)</param>
        /// <param name="isReutrnRecordCount">是否返回记录数</param>
        /// <returns>分页后的数据</returns>
        private static DataTable GetPagedInfoBySQL2005(string sql, string orderBy,int fromLocation, int startRowIndex, int pageSize,  out int recordCount, bool isReutrnRecordCount)
        {
            // 得到from所在的位置
            int fromIndex = 0;
            if (fromLocation == 0)
                fromIndex = sql.LastIndexOf(" from ", StringComparison.OrdinalIgnoreCase);
            else
            {
                int lastStart = -1;
                int fromCount = 0;

                while ((lastStart = sql.IndexOf(" from ", lastStart + 1, StringComparison.OrdinalIgnoreCase)) > -1)
                {
                    fromCount++;
                    if (fromCount == fromLocation)
                    {
                        fromIndex = lastStart;
                        break;
                    }
                }

                if (lastStart == -1)
                {
                    throw new ApplicationException("在指定位置没有找到from关键字");
                }
            }

            // 得到查询的字段
            string fields = sql.Substring(7, fromIndex - 7);

            // 得到from后面的语句
            string from = sql.Substring(fromIndex);

            // 拼凑要执行的SQL语句(适用于2005)
            string sqlAll = string.Format("select * from (select {0},row_number() over({1}) as Row {2}) as Temp where Row between {3} and {4}",
                fields, orderBy, from, (startRowIndex + 1), (startRowIndex + pageSize));

            if (isReutrnRecordCount)
            {
                sqlAll += ";select count(*) " + from;
            }

            DataSet ds = SqlHelper.ExecuteDataSet(sqlAll);

            if (isReutrnRecordCount)
            {
                // 设置输出记录数
                recordCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
            }
            else
            {
                recordCount = 0;
            }

            return ds.Tables[0];
        }

        #endregion
    }
}
