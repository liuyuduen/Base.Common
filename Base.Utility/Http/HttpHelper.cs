using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Net;
using System.IO;

namespace Base.Utility
{  /// <summary>
   /// Http相关辅助类
   /// </summary>
    public class HttpHelper
    {
        static HttpHelper()
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17";
            Timeout = 100000;
        }


        /// <summary>
        /// post请求到指定地址并获取返回的信息内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求参数</param>
        /// <param name="encodeType">编码类型如：UTF-8</param>
        /// <returns>返回响应内容</returns>
        public static string HttpPost(string POSTURL, string PostData)
        {
            //发送请求的数据
            WebRequest myHttpWebRequest = WebRequest.Create(POSTURL);
            myHttpWebRequest.Method = "POST";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(PostData);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = byte1.Length;
            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            //发送成功后接收返回的XML信息
            HttpWebResponse response = (HttpWebResponse)myHttpWebRequest.GetResponse();
            string lcHtml = string.Empty;
            Encoding enc = Encoding.GetEncoding("UTF-8");
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, enc);
            lcHtml = streamReader.ReadToEnd();
            return lcHtml;
        }

        /// <summary>
        /// 以GET方式抓取远程页面内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string Get_Http(string url)
        {
            string strResult;
            try
            {
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
                hwr.Timeout = 19600;
                HttpWebResponse hwrs = (HttpWebResponse)hwr.GetResponse();
                Stream myStream = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    sb.Append(sr.ReadLine() + "\r\n");
                }
                strResult = sb.ToString();
                hwrs.Close();
            }
            catch (Exception ee)
            {
                strResult = ee.Message;
            }
            return strResult;
        }


        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address) as HttpWebRequest;
                if (request == null) return null;
                request.Timeout = Timeout;
                request.UserAgent = UserAgent;
                return request;
            }
        }

        /// <summary>
        /// 获取或设置 使用的UserAgent信息
        /// </summary>
        /// <remarks>
        /// 可以到<see cref="http://www.sum16.com/resource/user-agent-list.html"/>查看更多User-Agent
        /// </remarks>
        public static String UserAgent { get; set; }
        /// <summary>
        /// 获取或设置 请求超时时间
        /// </summary>
        public static Int32 Timeout { get; set; }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="url">路径</param>
        /// <param name="message">out 内容信息</param>
        /// <param name="encoding">编码 默认UTF8</param>
        /// <returns>返回true 成功 false 失败</returns>
        public static Boolean GetContentString(String url, out String message, Encoding encoding)
        {
            try
            {
                if (encoding == null) encoding = Encoding.UTF8;
                using (var wc = new MyWebClient())
                {
                    message = encoding.GetString(wc.DownloadData(url));
                    return true;
                }
            }
            catch (Exception exception)
            {
                message = exception.Message;
                return false;
            }
        }


        /// <summary>
        ///  POST数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static String Post(String url, Byte[] data, Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            String str;
            using (var wc = new MyWebClient())
            {
                wc.Headers["Content-Type"] = "application/json";// "application/x-www-form-urlencoded";
                wc.Encoding = System.Text.Encoding.UTF8;
                var ret = wc.UploadData(url, "POST", data);
                str = encoding.GetString(ret);
            }
            return str;
        }

        public static Byte[] DownloadData(String address)
        {
            Byte[] data;
            using (var wc = new MyWebClient())
            {
                data = wc.DownloadData(address);
            }
            return data;
        }

        /// <summary>
        /// 获得网页内容长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Int64 GetContentLength(String url)
        {
            Int64 length;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = UserAgent;
            req.Method = "HEAD";
            req.Timeout = 5000;
            var res = (HttpWebResponse)req.GetResponse();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                length = res.ContentLength;
            }
            else
            {
                length = -1;
            }
            res.Close();
            return length;
        }


        /// <summary>
        /// 获得当前Page对象
        /// </summary>
        public static Page CurrentPage
        {
            get
            {
                return (Page)HttpContext.Current.Handler;
            }
        }

        /// <summary>
        /// 获得当前Cache对象
        /// </summary>
        public static Cache CurrentCache
        {
            get
            {
                return HttpContext.Current.Cache;
            }
        }

        /// <summary>
        /// 获得当前Requset对象
        /// </summary>
        public static HttpRequest CurrentRequest
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        /// <summary>
        /// 获得当前Response对象
        /// </summary>
        public static HttpResponse CurrentResponse
        {
            get
            {
                return HttpContext.Current.Response;
            }
        }

        /// <summary>
        /// 获得当前Server对象
        /// </summary>
        public static HttpServerUtility CurrentServer
        {
            get
            {
                return HttpContext.Current.Server;
            }
        }

        /// <summary>
        /// 获得当前Session对象
        /// </summary>
        public static HttpSessionState CurrentSession
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

    }
    public class UrlHelper
    {

        #region 网址处理：编码/解码/参数处理

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 剔除网址中的指定参数
        /// </summary>
        /// <param name="surl">原网址，如：http://www.cp158.com/default.aspx?page=5&type=1</param>
        /// <param name="sparam">要剔除的参数。如：page</param>
        /// <returns>如：http://www.cp158.com/default.aspx?type=1</returns>
        public static string ParseUrl(string surl, string sparam)
        {
            StringBuilder sb = new StringBuilder();
            string[] urls = surl.Split('?');
            sb.Append(urls[0]);
            if (urls.Length > 1)
            {

                string[] tmps = urls[1].Split('&');
                if (tmps.Length > 0)
                    sb.Append("?");
                bool isFirst = true;
                for (int i = 0; i < tmps.Length; i++)
                {
                    string[] tips = tmps[i].Split('=');
                    if (tips[0] != sparam)
                    {
                        if (isFirst)
                        {
                            sb.Append(tips[0]);
                            sb.Append("=");
                            sb.Append(tips[1]);
                            isFirst = false;
                        }
                        else
                        {
                            sb.Append("&");
                            sb.Append(tips[0]);
                            sb.Append("=");
                            sb.Append(tips[1]);
                        }
                    }
                }
            }
            return sb.ToString().TrimEnd('?');
        }


        /// <summary>
        /// 处理重写URL（下划线替换）
        /// </summary>
        public static string CleanRewriteURL(string url)
        {
            string[] aryReg = { "‘", "’", "（", "）", "：", "'", "'", "<", ">", "%", "\"\"", ",", ".", ">=", "=<", "-", "_", ";", "||", "[", "]", "&", "/", "-", "|", "(", ")", "Ⅲ", "Ⅱ", "Ⅳ", "*", "”", "~", "@", "#", "$", "^", "!", "\"", "+" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                url = url.Replace(aryReg[i], string.Empty);
            }
            return url.Replace(" ", "_");
        }

        /// <summary>
        /// 处理重写URL（中横线替换）
        /// </summary>
        public static string CleanRewriteURL1(string url)
        {
            string[] aryReg = { "‘", "’", "（", "）", "：", "'", "<", ">", "%", "\"\"", ",", ".", ">=", "=<", "-", "_", ";", "||", "[", "]", "&", "/", "-", "|", "(", ")", "Ⅲ", "Ⅱ", "Ⅳ", "*", "”", "~", "@", "#", "$", "^", "!", "\"", "+" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                url = url.Replace(aryReg[i], string.Empty);
            }
            return url.Replace(" ", "-");
        }

        #endregion
    }
}
