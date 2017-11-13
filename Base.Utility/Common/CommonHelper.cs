using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using Base.Utility;
using System.Diagnostics;
using System.Net;

namespace Base.Utility
{
    /// <summary>
    /// 功能：网站工具类
    /// </summary>
    public class CommonHelper
    {


        #region "全球唯一码GUID"
        /// <summary>
        /// 获取一个全球唯一码GUID字符串
        /// </summary>
        public static string GetGuid
        {
            get
            {
                return Guid.NewGuid().ToString().ToLower();
            }
        }
        #endregion

        #region 自动生成日期编号
        /// <summary>
        /// 自动生成编号  201008251145409865
        /// </summary>
        /// <returns></returns>
        public static string CreateNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString(); //生成编号 
            string code = DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;//形如
            return code;
        }
        #endregion

        #region 计时器
        /// <summary>
        /// 计时器开始
        /// </summary>
        /// <returns></returns>
        public static Stopwatch TimerStart()
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            return watch;
        }
        /// <summary>
        /// 计时器结束
        /// </summary>
        /// <param name="watch"></param>
        /// <returns></returns>
        public static string TimerEnd(Stopwatch watch)
        {
            watch.Stop();
            double costtime = watch.ElapsedMilliseconds;
            return costtime.ToString();
        }
        #endregion

        #region 获取本机的计算机名
        /// <summary>
        /// 获取本机的计算机名
        /// </summary>
        public static string LocalHostName
        {
            get
            {
                return Dns.GetHostName();
            }
        }
        #endregion


        /// <summary> 
        /// Description:获取下单来源（智能手机、电脑） 
        /// </summary>
        /// <returns></returns>
        public static short CheckUserAgent()
        {
            short agentId = 0;
            string userAgent = HttpContext.Current.Request.UserAgent;

            agentId = FormatHelper.ChangeInputType<short>(EnumHelper.ToEnum<UserAgent>(userAgent));

            //foreach (uint val in Enum.GetValues(typeof(UserAgent)))
            //{
            //    var enumName = Enum.GetName(typeof(UserAgent), val);
            //    if (enumName != null)
            //    {
            //        string name = enumName.Replace("_", " ");
            //        if (userAgent != null)
            //        {
            //            if (userAgent.Contains(name))
            //            {
            //                agentId = FormatHelper.ChangeInputType<short>(val);
            //                break;
            //            }
            //        }
            //    }
            //}

            return agentId;
        }

        /// <summary>    
        /// Description:获取大写不带分割线的Guid
        /// </summary>  
        /// <returns></returns>  
        public static string GuidNoLine()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        /// <summary>    
        /// Description:根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0}{1:X}", DateTime.Now.ToString("yyMMdd"), i - DateTime.Now.Ticks);
        }

        /// <summary>    
        /// Description:根据GUID获取19位的唯一数字序列
        /// </summary>
        /// <returns></returns>  
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }


     
    }


}
