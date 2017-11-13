using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Base.Utility
{
    /// <summary>
    /// 邮件发送工具类
    /// </summary>
    public class MailHelper
    {


        /// <summary>
        /// 客服部邮件发送（最高优先级发送，UTF8编码，Html邮件格式）
        /// </summary>
        /// <param name="strMailTo">收件人邮箱</param>
        /// <param name="strMailTitle">邮件主题</param>
        /// <param name="strMailContent">邮件正文</param>
        public static void SendMailBySevice(string strMailTo, string strMailTitle, string strMailContent, string strServiceMailFrom, string ServiceMailPass, string strHost, string strSmtpPost, string strEnableSsl, string strUseDefaultCredentials)
        {
            //NormalConfig config = NormalConfig.Instance;

            //SendMail(strMailTo, strMailTitle, strMailContent, MailPriority.High, true, Encoding.UTF8, config.ServiceMailFrom, EncryptTool.Decode(config.ServiceMailPass));
            SendMail(strMailTo, strMailTitle, strMailContent, MailPriority.High, true, Encoding.UTF8, strServiceMailFrom, ServiceMailPass, strHost, strSmtpPost, strEnableSsl, strUseDefaultCredentials);
        }



        /// <summary>
        /// 邮件发送（核心方法）
        /// </summary>
        /// <param name="strMailTo">收件人邮箱</param>
        /// <param name="strMailTitle">邮件主题</param>
        /// <param name="strMailContent">邮件正文</param>
        /// <param name="Priority">发送优先级</param>
        /// <param name="boolIsBodyHtml">是否Html格式</param>
        /// <param name="coding">编码</param>
        /// <param name="strMailFrom">发件人的邮箱</param>
        /// <param name="strMailPass">发件人的密码</param>
        public static void SendMail(string strMailTo, string strMailTitle, string strMailContent, MailPriority Priority, bool boolIsBodyHtml, Encoding coding, string strMailFrom, string strMailPass, string strHost, string strSmtpPort, string strEnableSsl, string strUseDefaultCredentials)
        {
            #region 加载配置

            // NormalConfig config = NormalConfig.Instance;

            //string host = config.SmtpHost;//发信人所用邮箱的服务器

            //int port = Convert.ToInt32(config.SmtpPort);//发邮件的端口

            //bool enableSSl = (config.EnableSsl == "1");//是否使用SSL连接,0=false,1=true，一般设为fasle

            //bool useDefaultCredentials = (config.UseDefaultCredentials == "1");//一般设为0,0表示不发送身份严正信息(0=false,1=true)
            string host = strHost;//发信人所用邮箱的服务器

            int port = Convert.ToInt32(strSmtpPort);//发邮件的端口

            bool enableSSl = (strEnableSsl == "1");//是否使用SSL连接,0=false,1=true，一般设为fasle

            bool useDefaultCredentials = (strUseDefaultCredentials == "1");//一般设为0,0表示不发送身份严正信息
            #endregion

            #region 用自己的服务器发邮件

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;//指定如何处理待发的邮件：通过SMTP服务器发送
            client.EnableSsl = enableSSl;
            client.Host = host;
            client.Port = port;
            client.UseDefaultCredentials = useDefaultCredentials;
            client.Credentials = new NetworkCredential(strMailFrom, strMailPass);

            MailMessage mm = new MailMessage();
            mm.Priority = Priority;//优先级，如：MailPriority.High
            mm.From = new MailAddress(strMailFrom);//发件人的邮箱
            mm.Sender = new MailAddress(strMailFrom);//发件人的邮箱
            mm.To.Add(new MailAddress(strMailTo));//收件人邮件帐号
            mm.Subject = strMailTitle; //邮件标题 
            mm.Body = strMailContent;//邮件内容
            mm.IsBodyHtml = boolIsBodyHtml;//是否Html格式
            mm.BodyEncoding = coding;//编码，如：Encoding.UTF8

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

        }
        #region 发送电子邮件

        private static bool isSucceed;

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="cc">抄送人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文内容</param>
        /// <param name="mode">方式</param>
        /// <returns>true，成功；false，失败</returns>
        public static bool SendMail(string to, string cc, string subject, string body, IsHtmlFormat mode)
        {
            return SendMail(ConfigHelper.EmailAddress, to, cc, subject, body, mode);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="cc">抄送人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文内容</param>
        /// <param name="mode">方式</param>
        /// <param name="files">附件</param>
        /// <returns>true，成功；false，失败</returns>
        public static bool SendMail(string to, string cc, string subject, string body, IsHtmlFormat mode, params string[] files)
        {
            return SendMail(ConfigHelper.EmailAddress, to, cc, subject, body, mode, files);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="from">发件人</param>
        /// <param name="to">收件人</param>
        /// <param name="cc">抄送人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文内容</param>
        /// <param name="mode">方式</param>
        /// <param name="files">附件</param>
        /// <returns>true，成功；false，失败</returns>
        public static bool SendMail(string from, string to, string cc, string subject, string body, IsHtmlFormat mode, params string[] files)
        {
            try
            {
                // 创建电子邮件
                MailMessage mail = new MailMessage();

                // 设置发件人
                mail.From = new MailAddress(from);
                // 设置收件人(逗号分隔)
                if (to != "")
                {
                    string[] tos = to.Split(',');
                    foreach (string t in tos)
                    {
                        // 添加多个收件人
                        mail.To.Add(new MailAddress(t));
                    }
                }
                // 设置抄送人(逗号分隔)
                if (cc != "")
                {
                    string[] ccs = cc.Split(',');
                    foreach (string c in ccs)
                    {
                        // 添加多个抄送人
                        mail.CC.Add(new MailAddress(c));
                    }
                }
                // 设置主题
                mail.Subject = subject;
                // 设置正文内容
                mail.Body = body;
                // 设置邮件格式
                mail.IsBodyHtml = (mode == IsHtmlFormat.Yes);
                // 设置附件
                if (files.Length > 0)
                {
                    foreach (string f in files)
                    {
                        mail.Attachments.Add(new Attachment(f));
                    }
                }

                // 创建邮件服务器类
                SmtpClient smtp = new SmtpClient();
                // 设置SMTP服务器
                // 一般服务器名称为smtp+邮件后缀
                // 如：cj@163.com的服务器地址为：smtp.163.com
                if (String.IsNullOrEmpty(ConfigHelper.SmtpServer))
                {
                    smtp.Host = "smtp." + from.Substring(from.IndexOf("@") + 1);
                }
                else
                {
                    smtp.Host = ConfigHelper.SmtpServer;
                }
                // 设置SMTP的端口
                smtp.Port = 25;
                // 设置服务器的用户名和密码
                smtp.Credentials = new NetworkCredential(
                    ConfigHelper.EmailUserName, ConfigHelper.EmailUserPassword);

                //smtp.SendCompleted += new SendCompletedEventHandler(smtp_SendCompleted);

                // 发送邮件
                smtp.SendAsync(mail, String.Empty);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        static void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                isSucceed = false;
            }
            isSucceed = true;
        }

        #endregion
    }

    /// <summary>
    /// 是否为Html格式
    /// </summary>
    public enum IsHtmlFormat
    {
        Yes,
        No
    }
}
