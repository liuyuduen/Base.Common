using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Base.Utility
{
    /// <summary>
    /// OleDb分页辅助类
    /// </summary>
    public class OleDbPageHelper
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

            return Convert.ToInt32(AccessHelper.ExecuteScalar(newSql));
        }


        /// <summary>
        /// 获得某一页的内容
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="orderBy">排序的条件(比如：order by Age Desc)</param>
        /// <param name="primaryKey">主键(比如：p.Id)</param>
        /// <param name="startRowIndex">开始行索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>分页后的数据</returns>
        public static DataTable GetPagedInfo(string sql, string orderBy, string primaryKey, int startRowIndex, int pageSize)
        {
            // 得到from所在的位置
            int fromIndex = sql.LastIndexOf(" from ", StringComparison.OrdinalIgnoreCase);

            // 得到where所在地位置
            int whereIndex = sql.LastIndexOf(" where ", StringComparison.OrdinalIgnoreCase);

            // 得到查询的字段
            string fields = sql.Substring(7, fromIndex - 7);

            // 得到from后面的语句
            string from = sql.Substring(fromIndex, whereIndex - fromIndex);

            // 得到where后面的语句
            string where = (whereIndex == -1 ? "where 1=1" : sql.Substring(whereIndex));

            // 拼凑要执行的SQL语句
            string sqlAll = "";
            if (startRowIndex == 0)
            {
                sqlAll = string.Format("select top {0} {1} {2} {3} {4}", pageSize, fields, from, where, orderBy);
            }
            else
            {
                sqlAll = string.Format("select top {0} {1} {2} {3} and {4} not in(select top {5} {6} {7} {8} {9}) {10}",
                pageSize, fields, from, where, primaryKey, startRowIndex, primaryKey, from, where, orderBy, orderBy);
            }


            return AccessHelper.ExecuteDataTable(sqlAll);

        }


    }
}
