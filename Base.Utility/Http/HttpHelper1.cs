using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Base.Utility
{
    public class HttpHelper1
    {

        static string smartProxyToken = ConfigHelper.AppSettings("SmartProxyToken");

        internal static string HttpPostForSmartProxy(string url, string content)
        {
            HttpWebResponse response = null;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                byte[] bytes = Encoding.UTF8.GetBytes(content);

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=UTF-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.ContentLength = bytes.Length;
                long timestamp = GetUnixTimeStamp();
                httpWebRequest.Headers.Add("timestamp", timestamp.ToString());
                string signature = SHA256Encrypt(timestamp + smartProxyToken + timestamp);
                httpWebRequest.Headers.Add("signature", signature);

                //发送接口请求参数数据流
                var requestStream = httpWebRequest.GetRequestStream();
                //byte[] bytes = Encoding.UTF8.GetBytes(content);
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();

                //获取返回信息
                //Utils.WriteLog("Signate", new string[] { timestamp.ToString(), timestamp + smartProxyToken + timestamp, signature });
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
         
        internal static string HttpPostForSmartProxy(string url, byte[] bytes, WebHeaderCollection headers)
        {
            HttpWebResponse response = null;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                //byte[] bytes = Encoding.UTF8.GetBytes(content);

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/html; charset=utf-8";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.ContentLength = bytes.Length;
                long timestamp = GetUnixTimeStamp();
                httpWebRequest.Headers.Add("timestamp", timestamp.ToString());
                string signature = SHA256Encrypt(timestamp + smartProxyToken + timestamp);
                httpWebRequest.Headers.Add("signature", signature);

                httpWebRequest.Headers.Add(headers);
                //Utils.WriteLog("Signate", new string[] { timestamp.ToString(), timestamp + smartProxyToken + timestamp, signature });
                //发送接口请求参数数据流
                var requestStream = httpWebRequest.GetRequestStream();
                //byte[] bytes = Encoding.UTF8.GetBytes(content);
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();

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
                throw new Exception("HttpPost异常：\r\n" + errMsg);

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

    }
}
