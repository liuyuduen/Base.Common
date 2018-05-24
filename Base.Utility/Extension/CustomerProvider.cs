using Base.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LeaRun.Utilities.Base.Extension
{
    public class CustomerProvider : ICustomerProvider
    {
        #region 静态实例
        /// <summary>当前提供者</summary>
        public static ICustomerProvider Provider
        {
            get { return new CustomerProvider(); }
        }
        #endregion

        /// <summary>
        /// 秘钥
        /// </summary>
        private string LoginUserKey = "LoginCustomer";
        /// <summary>
        /// 登陆提供者模式:Session、Cookie 
        /// </summary>
        private string LoginProvider = ConfigHelper.AppSettings("LoginProvider");
        /// <summary>
        /// 写入登录信息
        /// </summary>
        /// <param name="user">成员信息</param>
        public virtual void AddCurrent(ICustomer user)
        {
            try
            {

                if (LoginProvider == "Cookie")
                {
                    CookieHelper.SetCookie(LoginUserKey, DESEncryptHelper.Encrypt(JsonConvert.SerializeObject(user)), 1440);
                }
                else
                {
                    SessionHelper.Add(LoginUserKey, DESEncryptHelper.Encrypt(JsonConvert.SerializeObject(user)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public virtual ICustomer Current()
        {
            try
            {
                ICustomer user = new ICustomer();
                if (LoginProvider == "Cookie")
                {
                    user = JsonConvert.DeserializeObject<ICustomer>(DESEncryptHelper.Decrypt(CookieHelper.GetCookie(LoginUserKey)));
                }
                else
                {
                    user = JsonConvert.DeserializeObject<ICustomer>(DESEncryptHelper.Decrypt(SessionHelper.Get(LoginUserKey).ToString()));
                }
                //if (user == null)
                //{
                //    throw new Exception("登录信息超时，请重新登录。");
                //}
                return user;
            }
            catch
            {
                throw new Exception("登录信息超时，请重新登录。");
            }
        }
        /// <summary>
        /// 删除登录信息
        /// </summary>
        public virtual void EmptyCurrent()
        {
            if (LoginProvider == "Cookie")
            {
                HttpCookie objCookie = new HttpCookie(LoginUserKey.Trim());
                objCookie.Expires = DateTime.Now.AddYears(-5);
                HttpContext.Current.Response.Cookies.Add(objCookie);
            }
            else
            {
                SessionHelper.Remove(LoginUserKey.Trim());
            }
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns>True:过期</returns>
        public virtual bool IsOverdue()
        {
            object str = "";
            if (LoginProvider == "Cookie")
            {
                str = CookieHelper.GetCookie(LoginUserKey);
            }
            else
            {
                str = SessionHelper.Get(LoginUserKey);
            }

            return !(str != null && str.ToString() != "");
        }
    }
}
