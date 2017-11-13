using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Data.SqlClient;

namespace Base.Utility
{
    public class Log
    {
          
        // Fields 
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Methods
        public static void Debug(string strMsg)
        {
            log.Debug(strMsg);
        }

        public static void Debug(string strMsg, Exception objErr)
        {
            log.Debug(strMsg, objErr);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="strMsg">自定义错误信息</param>
        public static void Error(string strMsg)
        {
            log.Error(strMsg);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="errmsg">自定义错误信息</param>
        /// <param name="objErr">捕捉到的异常</param>
        public static void Error(string errmsg, Exception objErr)
        {
            log.Error(errmsg, objErr);
        }

        private static string Exception2String(Exception objErr)
        {
            return ("<b>Error Caught in Page_Error event</b><hr><br><br><b>Error in:</b>" + objErr.Source.ToString() + "<br><b>Error Message:</b>" + objErr.Message.ToString() + "<br><b>Stack Trace:</b><br>" + objErr.StackTrace.ToString());
        }

        public static void Fatal(string strMsg)
        {
            log.Fatal(strMsg);
        }

        public static void Fatal(string errmsg, Exception objErr)
        {
            log.Fatal(errmsg, objErr);
        }

        public static void Info(string strMsg)
        {
            log.Info(strMsg);
        }

        public static void Info(string strMsg, Exception objErr)
        {
            log.Info(strMsg, objErr);
        }

        public static void Warn(string strMsg)
        {
            log.Warn(strMsg);
        }

        public static void Warn(string errmsg, Exception objErr)
        {
            log.Warn(errmsg, objErr);
        }
    }
}
