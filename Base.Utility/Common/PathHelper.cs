using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Base.Utility
{
    /// <summary>
    /// 路径辅助类
    /// </summary>
    public class PathHelper
    {

        /// <summary>
        /// 获取一个文件的绝对路径（适用于WEB应用程序）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>string</returns>
        public static string GetRealFile(string filePath)
        {
            string strResult = "";

            strResult = ((filePath.IndexOf(":\\") > 0) ?
                filePath :
                GetRootPath() + "/" + filePath);
            return strResult;
        }

        /// <summary>
        /// 取得网站根目录的物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
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
            return AppPath;
        }



        /// <summary>
        /// 获得网站的根URL
        /// </summary>
        /// <returns>网站的根URL</returns>
        public static string GetBaseUrl()
        {
            if (HttpHelper.CurrentRequest.ApplicationPath == "/")
            {
                return "http://" + RequestHelper.GetServerString("HTTP_HOST");
            }
            return "http://" + RequestHelper.GetServerString("HTTP_HOST") + HttpHelper.CurrentRequest.ApplicationPath;
        }

        /// <summary>
        /// 获得不带Http的根URL
        /// </summary>
        /// <returns>不带Http的根URL</returns>
        public static string GetNoHttpBaseUrl()
        {
            if (HttpHelper.CurrentRequest.ApplicationPath == "/")
            {
                return RequestHelper.GetServerString("HTTP_HOST");
            }
            return RequestHelper.GetServerString("HTTP_HOST") + HttpHelper.CurrentRequest.ApplicationPath;
        }

        #region 路径转换（转换成绝对路径）
        /// <summary>
        /// 路径转换（转换成绝对路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string WebPathTran(string path)
        {
            try
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            catch
            {
                return path;
            }
        }
        #endregion
    }
}
