using Base.Utility;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Base.Utility
{
    public class ExcelHelper
    {
        /// <summary>
        /// 存放临时上传excel文件的文件夹位置
        /// </summary>
        private const string fileDicPath = "~/";
        /// <summary>
        /// 导出Excel(导出的内容的列名和dataTable的列名相同)
        /// </summary>
        /// <param name="dt">要导出的dataTable</param>
        /// <param name="excelName">生成的 Excel 的名称，不带后缀名(例如：测试)</param>
        public static void ExportExcel(System.Data.DataTable dt, string excelName)
        {
            try
            {
                int maxRow = dt.Rows.Count;
                string fileName = excelName + "Role.xls";// + DateTime.Now.ToString().Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "_") + ".xls"; //设置导出文件的名称,能用中文
                string fileURL = string.Empty;
                fileURL = ConvertToExcel(dt, fileName);
                //获取路径后从服务器下载文件至本地
                HttpContext curContext = System.Web.HttpContext.Current;
                curContext.Response.ContentType = "application/vnd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.Default;
                curContext.Response.AppendHeader("Content-Disposition", ("attachment;filename="
                    + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+", "%20")));//可以用中文名称
                curContext.Response.Charset = "";
                curContext.Response.WriteFile(fileURL);
                curContext.Response.Flush();
                File.Delete(fileURL);
                curContext.Response.End();
            }
            catch (Exception ex)
            {
                Log.Error("导出 excel 出错", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 生成 Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="saveFileName">excel 文件名字，带后缀名(例如：测试20120216.xls)</param>
        /// <returns>文件的绝对路径</returns>
        private static string ConvertToExcel(System.Data.DataTable table, string saveFileName)
        {
            string absFileName = "";
            //ExcelApp xlApp = new ExcelApp();

            Application xlApp = new Application();

            if (xlApp == null)
            {
                return "";
            }
            Workbooks workbooks = xlApp.Workbooks;
            Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];//取得sheet1
            try
            {


                long rows = table.Rows.Count;

                /*下边注释的两行代码当数据行数超过行时，出现异常：异常来自HRESULT:0x800A03EC。因为：Excel 2003每个sheet只支持最大行数据

                //Range fchR = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[table.Rows.Count+2, gridview.Columns.View.VisibleColumns.Count+1]);

                //fchR.Value2 = datas;*/

                if (rows > 65535)
                {

                    long pageRows = 60000;//定义每页显示的行数,行数必须小于

                    int scount = (int)(rows / pageRows);

                    if (scount * pageRows < table.Rows.Count)//当总行数不被pageRows整除时，经过四舍五入可能页数不准
                    {
                        scount = scount + 1;
                    }

                    for (int sc = 1; sc <= scount; sc++)
                    {
                        if (sc > 1)
                        {

                            object missing = System.Reflection.Missing.Value;

                            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add(

                           missing, missing, missing, missing);//添加一个sheet

                        }

                        else
                        {
                            worksheet = (Worksheet)workbook.Worksheets[sc];//取得sheet1
                        }

                        string[,] datas = new string[pageRows + 1, table.Columns.Count + 1];


                        for (int i = 0; i < table.Columns.Count; i++) //写入字段
                        {
                            datas[0, i] = table.Columns[i].Caption;
                        }

                        Range range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[1, table.Columns.Count]);
                        range.Interior.ColorIndex = 15;//15代表灰色
                        range.Font.Bold = true;
                        range.Font.Size = 9;


                        int init = int.Parse(((sc - 1) * pageRows).ToString().Trim());
                        int r = 0;
                        int index = 0;
                        int result;

                        if (pageRows * sc >= table.Rows.Count)
                        {
                            result = table.Rows.Count;
                        }
                        else
                        {
                            result = int.Parse((pageRows * sc).ToString().Trim());
                        }
                        for (r = init; r < result; r++)
                        {
                            index = index + 1;
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                //if (table.Columns[i].DataType == typeof(string) || table.Columns[i].DataType == typeof(Decimal) || table.Columns[i].DataType == typeof(DateTime))
                                //{
                                object obj = table.Rows[r][table.Columns[i].ColumnName];
                                datas[index, i] = obj == null ? "" : "'" + obj.ToString().Trim();//在obj.ToString().Trim()前加单引号是为了防止自动转化格式

                                //}

                            }
                        }

                        Range fchR = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[index + 2, table.Columns.Count + 1]);

                        fchR.Value2 = datas;
                        worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。

                        range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[index + 1, table.Columns.Count]);

                        //15代表灰色

                        range.Font.Size = 9;
                        range.RowHeight = 14.25;
                        range.Borders.LineStyle = 1;
                        range.HorizontalAlignment = 1;

                    }

                }

                else
                {

                    string[,] datas = new string[table.Rows.Count + 2, table.Columns.Count + 1];
                    for (int i = 0; i < table.Columns.Count; i++) //写入字段         
                    {
                        datas[0, i] = table.Columns[i].Caption;
                    }

                    //Range range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[1, table.Columns.Count]);
                    //range.Interior.ColorIndex = 15;//15代表灰色
                    //range.Font.Bold = true;
                    //range.Font.Size = 9;


                    int r = 0;
                    for (; r < table.Rows.Count; r++)
                    {
                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            //if (table.Columns[i].DataType == typeof(string) || table.Columns[i].DataType == typeof(Decimal) || table.Columns[i].DataType == typeof(DateTime))
                            //{
                            object obj = table.Rows[r][table.Columns[i].ColumnName];
                            datas[r + 1, i] = obj == null ? "" : "'" + obj.ToString().Trim();//在obj.ToString().Trim()前加单引号是为了防止自动转化格式

                            //}

                        }

                        //System.Windows.Forms.Application.DoEvents();


                    }

                    //Range fchR = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[table.Rows.Count + 2, table.Columns.Count + 1]);

                    //fchR.Value2 = datas;

                    //worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。

                    //range = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[table.Rows.Count + 1, table.Columns.Count]);

                    ////15代表灰色

                    //range.Font.Size = 9;
                    //range.RowHeight = 14.25;
                    //range.Borders.LineStyle = 1;
                    //range.HorizontalAlignment = 1;
                }

                try
                {
                    absFileName = HttpContext.Current.Server.MapPath(fileDicPath) + "Role.xls";
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(fileDicPath) + "Role.xls"))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(fileDicPath + "Role.xls"));
                    workbook.Saved = true;
                    workbook.SaveCopyAs(absFileName);
                }
                catch (Exception ex)
                {
                    Log.Error("导出 excel 出错", ex);
                    throw ex;
                }


            }
            catch (Exception ex)
            {
                Log.Error("导出 excel 出错", ex);
                throw ex;
            }
            finally
            {

                workbook.Close(false, null, null);
                workbooks.Close();
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);

                GC.Collect();//强行销毁 
            }
            return absFileName;

        }


        /// <summary>
        /// 生成 Excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="saveFileName">excel 文件名字，带后缀名(例如：测试20120216.xls)</param>
        /// 数字乱码 前面加单引号 '
        /// <returns>文件的绝对路径</returns>
        public static bool DataSetToExcel(System.Data.DataTable dataTable, bool isShowExcle)
        {
            int rowNumber = dataTable.Rows.Count;

            int rowIndex = 1;
            int colIndex = 0;


            if (rowNumber == 0)
            {
                return false;
            }

            //建立Excel对象
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Application.Workbooks.Add(true);
            excel.Visible = isShowExcle;

            //生成字段名称
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.ColumnName;
            }

            //填充数据
            foreach (DataRow row in dataTable.Rows)
            {
                rowIndex++;
                colIndex = 0;
                foreach (DataColumn col in dataTable.Columns)
                {
                    colIndex++;

                    excel.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString();
                }
            }

            return true;
        }

        /// <summary>
        /// 在已有的Excel 插入数据
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static bool InsertExcel(System.Data.DataTable dataTable)
        {
            int rowIndex = 1;
            int colIndex = 0;
            //加载excel
            Application app = new Application();
            Workbooks wb = app.Workbooks;
            _Workbook _wb = wb.Add(DirFileHelper.MapPath(fileDicPath) + "Role.xls");//"excel路径"
            //获取sheet
            Sheets sh = _wb.Sheets;
            _Worksheet _wsh = (_Worksheet)sh.get_Item(1);

            //生成字段名称
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;
                _wsh.Cells[1, colIndex] = col.ColumnName;
            }

            //填充数据
            foreach (DataRow row in dataTable.Rows)
            {
                rowIndex++;
                colIndex = 0;
                foreach (DataColumn col in dataTable.Columns)
                {
                    colIndex++;
                    _wsh.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString();
                }
            }

            //_wsh.Cells[3, 2] = "数据";//3为excel行，2为excel列
            _wsh.SaveAs(DirFileHelper.MapPath(fileDicPath) + "Role.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            _wb.Close();
            wb.Close();
            return true;
        }
        /// <summary>
        /// 连接Excel  读取Excel数据   并返回DataSet数据集合
        /// </summary>
        /// <param name="filepath">Excel服务器路径</param>
        /// <param name="tableName">Excel表名称</param>
        /// <returns></returns>
        public static DataSet ImportExcel(string filepath, string fileName, string tableName)
        {

            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
            OleDbConnection ExcelConn = new OleDbConnection(strCon);
            DataSet ds = new DataSet();
            try
            {
                string strCom = string.Format("SELECT * FROM [" + fileName + "$]");
                ExcelConn.Open();
                OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, ExcelConn);

                myCommand.Fill(ds, "[" + tableName + "$]");
                ExcelConn.Close();
            }
            catch (Exception ex)
            {
                Log.Error("导出 excel 出错", ex);
                throw ex;
            }
            return ds;
        }

    }
}
