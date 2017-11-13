using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace Base.Utility
{
    /// <summary>
    /// 网络相关的工具类
    /// </summary>
    public class NetWorkHelper
    {
        private static void CreateDir(string FilePath)
        {
            FileInfo info = null;
            DirectoryInfo directory = null;
            try
            {
                info = new FileInfo(FilePath);
                if (info != null)
                {
                    directory = info.Directory;
                    if ((directory != null) && !directory.Exists)
                    {
                        directory.Create();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (directory != null)
                {
                    directory = null;
                }
                if (info != null)
                {
                    info = null;
                }
            }
        }

        private static string GetFilePathFromConfig()
        {
            string str = "/UpLoads/Xml/";
            if (ConfigurationManager.AppSettings["TmpXmlDir"] != null)
            {
                str = ConfigurationManager.AppSettings["TmpXmlDir"].Trim();
            }
            string str2 = Guid.NewGuid().ToString().Replace("-", "");
            string path = str + str2 + ".xml";
            CreateDir(HttpContext.Current.Server.MapPath(path));
            return path;
        }

        public static string GetPage(string PageUrl, RequestMethodEnum ReqMethod, EncodeingEnum encodeing)
        {
            return GetPage(PageUrl, ReqMethod, encodeing, null);
        }

        public static string GetPage(string PageUrl, RequestMethodEnum ReqMethod, EncodeingEnum encodeing, List<HttpParam> ParamList)
        {
            HttpWebResponse response;
            StreamReader reader;
            string s = "";
            if ((ParamList != null) && (ParamList.Count > 0))
            {
                for (int i = 0; i < ParamList.Count; i++)
                {
                    string str4;
                    if (i == 0)
                    {
                        if (PageUrl.IndexOf("?") != -1)
                        {
                            str4 = s;
                            s = str4 + "&" + ParamList[i].Name + "=" + HttpContext.Current.Server.UrlEncode(ParamList[i].Value);
                        }
                        else
                        {
                            str4 = s;
                            s = str4 + "?" + ParamList[i].Name + "=" + HttpContext.Current.Server.UrlEncode(ParamList[i].Value);
                        }
                    }
                    else
                    {
                        str4 = s;
                        s = str4 + "&" + ParamList[i].Name + "=" + HttpContext.Current.Server.UrlEncode(ParamList[i].Value);
                    }
                }
            }
            if (ReqMethod == RequestMethodEnum.POST)
            {
                if (s.Length > 0)
                {
                    s = s.Substring(1);
                }
            }
            else
            {
                PageUrl = PageUrl + s;
            }
            string str2 = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(PageUrl));
            if (ReqMethod != RequestMethodEnum.POST)
            {
                request.Method = "GET";
            }
            else
            {
                byte[] bytes;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                switch (encodeing)
                {
                    case EncodeingEnum.UTF8:
                        bytes = Encoding.UTF8.GetBytes(s);
                        break;

                    case EncodeingEnum.GB2312:
                        bytes = Encoding.GetEncoding("GB2312").GetBytes(s);
                        break;

                    default:
                        bytes = Encoding.UTF8.GetBytes(s);
                        break;
                }
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException exception)
            {
                response = (HttpWebResponse)exception.Response;
            }
            switch (encodeing)
            {
                case EncodeingEnum.UTF8:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    break;

                case EncodeingEnum.GB2312:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
                    break;

                default:
                    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    break;
            }
            str2 = reader.ReadToEnd();
            reader.Close();
            return str2;
        }

        public static string PostXmlData(string Url, EncodeingEnum encodeing, string xmlData)
        {
            Encoding encoding = Encoding.UTF8;
            switch (encodeing)
            {
                case EncodeingEnum.UTF8:
                    encoding = Encoding.UTF8;
                    break;

                case EncodeingEnum.GB2312:
                    encoding = Encoding.GetEncoding("GB2312");
                    break;

                default:
                    encoding = Encoding.UTF8;
                    break;
            }
            byte[] bytes = encoding.GetBytes(xmlData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "text/XML";
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException exception)
            {
                response = (HttpWebResponse)exception.Response;
            }
            string str = response.StatusCode.ToString();
            string statusDescription = response.StatusDescription;
            StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
            string str3 = reader.ReadToEnd();
            reader.Close();
            return str3;
        }

        public static string SaveRequestXmlData()
        {
            string filePathFromConfig = GetFilePathFromConfig();
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(HttpContext.Current.Request.InputStream);
                document.Save(HttpContext.Current.Server.MapPath(filePathFromConfig));
            }
            catch
            {
                filePathFromConfig = "";
            }
            return filePathFromConfig.ToLower();
        }

        public static bool SendMail(string subject, string body, string SmtpServer, string MailFrom, string SenderName, string UserAccount, string AccountPwd, List<string> MailTo, bool IsBodyHtml)
        {
            bool flag = false;
            if (SenderName.Trim() == "")
            {
                SenderName = MailFrom;
            }
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    message.SubjectEncoding = Encoding.GetEncoding("GB2312");
                    message.BodyEncoding = Encoding.GetEncoding("GB2312");
                    if (MailFrom.Trim() != "")
                    {
                        message.From = new MailAddress(MailFrom.Trim(), SenderName, Encoding.GetEncoding("GB2312"));
                    }
                    for (int i = 0; i < MailTo.Count; i++)
                    {
                        message.To.Add(new MailAddress(MailTo[i], MailTo[i], Encoding.GetEncoding("GB2312")));
                    }
                    message.IsBodyHtml = IsBodyHtml;
                    message.Subject = subject;
                    message.Body = body;
                    message.Sender = new MailAddress(UserAccount, SenderName, Encoding.GetEncoding("GB2312"));
                    message.Priority = MailPriority.High;
                    new SmtpClient(SmtpServer) { UseDefaultCredentials = true, Credentials = new NetworkCredential(UserAccount, AccountPwd) }.Send(message);
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        // Properties
        public static string ClientBrowser
        {
            get
            {
                try
                {
                    HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                    return (browser.Browser + " v." + browser.Version);
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string ClientCurrentShortUrl
        {
            get
            {
                string clientCurrentUrl = ClientCurrentUrl;
                if (clientCurrentUrl.IndexOf("?") != -1)
                {
                    int index = clientCurrentUrl.IndexOf("?");
                    return clientCurrentUrl.Substring(0, index);
                }
                return clientCurrentUrl;
            }
        }

        public static string ClientCurrentUrl
        {
            get
            {
                string str = null;
                try
                {
                    if (HttpContext.Current.Request.Url != null)
                    {
                        return HttpContext.Current.Request.Url.ToString().Trim().ToLower();
                    }
                    str = "";
                }
                catch
                {
                }
                return str;
            }
        }

        public static string ClientIP
        {
            get
            {
                //string userHostAddress = string.Empty;
                string userHostAddress = null;
                userHostAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                switch (userHostAddress)
                {
                    case null:
                        //case string.Empty:
                        userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        break;
                }
                if ((userHostAddress == null) || (userHostAddress == string.Empty))
                {
                    userHostAddress = HttpContext.Current.Request.UserHostAddress;
                }
                if ((userHostAddress == null) || (userHostAddress == string.Empty))
                {
                    return "0.0.0.0";
                }
                return userHostAddress;
            }
        }

        public static bool ClientIsSupportCookies
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.Browser.Cookies;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool ClientIsSupportFrames
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.Browser.Frames;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool ClientIsSupportJs
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.Browser.JavaScript;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool ClientIsSupportVbs
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.Browser.VBScript;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static string ClientMajorVersionOfBrowser
        {
            get
            {
                string str = null;
                try
                {
                    str = HttpContext.Current.Request.Browser.MajorVersion.ToString();
                }
                catch
                {
                }
                return str;
            }
        }

        public static string ClientPlatform
        {
            get
            {
                string str = null;
                try
                {
                    str = HttpContext.Current.Request.Browser.Platform.ToString();
                }
                catch
                {
                }
                return str;
            }
        }

        public static string ClientShortUrlReferrer
        {
            get
            {
                string clientUrlReferrer = ClientUrlReferrer;
                if (clientUrlReferrer.IndexOf("?") != -1)
                {
                    int index = clientUrlReferrer.IndexOf("?");
                    return clientUrlReferrer.Substring(0, index);
                }
                return clientUrlReferrer;
            }
        }

        public static string ClientUrlReferrer
        {
            get
            {
                string str = null;
                try
                {
                    if (HttpContext.Current.Request.UrlReferrer != null)
                    {
                        return HttpContext.Current.Request.UrlReferrer.ToString().Trim().ToLower();
                    }
                    str = "";
                }
                catch
                {
                }
                return str;
            }
        }

        public static string ClientUserAgent
        {
            get
            {
                string str = null;
                try
                {
                    str = HttpContext.Current.Request.UserAgent.ToString();
                }
                catch
                {
                }
                return str;
            }
        }

        public static string ClientUserHostName
        {
            get
            {
                string str = null;
                try
                {
                    str = HttpContext.Current.Request.UserHostName.ToString();
                }
                catch
                {
                }
                return str;
            }
        }

        public static string[] ClientUserLanguages
        {
            get
            {
                string[] userLanguages = new string[0];
                try
                {
                    userLanguages = HttpContext.Current.Request.UserLanguages;
                }
                catch
                {
                }
                return userLanguages;
            }
        }

        public static string ClientVersionOfBrowser
        {
            get
            {
                string str = null;
                try
                {
                    str = HttpContext.Current.Request.Browser.Version.ToString();
                }
                catch
                {
                }
                return str;
            }
        }


    }


    public class HttpParam
    {
        private string _ParamName = string.Empty;
        private string _Value = string.Empty;

        public string Name
        {
            get
            {
                return this._ParamName;
            }
            set
            {
                this._ParamName = value;
            }
        }

        public string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }


    }
    public enum EncodeingEnum
    {
        UTF8,
        GB2312
    }

    public enum RequestMethodEnum
    {
        POST,
        GET
    }
}
