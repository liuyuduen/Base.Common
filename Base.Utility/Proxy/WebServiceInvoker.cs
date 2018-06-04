using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Base.Utility.Proxy
{
    public class WebServiceInvoker<T>
    {
        public const int DEFAULT_HTTP_TIMEOUT = 100 * 1000; 

        public static string WEB_SERVICE_URL = ConfigHelper.AppSettings("WEB_SERVICE_URL");

        public const string DEFAULT_HTTP_METHOD = "POST";


        public static T Invoker<T>(CallArgs reqCall)
        {
            var content = Invoker(reqCall);
            if (!string.IsNullOrEmpty(content))
                return (T)JsonHelper.GetObject(content, typeof(T));
            return default(T);
        }


        public static string Invoker(CallArgs reqCall)
        {
            string content = string.Empty;

            var req_Url = string.Format("{0}/{1}/{2}/", reqCall.Url, reqCall.Service, reqCall.Action);


            byte[] bytes = ByteHelper.StringToBytes(reqCall.Body);
            var httpReq = new HttpReq()
            {
                Url = reqCall.Url,
                Headers = reqCall.HttpHeaders,
                Method = DEFAULT_HTTP_METHOD,
                Body = bytes,
                Timeout = reqCall.Timeout
            };
            var result = HttpPostForSmartProxy(httpReq);

            return result;
        }

         
        public static async Task<string> InvokerAsync(CallArgs reqCall)
        {
            return await Task.Run(() => Invoker(reqCall));
        }

        public static async Task<T> InvokerAsync<T>(CallArgs reqCall)
        {
            return await Task.Run(() => Invoker<T>(reqCall));
        }
         
        internal static string HttpPostForSmartProxy(HttpReq req)
        {
            HttpWebResponse response = null;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(req.Url);

                httpWebRequest.Method = req.Method;
                httpWebRequest.ContentType = req.ContentType;
                httpWebRequest.Accept = req.Accept;
                httpWebRequest.Timeout = req.Timeout;

                httpWebRequest.AllowAutoRedirect = req.AllowAutoRedirect;


                SetHttpHeader(httpWebRequest, req.Headers);


                if (req.Body != null)
                {
                    httpWebRequest.ContentLength = (long)req.Body.Length;
                    using (var stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(req.Body, 0, req.Body.Length);
                    }
                }
                else
                {
                    httpWebRequest.ContentLength = 0;
                }

                //获取返回信息 
                response = (HttpWebResponse)httpWebRequest.GetResponse();
                var myResponseStream = response.GetResponseStream();

                string result = "";
                //读取返回流
                if (myResponseStream != null)
                {
                    var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    result = myStreamReader.ReadToEnd();
                    //关闭请求连接
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
                return result;
            }
            catch (WebException ex)
            {
                // 对于连接级别的错误，直接抛出异常
                if (ex.InnerException != null && ex.InnerException is SocketException)
                    throw ex;

                // 注意，若捕获到 WebException 异常，则必须将异常中的Response对象关闭掉，否则将可能发生一些未预期的错误。
                // 如果在抛出 WebException 异常后，不关闭异常的 Response 属性，已知的未预期错误有：
                // ORA-24550: signal received: Unhandled exception: Code=e0434f4d Flags=1

                response = (HttpWebResponse)ex.Response;
                var errMsg = ex.Message + "\r\n" + GetErroMsgFromResponse(response);
                throw new Exception("HttpPost异常：\r\n" + errMsg);//NoticeException("HttpPost异常：\r\n" + errMsg);

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
         
        internal static void SetHttpHeader(WebRequest webRequest, IEnumerable<KeyValuePair<string, string>> headers)
        {
            // 避免多次类型转换
            var httpWebRequest = webRequest as HttpWebRequest;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    if (!WebHeaderCollection.IsRestricted(item.Key))
                    {
                        webRequest.Headers.Add(item.Key, item.Value);
                    }
                    else
                    {
                        /*
                         * 某些公共标头被视为受限制的，它们或者直接由 API（如 Content-Type）公开，或者受到系统保护，不能被更改。
                         *  Accept
                         *  Connection
                         *  Content-Length
                         *  Content-Type
                         *  Date
                         *  Expect
                         *  Host
                         *  If-Modified-Since
                         *  Range
                         *  Referer
                         *  Transfer-Encoding
                         *  User-Agent
                         *  Proxy-Connection
                         * */
                        var key = item.Key.Trim().ToLower();
                        switch (key)
                        {
                            case "content-type":
                                webRequest.ContentType = item.Value;
                                break;
                            default:
                                {
                                    try
                                    {
                                        if (webRequest is HttpWebRequest)
                                        {
                                            switch (key)
                                            {
                                                case "accept":
                                                    httpWebRequest.Accept = item.Value;
                                                    break;
                                                case "connection":
                                                    httpWebRequest.Connection = item.Value;
                                                    break;
#if NET40
                                            case "date":
                                                ((HttpWebRequest)webRequest).Date = DateTime.Parse(item.Value);
                                                break;
                                            case "host":
                                                ((HttpWebRequest)webRequest).Host = item.Value;
                                                break;
#endif
                                                case "if-modified-since":
                                                    httpWebRequest.IfModifiedSince = DateTime.Parse(item.Value);
                                                    break;
                                                case "keep-alive":
                                                    httpWebRequest.KeepAlive = bool.Parse(item.Value);
                                                    break;
                                                case "referer":
                                                    httpWebRequest.Referer = item.Value;
                                                    break;
                                                case "user-agent":
                                                    httpWebRequest.UserAgent = item.Value;
                                                    break;
                                                default:
                                                    throw new Exception("Not supported header: " + item.Key);
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Not supported header: " + item.Key);
                                        }
                                    }
                                    catch (ArgumentException)
                                    {
                                        // do nothing.
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            foreach (var k in webRequest.Headers.AllKeys)
            {
                if (k.ToLower() == "cookie" && webRequest is HttpWebRequest)
                {
                    var cookieStr = webRequest.Headers[k];
                    httpWebRequest.CookieContainer = new CookieContainer();
                    var pieces = cookieStr.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var p in pieces)
                    {
                        httpWebRequest.CookieContainer.SetCookies(webRequest.RequestUri, p);
                    }
                }

                if (k.ToLower() == "cache-control")
                {
                    var tmp = webRequest.Headers[k];
                    if (tmp != null && tmp.ToLower().Contains("no-cache"))
                    {
                        webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    }
                }
            }
        }
         
        private static string GetErroMsgFromResponse(HttpWebResponse response)
        {
            string errMsg = string.Empty;
            if (response == null)
            {
                return errMsg;
            }
            var data = new byte[0];
            if (response != null)
            {
                using (var s = response.GetResponseStream())
                {
                    Stream stream;
                    switch (response.ContentEncoding.ToUpperInvariant())
                    {
                        case "GZIP":
                            stream = new GZipStream(s, CompressionMode.Decompress);
                            break;
                        case "DEFLATE":
                            stream = new DeflateStream(s, CompressionMode.Decompress);
                            break;

                        default:
                            stream = s;
                            break;
                    }

                    using (stream)
                    {
                        data = new byte[response.ContentLength > 0 ? response.ContentLength : 0];
                        byte[] buffer = new byte[1024]; // HACK：每次读取的字节数
                        int copied = 0;
                        int n = 0;
                        while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (data.Length < copied + n)
                                Array.Resize(ref data, copied + n);

                            Array.Copy(buffer, 0, data, copied, n);
                            copied += n;
                        }
                    }
                }
            }

            return Encoding.UTF8.GetString(data);

        }

        internal static string HttpPost(string url, string content)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentLength = bytes.Length;
            //发送接口请求参数数据流
            var requestStream = httpWebRequest.GetRequestStream();
            //byte[] bytes = Encoding.UTF8.GetBytes(content);
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            //获取返回信息
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            var myResponseStream = response.GetResponseStream();

            string result = "";
            //读取返回流
            if (myResponseStream != null)
            {
                var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                result = myStreamReader.ReadToEnd();
                //关闭请求连接
                myStreamReader.Close();
                myResponseStream.Close();
            }
            return result;
        }

        internal static string HttpGet(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //设置HttpWebRequest头信息
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 600000;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Accept = "application/json";

            //获取响应输入流
            WebResponse webResponse = httpWebRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();

            string result = "";
            //读取返回流
            if (responseStream != null)
            {
                var myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                result = myStreamReader.ReadToEnd();

                //关闭请求连接  
                myStreamReader.Close();
                responseStream.Close();
            }
            return result;
        }

        internal static string SHA256Encrypt(string str)
        {
            StringBuilder pwd = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                byte[] s = hash.ComputeHash(Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < s.Length; i++)
                    pwd.Append(s[i].ToString("X2"));
            }
            return pwd.ToString();

        }

        internal static long GetUnixTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)Math.Round((nowTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
            return unixTime;
        }

        #region 辅助类
        public class CallArgs
        {
            /// <summary>
            /// 为空或 null 时将自动从 /appSettings/add[key=ProxyUrl|ApiUrl] 获取。
            /// </summary>
            public string Url { get; set; }
            public string Service { get; set; }
            public string Action { get; set; }
            public string Body { get; set; }
            /// <summary>
            /// 将会直接以 HTTP HEAD 的形式传递给 Proxy 的头，如<see cref="ClientInfo"/> 信息（形如 WebLoginUser、WebTrueLoginUser 等由 RIO 定义）。
            /// </summary>
            public Dictionary<string, string> HttpHeaders { get; set; }

            public bool ThrowException { get; set; }
            public bool DisableCache { get; set; }
            public int Timeout
            {
                get
                {
                    if (_timeout <= 0)
                        return DEFAULT_HTTP_TIMEOUT;

                    return _timeout;
                }
                set
                {
                    _timeout = value;
                }
            }
            private int _timeout;

            public bool ContextEnabled { get; set; }
            public bool SchemaEnabled { get; set; }

            /// <summary>
            /// 为空或 null 时将自动从 /appSettings/add[key=AppKey] 获取。
            /// </summary>
            public string AppKey { get; set; }


            /// <summary>
            /// 为空或 null 时将自动获取
            /// </summary>
            public string Nonce { get; set; }
            /// <summary>
            /// 为空或 null 时将自动计算签名
            /// </summary>
            public string Sign { get; set; }

            /// <summary>
            /// 调用者的标记，主要用于标记当前 HTTP 请求具体是由哪里发起的。
            /// </summary>
            public string Tag { get; set; }

            public bool GZip { get; set; }
        }

        [Serializable]
        public class HttpReq
        {
            /// <summary>
            /// 要请求的URL地址
            /// </summary>
            public string Url;

            /// <summary>
            /// 请求方法：GET/POST/PUT/DELETE...
            /// </summary>
            public string Method;


            /// <summary>
            /// 请求交互文件格式\编码："application/json; charset=UTF-8"
            /// </summary>
            public string ContentType = "application/json; charset=UTF-8";

            /// <summary>
            /// 请求交互文件格式： "application/json"
            /// </summary>
            public string Accept = "application/json";

            /// <summary>
            /// HEAD集合
            /// </summary>
            public IDictionary<string, string> Headers;

            /// <summary>
            /// 请求的超时时间，为“0”时将采用默认值。见：<seealso cref="System.Net.HttpWebRequest.Timeout"/>
            /// </summary>
            public int Timeout = DEFAULT_HTTP_TIMEOUT;

            /// <summary>
            /// 请求体正文
            /// </summary>
            public byte[] Body;

            /// <summary>
            /// 是否应跟随重定向响应。见：<seealso cref="System.Net.HttpWebRequest.AllowAutoRedirect"/>
            /// </summary>
            public bool AllowAutoRedirect = true;
        }


        #endregion
    }
}
