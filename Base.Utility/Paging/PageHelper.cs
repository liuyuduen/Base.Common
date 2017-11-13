using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utility
{
    public class PageHelper
    {
        public static string ToStrSql(string sql, ref PageParam pageparam)
        {
             
            StringBuilder strSql = new StringBuilder();
            string orderField = pageparam.sidx;
            string orderType = pageparam.sord;
            int pageIndex = pageparam.page;
            int pageSize = pageparam.rows;
            int totalRow = pageparam.records;
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            int num = (pageIndex - 1) * pageSize;
            int num1 = (pageIndex) * pageSize;
            string OrderBy = "";
            if (!string.IsNullOrEmpty(orderField))
                OrderBy = "Order By " + orderField + " " + orderType + "";
            else
                OrderBy = "order by (select 0)";
            strSql.Append("Select * From (Select ROW_NUMBER() Over (" + OrderBy + ")");
            strSql.Append(" As rowNum, * From (" + sql + ") As T ) As N Where rowNum > " + num + " And rowNum <= " + num1 + "");
            totalRow = Convert.ToInt32(SqlHelper.ExecuteScalar("Select Count(1) From (" + sql + ") As t"));
            pageparam.records = totalRow;
            return strSql.ToString();

        }

    }
}
