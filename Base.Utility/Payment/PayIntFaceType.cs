using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Base.Utility
{
    public class PayIntFaceType
    {

         
        /// <summary>
        /// 获取支付提交路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetPaymentURL(string url)
        {
            string payIntfaceURL = string.Empty;
            payIntfaceURL = ConfigHelper.AppSettings(url);
            return payIntfaceURL;
        }
        /// <summary>
        /// 后台通知地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetNotify_Url(string notify_url)
        {
            string notifyUrl = string.Empty;
            notifyUrl = ConfigHelper.AppSettings(notify_url);
            return notifyUrl;
        }
        /// <summary>
        /// 页面跳转同步通知地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetReturn_Url(string return_url)
        {
            string returnUrl = string.Empty;
            returnUrl = ConfigHelper.AppSettings(return_url);
            return returnUrl;
        }
        
        /// <summary>
        /// 反射类支付方式名称
        /// </summary>
        /// <param name="pd_FrpId">所选银行</param>
        /// <returns></returns>
        public static string getPaymentName(string paymentType)
        {
            string paymentName = string.Empty;
            switch (paymentType)
            {
                case "1":
                    paymentName = "PaymentHX";
                    break;
                case "2":
                    paymentName = "PaymentHC";
                    break;
                case "3":
                    paymentName = "PaymentYB";
                    break;
                case "4":
                    paymentName = "PaymentZL";
                    break;
                case "5":
                    paymentName = "PaymentMob";
                    break;
                case "7":
                    paymentName = "PaymentHX7";
                    break;
                case "8":
                    paymentName = "PaymentZL";
                    break;
                case "9":
                    paymentName = "PaymentHFB";
                    break;
                case "10":
                    paymentName = "Payment19Pay";
                    break;
                default:
                    paymentName = "PaymentHFB";
                    break;

            }
            return paymentName;
        } 

    }

    public class ResultCallBack
    {
        public bool isResponse { get; set; }
        public string responseText { get; set; }
        public string errorMsg { get; set; }
        public string succMsg { get; set; }
        public string acount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isresponse">是否回发</param>
        /// <param name="responseText">回发代码</param>
        /// <param name="errorMsg">错误信息</param>
        /// <param name="succMsg">成功信息</param>
        /// <param name="acount">金额</param>
        public ResultCallBack(bool isresponse, string responseText, string errorMsg, string succMsg, string acount)
        {
            this.isResponse = isresponse;
            this.responseText = responseText;
            this.errorMsg = errorMsg;
            this.succMsg = succMsg;
            this.acount = acount;
        }
    }


}
