using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Base.Utility
{
    public class LogHelper
    {

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="msg">内容</param>
        public static void LogWriter(string fileName, string msg)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(GetLogPath() + fileName + ".log");

                if (!Directory.Exists(GetLogPath())) Directory.CreateDirectory(GetLogPath());
                if (fileInfo.Exists)
                {
                    StreamWriter streamWriter = new StreamWriter(fileInfo.FullName, true, Encoding.GetEncoding("utf-8"));
                    streamWriter.WriteLine("-------------------");
                    streamWriter.WriteLine(DateTime.Now.ToString() + ":" + msg);
                    ((TextWriter)streamWriter).Flush();
                    streamWriter.Close();
                }
                else
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fileInfo.FullName, false, System.Text.Encoding.GetEncoding("utf-8"));
                    sw.WriteLine(DateTime.Now.ToString() + ":" + msg);
                    sw.Close();
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="docName">日志路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="msg">内容</param>
        public static void LogWriter(string docName, string fileName, string msg)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(GetLogPath() + docName + "\\" + fileName + ".log");

                if (!Directory.Exists(GetLogPath() + docName + "\\")) Directory.CreateDirectory(GetLogPath() + docName + "\\");
                if (fileInfo.Exists)
                {
                    StreamWriter streamWriter = new StreamWriter(fileInfo.FullName, true, Encoding.GetEncoding("utf-8"));
                    streamWriter.WriteLine("-------------------");
                    streamWriter.WriteLine(DateTime.Now.ToString() + ":" + msg);
                    ((TextWriter)streamWriter).Flush();
                    streamWriter.Close();
                }
                else
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fileInfo.FullName, false, System.Text.Encoding.GetEncoding("utf-8"));
                    sw.WriteLine(DateTime.Now.ToString() + ":" + msg);
                    sw.Close();
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name=""fileName"">路径</param> 
        /// <returns></returns>
        public static bool ClearLog(string fileName)
        {
            bool result = false;
            try
            {
                DeleteFile(GetLogPath() + fileName + "\\");
                Log.Info("清空文件夹" + fileName);
                result = true;

            }
            catch
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 批量删除文件夹下的文件
        /// </summary>
        /// <param name="DirPath">路径</param>
        private static void DeleteFile(string DirPath)
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
        /// 设置日志路径
        /// </summary>
        /// <returns></returns>
        public static string GetLogPath()
        {
            string AppPath = "";
            HttpContext HttpCurrent = HttpContext.Current;
            if (HttpCurrent != null)
            {
                AppPath = HttpCurrent.Server.MapPath("~");
            }
            else
            {
                AppPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Regex.Match(AppPath, @"\\$", RegexOptions.Compiled).Success)
                    AppPath = AppPath.Substring(0, AppPath.Length - 1);
            }
            return AppPath + "\\Logs\\";
        }
    }
}
