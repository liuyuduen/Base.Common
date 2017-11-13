using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Base.Utility
{
    public class IpDetail
    {

        public String Ret { get; set; }



        public String Start { get; set; }



        public String End { get; set; }



        public String Country { get; set; }



        public String Province { get; set; }



        public String City { get; set; }



        public String District { get; set; }



        public String Isp { get; set; }



        public String Type { get; set; }



        public String Desc { get; set; }

    }



    public static class IpHelper
    {

        /// <summary>  

        /// 获取IP地址的详细信息，调用的借口为  

        /// http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip={ip}  

        /// </summary>  

        /// <param name="ipAddress">请求分析得IP地址</param>  

        /// <param name="sourceEncoding">服务器返回的编码类型</param>  

        /// <returns>IpUtils.IpDetail</returns>  

        public static IpDetail Get(String ipAddress, Encoding sourceEncoding)
        {

            String ip = string.Empty;

            if (sourceEncoding == null)

                sourceEncoding = Encoding.UTF8;

            using (var receiveStream = WebRequest.Create("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip=" + ipAddress).GetResponse().GetResponseStream())
            {

                using (var sr = new StreamReader(receiveStream, sourceEncoding))
                {

                    var readbuffer = new char[1000];

                    int n = sr.Read(readbuffer, 0, readbuffer.Length);

                    int realLen = 0;

                    while (n > 0)
                    {

                        realLen = n;

                        n = sr.Read(readbuffer, 0, readbuffer.Length);

                    }

                    ip = sourceEncoding.GetString(sourceEncoding.GetBytes(readbuffer, 0, realLen));

                }

            }

            return !string.IsNullOrEmpty(ip) ? new JavaScriptSerializer().Deserialize<IpDetail>(ip) : null;

        }

        /// <summary>
        /// 将IP转换成为10进制的数字
        /// </summary>
        /// <param name="strIP">192.168.1.1类型的IP</param>
        /// <returns></returns>
        public static long IPToNumber(string strIP)
        {
            long lgIp = 0;
            try
            {
                string[] strArrayIP = strIP.Split('.');
                long lgIpOne = Convert.ToInt64(string.IsNullOrEmpty(strArrayIP[0]) ? "0" : strArrayIP[0]) * 256 * 256 * 256;
                long lgIpTwo = Convert.ToInt64(string.IsNullOrEmpty(strArrayIP[1]) ? "0" : strArrayIP[1]) * 256 * 256;
                long lgIpThree = Convert.ToInt64(string.IsNullOrEmpty(strArrayIP[2]) ? "0" : strArrayIP[2]) * 256;
                long lgIpFour = Convert.ToInt64(string.IsNullOrEmpty(strArrayIP[3]) ? "0" : strArrayIP[3]) * 1;
                lgIp = lgIpOne + lgIpTwo + lgIpThree + lgIpFour;

            }
            catch (Exception)
            {

                return lgIp;
            }

            return lgIp;
        }

        /// <summary>
        /// 将10进制数字转换成为IP
        /// </summary>
        /// <param name="lgNumber">10进制类型的IP，例如：974368566</param>
        /// <returns></returns>
        public static string NumberToIP(string lgNumber)
        {
            return System.Net.IPAddress.Parse(lgNumber).ToString();
        }

         
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebClientIp()
        {

            const string userIp = "未获取用户IP";
            try
            {
                if (System.Web.HttpContext.Current == null
                    || System.Web.HttpContext.Current.Request == null
                    || System.Web.HttpContext.Current.Request.ServerVariables == null)
                    return "";

                string CustomerIP = "";

                //CDN加速后取到的IP 
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(CustomerIP))
                    return CustomerIP;

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                }

                if (string.Compare(CustomerIP, "unknown", true) == 0)
                    return System.Web.HttpContext.Current.Request.UserHostAddress;

                return CustomerIP == "::1" ? "127.0.0.1" : CustomerIP;
            }
            catch
            {
            }

            return userIp;

        }
    }

}
