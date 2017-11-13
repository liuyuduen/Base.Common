using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Data.OleDb;
using System.Data;
using System.Collections;

namespace Base.Utility
{
    /// <summary>
    /// 文件及文件夹操作
    /// </summary>
    public class FileTool
    {

        #region "文件读写"

        /// <summary>
        /// 以文件流的形式将内容写入到指定文件中（如果该文件或文件夹不存在则创建）
        /// </summary>
        /// <param name="file">文件名和指定路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <param name="Append">是否追加指定内容到该文件中</param>
        /// <returns>返回布尔值</returns>
        public static bool WriteFile(string file, string fileContent, bool Append)
        {
            FileInfo f = new FileInfo(file);
            // 如果文件所在的文件夹不存在则创建文件夹
            if (!Directory.Exists(f.DirectoryName)) Directory.CreateDirectory(f.DirectoryName);

            StreamWriter sWriter = new StreamWriter(file, Append, Encoding.GetEncoding("utf-8"));

            try
            {
                sWriter.Write(fileContent);
                return true;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.ToString());
            }
            finally
            {
                sWriter.Flush();
                sWriter.Close();
            }
        }

        /// <summary>
        /// 以文件流的形式将内容写入到指定文件中（如果该文件或文件夹不存在则创建）
        /// </summary>
        /// <param name="file">文件名和指定路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <param name="Append">是否追加指定内容到该文件中</param>
        /// <param name="strEncoding">指定编码，如ascii,Unicode,Unicode big endian,UTF-8</param>
        /// <returns>返回布尔值</returns>
        public static bool WriteFile(string file, string fileContent, bool Append, string strEncoding)
        {
            FileInfo f = new FileInfo(file);
            // 如果文件所在的文件夹不存在则创建文件夹
            if (!Directory.Exists(f.DirectoryName)) Directory.CreateDirectory(f.DirectoryName);

            StreamWriter sWriter = new StreamWriter(file, Append, Encoding.GetEncoding(strEncoding));

            try
            {
                sWriter.Write(fileContent);
                return true;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.ToString());
            }
            finally
            {
                sWriter.Flush();
                sWriter.Close();
            }
        }

        /// <summary>
        /// 以文件流的形式读取指定文件的内容
        /// </summary>
        /// <param name="file">指定的文件及其全路径</param>
        /// <returns>返回 String</returns>
        public static string ReadFile(string file)
        {
            string strResult = "";

            FileStream fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader sReader = new StreamReader(fStream, Encoding.Default);

            try
            {
                strResult = sReader.ReadToEnd();
            }
            catch { }
            finally
            {
                fStream.Flush();
                fStream.Close();
                sReader.Close();
            }

            return strResult;
        }


        /// <summary>
        /// 读取excle文件，将每一行数据作为数组的一个元素返回,其中每个字段之间用"^"符号连接
        /// </summary>
        /// <param name="filePath">excle文件的路径，如：Server.MapPath("~/") + "\\" + "SampleExcel.xls"</param>
        /// author:chuck
        /// 注意：excle文件第一行是标题，不是正式数据
        /// <returns></returns>
        public static ArrayList ReadXlsFile(string filePath)
        {
            ArrayList XlsLineList = new ArrayList();

            string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = " + filePath + ";Extended Properties=Excel 8.0";///建立连接,地址为str传递的地址 
            OleDbConnection Conn = new OleDbConnection(strCon);
            string command = " SELECT * FROM [Sheet1$]";   //SQL操作语句,就是说:取得所有数据从Sheet1 
            Conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(command, Conn);
            DataSet dataset = new DataSet();               //建立新的数据集 
            adapter.Fill(dataset, "[Sheet1$]");            //填充数据集 
            Conn.Close();

            int columnNum = dataset.Tables[0].Columns.Count;//列数 
            int rowNum = dataset.Tables[0].Rows.Count;      //行数 
            foreach (DataRow dr in dataset.Tables[0].Rows)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < columnNum; i++)
                {
                    sb.Append(dr[i]).Append("^");

                }
                XlsLineList.Add(sb.ToString());
            }
            return XlsLineList;
        }

        /// <summary>
        /// 读取csv文件，将每一行数据作为数组的一个元素返回,其中每个字段之间用"^"符号连接
        /// </summary>
        /// <param name="filePath">csv文件的路径，如：Server.MapPath("~/") + "\\" + "SampleCSV.csv"</param>
        /// author:chuck
        /// <returns></returns>
        public static ArrayList ReadCsvFile(string filePath)
        {
            ArrayList CsvLineList = new ArrayList();
            string strline = "";
            System.IO.StreamReader reader = new System.IO.StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"));//读取cvs文件
            while ((strline = reader.ReadLine()) != null)  //每次单独抽取cvs一行的内容
            {
                strline = dealcode(strline).Replace("\"", "");//将临时用到的双引号去掉
                CsvLineList.Add(strline);
            }
            reader.Close();
            return CsvLineList;

        }

        /// <summary>
        /// 对读取到的cvs单独一行内容进行处理，去掉cvs格式，返回常规字符串，每项之间用特殊字符“^”隔开
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string dealcode(string str)
        {
            string s = "";
            int k = 1;
            if (str.Length == 0) return "";
            str = str.Replace("^", "");
            for (int i = 0; i < str.Length; i++)
            {
                switch (str.Substring(i, 1))
                {
                    case "\"":
                        s += str.Substring(i, 1);//csv文件有个特性，一旦字段中加了特殊符号如“，”，那么会自动给该字段外面加上双引号。
                        k++;
                        break;
                    case ",":
                        if (k % 2 == 0)
                            s += str.Substring(i, 1);//双引号内的","保留下来，否则当作另外字段
                        else
                            s += "^";
                        break;
                    default: s += str.Substring(i, 1); break;
                }
            }
            return s;
        }

        #endregion 

        #region "文件上传"

        private static IList<string> IMAGE_FILTER = new string[] { ".jpg", ".bmp", ".tif", ".png", ".jpeg", ".gif" };
        private static IList<string> OFFICE_FILTER = new string[] { ".doc", ".xls", ".txt" };
        private static IList<string> PDF_FILTER = new string[] { ".pdf" };
        private static IList<string> ZIP_FILTER = new string[] { ".zip" };
        private static IList<string> SOUND_FILTER = new string[] { ".wav", ".mp3" };
        //private static readonly int MAXLENGTH = 30720; //30k

            

        /// <summary>
        /// 文件上传简洁版，默认类型为图片
        /// </summary>
        /// <param name="requestName"></param>        
        /// <param name="MAXLENGTH">最大尺寸，k为单位</param>
        /// <param name="TipLanguage">抛出异常的语言，1=EN,2=CN</param>
        /// <param name="newFileName"></param>
        public static void UploadFile(string requestName, int MAXLENGTH, int TipLanguage, string newFileName)
        {
            UploadFile(requestName, newFileName, false, MAXLENGTH, TipLanguage, FileFilter.Image);
        }
        /// <summary>
        /// 文件上传。
        /// </summary>
        /// <param name="requestName">上传控件名称，如："Upload"</param>
        /// <param name="newFileName">保存文件的路径+文件名</param>
        /// <param name="overWrite">是否覆盖同名文件</param>        
        /// <param name="MAXLENGTH">最大尺寸，k为单位</param>
        /// <param name="TipLanguage">抛出异常的语言，1=EN,2=CN</param>
        /// <param name="filter">允许的文件后缀</param>
        public static void UploadFile(string requestName, string newFileName, bool overWrite, int MAXLENGTH, int TipLanguage, FileFilter filter)
        {
            switch (filter)
            {
                case FileFilter.Image:
                    UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, IMAGE_FILTER);
                    break;
                case FileFilter.Office:
                    UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, OFFICE_FILTER);
                    break;
                case FileFilter.Pdf:
                    UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, PDF_FILTER);
                    break;
                case FileFilter.Zip:
                    UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, ZIP_FILTER);
                    break;
                case FileFilter.Sound:
                    UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, SOUND_FILTER);
                    break;
            }
        }

        /// <summary>
        /// 文件上传。
        /// </summary>
        /// <param name="requestName">上传控件名称，如："Upload"</param>
        /// <param name="newFileName">保存文件的路径+文件名</param>
        /// <param name="overWrite">是否覆盖同名文件</param>        
        /// <param name="MAXLENGTH">最大尺寸，k为单位</param>
        /// <param name="TipLanguage">抛出异常的语言，1=EN,2=CN</param>
        /// <param name="filter">允许的文件后缀</param>
        public static void UploadFile(string requestName, string newFileName, bool overWrite, int MAXLENGTH, int TipLanguage, params string[] filter)
        {
            UploadFile(requestName, newFileName, overWrite, MAXLENGTH, TipLanguage, (IList<string>)filter);
        }
        /// <summary>
        /// 文件上传基本方法
        /// </summary>
        /// <param name="requestName"></param>
        /// <param name="newFileName"></param>
        /// <param name="overWrite"></param>
        /// <param name="MAXLENGTH">最大尺寸，k为单位</param>
        /// <param name="TipLanguage">抛出异常的语言，1=EN,2=CN</param>
        /// <param name="filter"></param>
        public static void UploadFile(string requestName, string newFileName, bool overWrite, int MAXLENGTH, int TipLanguage, IList<string> filter)
        {
            if (filter == null || filter.Count == 0)
            {
                filter = IMAGE_FILTER;
            }
            if (File.Exists(newFileName))
            {
                if (!overWrite)
                {
                    //throw new ArgumentException(TipLanguage==1?"File Exists!":"已存在该名称的文件！"); 
                }
                else
                    File.SetAttributes(newFileName, FileAttributes.Normal);
            }
            HttpPostedFile file;
            file = HttpContext.Current.Request.Files[requestName];
            if (file == null || file.InputStream.Length <= 0)
            {
                //throw new ArgumentException(TipLanguage == 1 ? "System can not find a file to upload !" : "未找到要上传的文件！");      
            }
            if (file.InputStream.Length > MAXLENGTH * 1024)
            {
                //throw new ArgumentException(TipLanguage == 1 ? "Upload file is too large !" : "要上传的文件太大！");  
            }
            if (!filter.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                //throw new ArgumentException(TipLanguage == 1 ? "Upload file type is incorrect !" : "不允许上传该类型的文件！"); 
            }
            try
            {
                file.SaveAs(newFileName);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                file = null;
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="newFileName"></param>
        /// <param name="overWrite"></param>
        /// <param name="MAXLENGTH">最大尺寸，k为单位</param>
        /// <param name="TipLanguage">抛出异常的语言，1=EN,2=CN</param>
        /// <param name="filters"></param>
        public static void UpFile(HttpPostedFile file, string newFileName, bool overWrite, int MAXLENGTH, int TipLanguage, FileFilter filters)
        {
            IList<string> filter = null;
            switch (filters)
            {
                case FileFilter.Image:
                    filter = IMAGE_FILTER;
                    break;
                case FileFilter.Office:
                    filter = OFFICE_FILTER;
                    break;
                case FileFilter.Pdf:
                    filter = PDF_FILTER;
                    break;
                case FileFilter.Zip:
                    filter = ZIP_FILTER;
                    break;
                case FileFilter.Sound:
                    filter = SOUND_FILTER;
                    break;
            }

            if (filter == null || filter.Count == 0)
            {
                filter = IMAGE_FILTER;
            }

            if (File.Exists(newFileName))
            {
                if (!overWrite)
                {
                    //throw new ArgumentException(TipLanguage == 1 ? "File Exists!" : "已存在该名称的文件！"); 
                }
                else
                    File.SetAttributes(newFileName, FileAttributes.Normal);
            }


            if (file == null || file.InputStream.Length <= 0)
            {
                //throw new ArgumentException(TipLanguage == 1 ? "System can not find a file to upload !" : "未找到要上传的文件！");  
            }
            if (file.InputStream.Length > MAXLENGTH)
            {
                //throw new ArgumentException(TipLanguage == 1 ? "Upload file is too large !" : "要上传的文件太大！"); 
            }
            if (!filter.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                //throw new ArgumentException(TipLanguage == 1 ? "Upload file type is incorrect !" : "不允许上传该类型的文件！"); 
            }
            try
            {
                file.SaveAs(newFileName);
            }
            catch (Exception ex)
            {
                //throw new ArgumentException(TipLanguage == 1 ? "File upload failure !" : "文件上传失败！"); 
            }
            finally
            {
                file = null;
            }
        }

        #endregion

        #region 文件下载

        /// <summary>
        /// 下载指定路径的文件
        /// </summary>
        /// <param name="FileName">如：C\123\123.zip</param>
        public static void FileDownload(string FileName)
        {
            FileInfo DownloadFile = new FileInfo(FileName);
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = false;
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(DownloadFile.FullName, System.Text.Encoding.UTF8));
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            System.Web.HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 下载指定路径的文件,可以指定文件名称
        /// </summary>
        /// <param name="filePath">如：C\123\123.zip</param>
        /// <param name="fileName">下载显示和保存的文件名称 如123.zip</param>
        public static void FileDownload(string filePath, string fileName)
        {
            FileInfo DownloadFile = new FileInfo(filePath);
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.Buffer = false;
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            System.Web.HttpContext.Current.Response.WriteFile(DownloadFile.FullName);
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        #endregion

        #region static string ReadLine(string fileName) 读取某个文件的一行内容
        /// <summary>
        /// 读取某个文件的一行内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>读取的内容</returns>
        public static string ReadLine(string fileName)
        {
            return ReadLine(fileName, Encoding.Default);
        }
        #endregion

        #region static string ReadLine(string fileName, Encoding coding) 用编码读取某个文件的一行内容
        /// <summary>
        /// 用编码读取某个文件的一行内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="coding">编码</param>
        /// <returns>读取的内容</returns>
        public static string ReadLine(string fileName, Encoding coding)
        {
            using (StreamReader reader = new StreamReader(fileName, coding))
            {
                return reader.ReadLine();
            }
        }
        #endregion

        #region static string ReadToEnd(string fileName) 读取某个文件的所有内容
        /// <summary>
        /// 读取某个文件的所有内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件的所有内容</returns>
        public static string ReadToEnd(string fileName)
        {
            return ReadToEnd(fileName, Encoding.Default);
        }
        #endregion

        #region static string ReadToEnd(string fileName, Encoding coding) 用编码读取某个文件的所有内容
        /// <summary>
        /// 用编码读取某个文件的所有内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="coding">编码</param>
        /// <returns>文件的所有内容</returns>
        public static string ReadToEnd(string fileName, Encoding coding)
        {
            using (StreamReader reader = new StreamReader(fileName, coding))
            {
                return reader.ReadToEnd();
            }
        }
        #endregion

        #region static void WriteText(string fileName, string text, bool append) 把内容写入某个文件
        /// <summary>
        /// 把内容写入某个文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="text">要写入的内容</param>
        /// <param name="append">是否追加</param>
        public static void WriteText(string fileName, string text, bool append)
        {
            WriteText(fileName, text, append, Encoding.Default);
        }
        #endregion

        #region static void WriteText(string fileName, string text, bool append, Encoding coding) 用编码把内容写入某个文件
        /// <summary>
        /// 用编码把内容写入某个文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="text">要写入的内容</param>
        /// <param name="append">是否追加</param>
        /// <param name="coding">编码</param>
        public static void WriteText(string fileName, string text, bool append, Encoding coding)
        {
            using (StreamWriter writer = new StreamWriter(fileName, append, coding))
            {
                writer.WriteLine(text);
            }
        }
        #endregion

         

        /// <summary>
        /// 删除批定文件夹
        /// </summary>
        /// <param name="FolderPath"></param>
        public static void DeleteFolder(string FolderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderPath);

            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Attributes.ToString().IndexOf("ReadOnly") != -1)
                    files[i].Attributes = FileAttributes.Normal;
                files[i].Delete();
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                DeleteFolder(dirs[i].FullName);
            }

            dir.Delete(true);
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        /// <param name="arrExtName"></param>
        public static void CopyFolder(string sourceFolder, string destFolder, ref ArrayList arrExtName)
        {
            DirectoryInfo dir_source = new DirectoryInfo(sourceFolder);
            DirectoryInfo dir_dest = new DirectoryInfo(destFolder);
            if (!dir_dest.Exists)
                dir_dest.Create();

            FileInfo[] files = dir_source.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (!arrExtName.Contains(Path.GetExtension(files[i].Name.ToLower())))
                    File.Copy(files[i].FullName, dir_dest.FullName.Trim('\\') + "\\" + files[i].Name);
            }

            DirectoryInfo[] dirs = dir_source.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                CopyFolder(dirs[i].FullName, destFolder + "\\" + dirs[i].Name, ref arrExtName);
            }
        }

        /// <summary>
        /// 将整个文件夹复制到目标文件夹中。操作成功返回true
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="newPath">目标文件夹</param>
        public static bool CopyFolder(string srcPath, string newPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (newPath[newPath.Length - 1] != Path.DirectorySeparatorChar)
                    newPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                // string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                        CopyFolder(file, newPath + Path.GetFileName(file));
                    // 否则直接Copy文件
                    else
                        File.Copy(file, newPath + Path.GetFileName(file), true);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除文件夹下的文件
        /// </summary>
        /// <param name="DirPath"></param>
        public static void DeleteFile(string DirPath)
        {
            try
            {
                if (Directory.Exists(DirPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(DirPath);
                    FileInfo[] files = dir.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        try
                        {
                            File.Delete(files[i].FullName);
                        }
                        catch { }
                    }

                    DirectoryInfo[] dirs = dir.GetDirectories();
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        DeleteFile(dirs[i].FullName);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 写二进制文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="fileFullPath"></param>
        public static void WriteBinaryFile(byte[] buffer, string fileFullPath)
        {
            if (buffer != null && buffer.Length > 0)
            {
                FileStream fs = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(buffer);
                bw.Close();
            }
        }
        #region 访问网页
        /// <summary>
        ///  读取某一绝对或相对地址，将将内容输出到html格式..
        /// </summary>
        /// <param name="readHtml">读出地址...</param>
        /// <returns>返回字符值</returns>
        public string ReadUrl(string readHtml)
        {
            bool isError;
            return ReadUrl(readHtml, "GB2312", out isError);
        }

        /// <summary>
        ///  读取某一绝对或相对地址，将将内容输出到html格式..
        /// </summary>
        /// <param name="readHtml">读出地址...</param>
        /// <param name="encoding">1.(GB2312) 2.(utf-8) 3.(big5)</param>
        /// <param name="isError">出错时返回true</param>
        /// <returns>返回字符值</returns>
        public string ReadUrl(string readHtml, string encoding, out bool isError)
        {
            isError = false;
            string strHtml = string.Empty;
            try
            {
                WebResponse myWebResponse;
                WebRequest mWebRequest = WebRequest.Create(readHtml);
                try
                {
                    myWebResponse = mWebRequest.GetResponse();
                }
                catch (WebException rspWebEx)
                {
                    myWebResponse = rspWebEx.Response;
                    isError = true;
                }
                using (var responseStream = myWebResponse.GetResponseStream())
                {
                    //TODO: how to process when responseStream is null ?
                    using (var mySr = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("" + encoding + "")))
                    {
                        strHtml = mySr.ReadToEnd();
                        mySr.Close();
                    }
                    myWebResponse.Close();
                    responseStream.Close();
                }
            }
            catch (Exception ex)
            {
                isError = true;
            }

            if (isError)
            {
            }

            return strHtml;
        }

        /// <summary>
        /// Post请求绝对或相对地址，输出内容
        /// </summary>
        /// <param name="readHtml">URL地址</param>
        /// <param name="encoding">页面编码</param>
        /// <param name="isError">调用页面时是否发生错误</param>
        /// <returns></returns>
        public static string PostUrl(string readHtml, string strParam, string encoding, out bool isError)
        {
            isError = false;
            string strHtml = string.Empty;
            try
            {
                Encoding pageEncode;
                WebResponse myWebResponse;
                WebRequest mWebRequest = WebRequest.Create(readHtml);
                mWebRequest.Method = "Post";
                mWebRequest.ContentType = "application/x-www-form-urlencoded";
                //mWebRequest.Timeout = 3000;
                try
                {
                    pageEncode = System.Text.Encoding.GetEncoding("" + encoding + "");
                }
                catch
                {
                    pageEncode = Encoding.Default;
                }
                if (strParam != null)
                {
                    using (Stream reqStream = mWebRequest.GetRequestStream())
                    {
                        byte[] bsParam = new byte[0];
                        bsParam = pageEncode.GetBytes(strParam);
                        reqStream.Write(bsParam, 0, bsParam.Length);
                    }
                }
                try
                {
                    myWebResponse = mWebRequest.GetResponse();
                }
                catch (WebException rspWebEx)
                {
                    myWebResponse = rspWebEx.Response;
                    isError = true;
                }
                using (var responseStream = myWebResponse.GetResponseStream())
                {
                    //TODO: how to process when responseStream is null ?
                    using (var mySr = new StreamReader(responseStream, pageEncode))
                    {
                        strHtml = mySr.ReadToEnd();
                        mySr.Close();
                    }
                    myWebResponse.Close();
                    responseStream.Close();
                }

            }
            catch (Exception ex)
            {
                isError = true;
            }

            if (isError)
            {
            }

            return strHtml;
        }

        #endregion

        #region  整站静态化

        /// <summary>
        /// 删除指定固定的文件名或文件夹
        /// </summary>
        /// <param name="FullPath">输入要删除的文件</param>
        public string Delete(string FullPath)
        {
            string Rvalue;

            if (FullPath.IndexOf("/") > 0) //删除文件
            {
                try
                {
                    File.Delete(FullPath);
                    Rvalue = "0";
                }
                catch (System.Exception ex)
                {
                    Rvalue = ex.Message;
                }
            }
            else //删除目录
            {
                try
                {
                    Directory.Delete(FullPath);
                    Rvalue = "0";
                }
                catch (System.Exception ex)
                {
                    Rvalue = ex.Message;
                }
            }

            return Rvalue;
        }


        /// <summary>
        /// 读取本地文件文件的方法....
        /// </summary>
        /// <param name="Txt">文件文件的相对路径名</param>
        /// <returns>返回文本文件的内容</returns>
        public string ReadTxt(string Txt)
        {
            string FileTxt = HttpContext.Current.Server.MapPath(Txt.ToString());
            FileStream fs = new FileStream(FileTxt, FileMode.Open);
            StreamReader objRead = new StreamReader(fs);
            string Ctxt = objRead.ReadToEnd();
            objRead.Close();
            return Ctxt;
        }

        /// <summary>
        /// 获取内容并生成Html格式的文件...
        /// </summary>
        /// <param name="OutHtml">输出的Html格式的文件名</param>
        /// <param name="OutText">输入的字符值</param>
        public string Make(string OutHtml, string InputText)
        {
            string Rvalue;

            try
            {
                string Temphtml = HttpContext.Current.Server.MapPath(OutHtml);
                Temphtml = Temphtml.Replace(@"\", @"\\").ToString();

                StreamWriter Sw = new StreamWriter(Temphtml, false, System.Text.Encoding.GetEncoding("GB2312"));
                Sw.WriteLine(InputText);
                Sw.Flush();
                Sw.Close();
                Rvalue = "0";
            }
            catch (System.Exception Ex)
            {
                Rvalue = Ex.Message;
            }

            return Rvalue;
        }


        /// <summary>
        /// 获取内容并生成Html格式的文件...
        /// </summary>
        /// <param name="OutHtml">输出的Html格式的文件名</param>
        /// <param name="Encoding">1.(GB2312) 2.(utf-8) 3.(big5)</param>
        /// <param name="OutText">输入的字符值</param>
        public string Make(string OutHtml, string InputText, string Encoding)
        {
            string Rvalue;

            try
            {
                string Temphtml = HttpContext.Current.Server.MapPath(OutHtml);
                Temphtml = Temphtml.Replace(@"\", @"\\").ToString();

                StreamWriter Sw = new StreamWriter(Temphtml, false, System.Text.Encoding.GetEncoding("" + Encoding + ""));
                Sw.WriteLine(InputText);
                Sw.Flush();
                Sw.Close();
                Rvalue = "0";
            }
            catch (System.Exception Ex)
            {
                Rvalue = Ex.Message;
            }

            return Rvalue;
        }

        #endregion


    }


    #region 允许上传的文件类型定义（可以根据项目实际调整，配合上面的文件上传代码使用）
    /// <summary>
    /// File Type
    /// </summary>
    public enum FileFilter
    {
        /// <summary>
        /// *.jpg,*.bmp,*.tif,*.png,*.jpeg,*.gif
        /// </summary>
        Image = 0,
        /// <summary>
        /// *.doc,*.xls,*.txt
        /// </summary>
        Office = 1,
        /// <summary>
        /// *.pdf
        /// </summary>
        Pdf = 2,
        /// <summary>
        /// *.zip
        /// </summary>
        Zip = 3,
        /// <summary>
        /// *.wav,*.mp3
        /// </summary>
        Sound = 4
    }
    #endregion


}
