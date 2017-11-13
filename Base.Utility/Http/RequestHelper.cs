using Base.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Base.Utility
{
    /// <summary>
    /// 请求相关操作类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 获得请求的字符串值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public static string GetString(string name)
        {
            if (HttpHelper.CurrentRequest[name] == null)
            {
                return "";
            }
            return HttpHelper.CurrentRequest[name];
        }

        /// <summary>
        /// 获得请求的Int值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public static int GetInt(string name)
        {
            return StringHelper.StrToInt32(HttpHelper.CurrentRequest[name], 0);
        }

        /// <summary>
        /// 获得查询字符串的值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public static string GetQueryString(string name)
        {
            if (HttpHelper.CurrentRequest.QueryString[name] == null)
            {
                return "";
            }
            return HttpHelper.CurrentRequest.QueryString[name];
        }

        /// <summary>
        /// 获得查询字符串中的Int值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>值（转换失败时，默认值为0）</returns>
        public static int GetQueryInt(string name)
        {
            return GetQueryInt(name, 0);
        }

        /// <summary>
        /// 获得查询字符串中的Int值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>值</returns>
        public static int GetQueryInt(string name, int defaultValue)
        {
            return StringHelper.StrToInt32(HttpHelper.CurrentRequest.QueryString[name], defaultValue);
        }


        /// <summary>
        /// 过滤Html标记和其他符号
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>过滤后的内容</returns>
        public static string FilterHtml(string content)
        {
            string text = content.Trim();

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, "<\\/?[^>]+>", "");	// 去掉所有的html标记
            text = Regex.Replace(text, "[\\s]{2,}", "");	// 除两个以上的空格
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", "");	//　替换&nbsp;

            return text;
        }

        /// <summary>
        /// 替换Html标记和其他符号
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns>替换后的内容</returns>
        public static string ReplaceHtml(string content)
        {
            string text = content.Trim();

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = Regex.Replace(text, "[\\s]{2,}", " ");	// 除两个以上的空格
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	// 替换<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//　替换&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags

            return text;
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="name">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string name)
        {
            if (HttpHelper.CurrentRequest.ServerVariables[name] == null)
            {
                return "";
            }
            return HttpHelper.CurrentRequest.ServerVariables[name].ToString();
        }

        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpHelper.CurrentRequest.UrlReferrer == null)
            {
                return false;
            }
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpHelper.CurrentRequest.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string pageName = string.Empty;

            string absolutePath = HttpHelper.CurrentRequest.Url.AbsolutePath;
            try
            {
                pageName = absolutePath.Substring(absolutePath.LastIndexOf("/") + 1).ToLower();
            }
            catch { }

            return pageName;
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;

            result = GetServerString("HTTP_X_FORWARDED_FOR");
            if (string.IsNullOrEmpty(result))
            {
                result = GetServerString("REMOTE_ADDR");
            }

            if (string.IsNullOrEmpty(result))
            {
                result = HttpHelper.CurrentRequest.UserHostAddress;
            }

            if (string.IsNullOrEmpty(result) || !IsIP(result))
            {
                return "127.0.0.1";
            }

            return result;

        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">要验证的IP地址</param>
        /// <returns>bool值</returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }

        /// <summary>
        /// 保存用户上传的文件
        /// </summary>
        /// <param name="path">保存路径</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpHelper.CurrentRequest.Files.Count > 0)
            {
                HttpHelper.CurrentRequest.Files[0].SaveAs(path);
            }
        }
    }
}
