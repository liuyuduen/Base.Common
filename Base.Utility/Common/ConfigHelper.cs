using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;

namespace Base.Utility
{
    /// <summary>
    /// 配置文件辅助类 初始化所有配置信息
    /// </summary>
    public class ConfigHelper
    {
        #region 配置文件名、定义连接字符串常量

        /// 数据库类型 
        private static readonly string DB_TYPE = "ComponentDbType";
        /// <summary>
        /// 数据连接字符Name
        /// </summary>
        public static readonly string DB_CONNECTION_STRING = "DbConnectionString";

        /// 数据字符串是否加密  True or False 
        private static readonly string DB_CONNECTION_STRING_DESENCRYPT = "DbConnectionString_DESEncrypt";

        /// 登陆提供者模式:Session、Cookie  
        private static readonly string LOGIN_PROVIDER = "LoginProvider";

        /// 是否启用缓存 
        private static readonly string ISCACHE = "IsCache";

        ///服务器缓存设置时间（分钟）  
        private static readonly string CACHE_TIME = "CacheTime";
        ///服务器缓存设置时间（分钟）  
        private static readonly string CACHE_DB_NAME = "CacheDBName";


        ///IOC配置文件路径
        private static readonly string IOC_CONFIG_PAHT = "IocConfigPath";

        ///系统appSettings配置文件路径
        private static readonly string System_Config_Path = "SystemConfigPath";

        private static readonly string WEBSERVICEBINDINGTYPE = "WebServiceBindingType";


        //定义电子邮箱用户名
        private static readonly string EMAIL_USER_NAME = "EmailUserName";
        // 定义电子邮箱密码
        private static readonly string EMAIL_USER_PASSWORD = "EmailUserPassword";
        // 定义电子邮箱地址
        private static readonly string EMAIL_ADDRESS = "EmailAddress";
        // 定义SMTP服务器
        private static readonly string SMTP_SERVER = "SmtpServer";
        // 定义系统名称
        private static readonly string SYS_NAME = "SysName";

        #endregion

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        public static readonly string DbConnectionString;

        /// <summary>
        /// 数据库的类型
        /// </summary>
        public static readonly DatabaseType DBType;
        /// <summary>
        /// 字符串是否加密
        /// </summary>
        public static readonly string ConStringDESEncrypt;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static readonly SQLDbType SQLDbType;
        /// <summary>
        /// 登陆者提供模式
        /// </summary>
        public static readonly string LoginProvider;
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public static readonly string IsCache;
        /// <summary>
        /// 服务器缓存设置时间（分钟）  
        /// </summary>
        public static readonly string CacheTime;
        /// <summary>
        /// 缓存数据库 数据库中设置缓存table
        /// </summary>
        public static readonly string CacheDBName;

        /// <summary>
        /// IOC配置文件路径
        /// </summary>
        public static readonly string IocConfigPath;

        public static readonly string WebServiceBindingType;

        /// <summary>
        /// 获得电子邮箱用户名
        /// </summary>
        public static readonly string EmailUserName;
        /// <summary>
        /// 获得电子邮箱密码
        /// </summary>
        public static readonly string EmailUserPassword;
        /// <summary>
        /// 获得电子邮箱地址
        /// </summary>
        public static readonly string EmailAddress;
        /// <summary>
        /// 获得SMTP服务器
        /// </summary>
        public static readonly string SmtpServer;
        /// <summary>
        /// 获得系统名称
        /// </summary>
        public static readonly string SysName;

        /// <summary>
        /// 系统ID
        /// </summary>
        public static readonly string SystemID;
        /// <summary>
        /// 初始化配置信息
        /// </summary>
        static ConfigHelper()
        {
            // 读取连接字符串
            DbConnectionString = ConnectionStrings(DB_CONNECTION_STRING);

            string dbType = AppSettings(DB_TYPE);
            if (!string.IsNullOrEmpty(dbType))
            {
                DBType = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType);
                //SqlDbType = (SQLDbType)Enum.Parse(typeof(SQLDbType), AppSettings(DB_TYPE));
            }
            else
            {
                DBType = DatabaseType.SqlServer;
                // 默认为SQLServer2005
                // SqlDbType = SQLDbType.SQL2005;
            }

            ConStringDESEncrypt = AppSettings(DB_CONNECTION_STRING_DESENCRYPT);
            IsCache = AppSettings(ISCACHE) == "" ? "false" : AppSettings(ISCACHE);
            CacheTime = AppSettings(CACHE_TIME);
            CacheDBName = AppSettings(CACHE_DB_NAME);
            IocConfigPath = AppSettings(IOC_CONFIG_PAHT);

            // 读取邮件相关
            EmailUserName = AppSettings(EMAIL_USER_NAME);
            EmailUserPassword = AppSettings(EMAIL_USER_PASSWORD);
            EmailAddress = AppSettings(EMAIL_ADDRESS);
            SmtpServer = AppSettings(SMTP_SERVER);
            // 读取应用程序设置
            SysName = AppSettings(SYS_NAME);

            WebServiceBindingType = AppSettings(WEBSERVICEBINDINGTYPE);
        }

        /// <summary>
        /// 读取AppSettings
        /// </summary>
        public static string AppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? String.Empty;
        }

        /// <summary>
        /// 读取连接字符串
        /// </summary>
        public static string ConnectionStrings(string connStringName)
        {
            ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings[connStringName];

            return connStringSettings != null ? connStringSettings.ConnectionString : String.Empty;
        }

        /// <summary>
        /// 根据Key修改Value
        /// </summary>
        /// <param name="key">要修改的Key</param>
        /// <param name="value">要修改为的值</param>
        public static void SetValue(string key, string value)
        {
            try
            {
                // XmlHelper.SetInnerText("/Configuration/Config.xml", key,value);

                System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                xDoc.Load(HttpContext.Current.Server.MapPath("/Configuration/system.config"));//如："/Configuration/Config.xml"
                System.Xml.XmlNode xNode;
                System.Xml.XmlElement xElem1;
                System.Xml.XmlElement xElem2;
                xNode = xDoc.SelectSingleNode("//appSettings");

                xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
                if (xElem1 != null) xElem1.SetAttribute("value", value);
                else
                {
                    xElem2 = xDoc.CreateElement("add");
                    xElem2.SetAttribute("key", key);
                    xElem2.SetAttribute("value", value);
                    xNode.AppendChild(xElem2);
                }
                xDoc.Save(HttpContext.Current.Server.MapPath("/Configuration/system.config"));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
    /// <summary>
    /// SQLServer数据库的类型
    /// </summary>
    public enum SQLDbType
    {
        SQL2000,
        SQL2005,
        SQL2008
    }
}
