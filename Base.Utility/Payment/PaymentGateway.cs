using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.IO;
using Base.Utility;

namespace Base.Utility
{
    public abstract class PaymentGateway
    {
        /// <summary>
        /// 在线支付接口
        /// </summary>
        /// <param name="bankType">支付银行</param>
        /// <param name="billNO">订单号</param>
        /// <param name="billAmount">订单金额</param>
        /// <param name="product_name">商品名称</param>
        /// <param name="product_remark">订单备注</param>
        /// <param name="strHomeUrl">提交路径</param>
        public abstract string PayOnline(string bankType, string billNO, string billAmount, string product_name, string product_remark, string strHomeUrl);

        /// <summary>
        ///  支付回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract ResultCallBack RechargeCallBack();


    }

    /// <summary>
    /// 汇付宝在线支付 
    /// </summary>
    public class PaymentHFB : PaymentGateway
    {


        /// <summary>
        /// 在线支付接口
        /// </summary>
        /// <param name="bankType">支付银行</param>
        /// <param name="billNO">订单号</param>
        /// <param name="billAmount">订单金额</param>
        /// <param name="product_name">商品名称</param>
        /// <param name="product_remark">订单备注</param>
        /// <param name="strHomeUrl">提交路径</param>
        public override string PayOnline(string bankType, string billNO, string billAmount, string product_name, string product_remark, string strHomeUrl)
        {
            //页面提交参数
            Dictionary<string, string> payData = new Dictionary<string, string>();
            StringBuilder sbHtml = new StringBuilder();


            string merhantID = ConfigHelper.AppSettings("hfb_merhantID");
            string merhantKey = ConfigHelper.AppSettings("hfb_merhantKey");

            string agent_id = string.Empty;//商家号
            string agent_bill_id = string.Empty;//商家单号
            string agent_bill_time = string.Empty;//下单时间

            string pay_type = string.Empty;//支付类型
            string pay_code = string.Empty;//支付代码
            string pay_amt = string.Empty;//支付金额
            string goods_name = string.Empty;//商品名

            string notify_url = string.Empty;//返回路径
            string return_url = string.Empty;//返回路径
            string user_ip = string.Empty;//用户IP
            string sign = string.Empty;//签名
            string is_phone = string.Empty;//手机 
            string is_frame = string.Empty;//网页


            string md5src;

            #region 汇付宝接口


            #region 汇付宝银行名称对应银行ID

            switch (bankType)
            {
                case "1": //中国工商
                    pay_code = "001";
                    break;
                case "2": //中国建设
                    pay_code = "003";
                    break;
                case "3": //中国农业
                    pay_code = "005";
                    break;
                case "4": //中国银行
                    pay_code = "004";
                    break;
                case "5": //中国邮政
                    pay_code = "020";
                    break;
                case "6": //中国光大
                    pay_code = "010";
                    break;
                case "7": //招商银行
                    pay_code = "002";
                    break;
                case "8": //广发银行
                    pay_code = "016";
                    break;
                case "9": //中国民生
                    pay_code = "013";
                    break;
                case "11": //浦发银行
                    pay_code = "007";
                    break;
                case "12": //中信银行
                    pay_code = "015";
                    break;
                case "13": //交通银行
                    pay_code = "006";
                    break;
                case "14": //兴业银行
                    pay_code = "011";
                    break;
                case "17": //北京银行
                    pay_code = "045";
                    break;
                case "31"://微信
                    pay_code = "131";
                    break;
                case "32"://支付宝
                    pay_code = "132";
                    break;
                default:
                    pay_code = "1";
                    break;
            }
            #endregion

            pay_type = "20";

            if (pay_code == "131")
            {
                //微信付款
                pay_code = "";
                pay_type = "30";
                is_phone = "0";// rechargeMark == "4" ? 1 : 0;
                is_frame = "0";
            }
            else if (pay_code == "132")
            {
                //支付宝
                pay_code = "";
                pay_type = "22";
                is_phone = "0";
                is_frame = "0";
            }


            //后台通知地址(必填)
            notify_url = PayIntFaceType.GetNotify_Url("hfb_notify_url");
            return_url = PayIntFaceType.GetNotify_Url("hfb_return_url");
            pay_amt = billAmount;
            agent_id = merhantID;  //商家号（必填） 
            agent_bill_id = billNO;//商家定单号(必填)
            agent_bill_time = DateTime.Now.ToString("yyyyMMddHHmmss");

            //商品名称（必填） 
            goods_name = HttpUtility.UrlEncode(product_name);

            //页面跳转同步通知地址(选填)

            user_ip = IpHelper.GetWebClientIp();
            StringBuilder _StringSign = new StringBuilder();
            //注意拼接顺序,详情请看《汇付宝即时到帐接口开发指南2.0.4.pdf》
            _StringSign.Append("version=1")
                .Append("&agent_id=" + agent_id)
                .Append("&agent_bill_id=" + agent_bill_id)
                .Append("&agent_bill_time=" + agent_bill_time)
                .Append("&pay_type=" + pay_type)
                .Append("&pay_amt=" + pay_amt)
                .Append("&notify_url=" + notify_url)
                .Append("&return_url=" + return_url)
                .Append("&user_ip=" + user_ip);
            _StringSign.Append("&key=" + merhantKey);

            md5src = _StringSign.ToString();
            sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(md5src, "MD5");



            payData.Clear();
            payData.Add("version", "1");
            payData.Add("agent_id", agent_id);
            payData.Add("agent_bill_id", agent_bill_id);
            payData.Add("agent_bill_time", agent_bill_time);
            payData.Add("pay_type", pay_type);
            payData.Add("pay_code", pay_code);
            payData.Add("pay_amt", pay_amt);

            payData.Add("notify_url", notify_url);
            payData.Add("return_url", return_url);
            payData.Add("user_ip", user_ip);
            payData.Add("goods_name", goods_name);
            payData.Add("goods_num", "1");

            payData.Add("goods_note", goods_name);
            payData.Add("is_test", "0");
            payData.Add("remark", product_remark);
            payData.Add("sign", sign);
            if (pay_type == "20")
            {
                payData.Add("is_phone", is_phone);
                payData.Add("is_frame", is_frame);
            }
            #endregion 汇付宝接口

            sbHtml.Length = 0;
            foreach (KeyValuePair<string, string> temp in payData)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }


            return sbHtml.ToString();
        }


        /// <summary>
        /// 汇付宝支付回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ResultCallBack RechargeCallBack()
        {
            string strTipMsg;
            bool isResponse = false;
            string responseText = "";
            string errorMsg = string.Empty;
            string succMsg = string.Empty;
            #region 汇付宝


            string merhantID = ConfigHelper.AppSettings("hfb_merhantID");
            string merhantKey = ConfigHelper.AppSettings("hfb_merhantKey");

            string result = HttpContext.Current.Request["result"];
            string pay_message = HttpContext.Current.Request["pay_message"];
            string agent_id = HttpContext.Current.Request["agent_id"];
            string jnet_bill_no = HttpContext.Current.Request["jnet_bill_no"];
            string agent_bill_id = HttpContext.Current.Request["agent_bill_id"];
            string pay_type = HttpContext.Current.Request["pay_type"];
            string pay_amt = HttpContext.Current.Request["pay_amt"];
            string remark = HttpContext.Current.Request["remark"];
            string returnSign = HttpContext.Current.Request["sign"];

            StringBuilder sbSign = new StringBuilder();
            sbSign.Append("result=" + result)
                .Append("&agent_id=" + agent_id)
                .Append("&jnet_bill_no=" + jnet_bill_no)
                .Append("&agent_bill_id=" + agent_bill_id)
                .Append("&pay_type=" + pay_type)
                .Append("&pay_amt=" + pay_amt)
                .Append("&remark=" + remark)
                .Append("&key=" + merhantKey);

            String md5src = sbSign.ToString();
            // 对数据进行加密验证

            String md5sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(md5src, "MD5").ToLower();

            if (returnSign.Equals(md5sign))
            {
                if (result == "1")
                {
                    //处理订单支付状态
                    bool isOK = false;
                    strTipMsg = "";
                    if (isOK)
                    {
                        Base.Utility.Log.Error(String.Format("{0},strPaymentId:{1}", strTipMsg, agent_bill_id));
                    }
                    succMsg = strTipMsg;
                    isResponse = true;
                    responseText = "ok";
                }
                else
                {
                    errorMsg = pay_message;
                }
            }
            else
            {
                errorMsg = "交易签名无效！";
            }
            #endregion
            return new ResultCallBack(isResponse, responseText, errorMsg, succMsg, pay_amt);
        }
    }

    /// <summary>
    /// Mo宝在线支付 
    /// </summary>
    public class PaymentMob : PaymentGateway
    {
        /// <summary>
        /// 在线支付接口
        /// </summary>
        /// <param name="bankType">支付银行</param>
        /// <param name="billNO">订单号</param>
        /// <param name="billAmount">订单金额</param>
        /// <param name="product_name">商品名称</param>
        /// <param name="product_remark">订单备注</param>
        /// <param name="strHomeUrl">提交路径</param>
        public override string PayOnline(string bankType, string billNO, string billAmount, string product_name, string product_remark, string strHomeUrl)
        {
            //页面提交参数
            Dictionary<string, string> payData = new Dictionary<string, string>();
            StringBuilder sbHtml = new StringBuilder();

            string merhantID = ConfigHelper.AppSettings("mob_merhantID");
            string platformID = ConfigHelper.AppSettings("mob_platformID");
            string merhantKey = ConfigHelper.AppSettings("mob_merhantKey");

            bool isDirect = (bankType == "32" || bankType == "31") ? false : true;
            string bank_code = string.Empty;

            #region Mo宝接口


            #region Mo宝银行名称对应银行ID

            switch (bankType)
            {
                case "1": //中国工商
                    bank_code = "ICBC";
                    break;
                case "2": //中国建设
                    bank_code = "CCB";
                    break;
                case "3": //中国农业
                    bank_code = "ABC";
                    break;
                case "4": //中国银行
                    bank_code = "BOC";
                    break;
                case "5": //中国邮政
                    bank_code = "PSBC";
                    break;
                case "6": //中国光大
                    bank_code = "CEB";
                    break;
                case "7": //招商银行
                    bank_code = "CMB";
                    break;
                case "8": //广发银行
                    bank_code = "GDB";
                    break;
                case "9": //中国民生
                    bank_code = "CMBC";
                    break;
                case "10": //平安银行
                    bank_code = "PAB";
                    break;
                case "11": //浦发银行
                    bank_code = "SPDB";
                    break;
                case "12": //中信银行
                    bank_code = "CNCB";
                    break;
                case "13": //交通银行
                    bank_code = "COMM";
                    break;
                case "14": //兴业银行
                    bank_code = "CIB";
                    break;
                default:
                    bank_code = "ICBC";
                    break;
            }
            #endregion

            payData.Clear();
            payData.Add("apiName", "WEB_PAY_B2C");
            payData.Add("apiVersion", "1.0.0.0");
            payData.Add("platformID", platformID);//商户平台号
            payData.Add("merchNo", merhantID);//商户帐号
            payData.Add("orderNo", billNO);
            payData.Add("tradeDate", DateTime.Now.ToString("yyyyMMdd"));
            payData.Add("amt", billAmount);
            payData.Add("merchUrl", PayIntFaceType.GetReturn_Url("mob_Return_Url"));//返回链接
            payData.Add("merchParam", "PayIntfaceType=5");
            payData.Add("tradeSummary", product_name);
            payData.Add("bankCode", isDirect ? bank_code : "");


            string requestStr = MobaopayMerchant.Instance.generatePayRequest(payData);  // 组织签名源数据
            payData.Add("signMsg", MobaopaySignUtil.Instance.sign(requestStr));         // 生成签名数据

            #endregion
            sbHtml.Length = 0;
            foreach (KeyValuePair<string, string> temp in payData)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }
            return sbHtml.ToString();
        }


        /// <summary>
        /// Mob支付回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ResultCallBack RechargeCallBack()
        {
            bool isResponse = false;
            string responseText = "";
            string errorMsg = string.Empty;
            string succMsg = string.Empty;
            string strTipMsg = string.Empty;
            string tradeAmt = "";
            #region mo宝

            Dictionary<string, string> dict = GetRequestPost();
            // 判断是否有带返回参数
            if (dict.Count > 0)
            {
                // 验证签名，先获取到签名源字符串和签名字符串后，做签名验证。
                string srcString =
                    string.Format(
                        "apiName={0}&notifyTime={1}&tradeAmt={2}&merchNo={3}&merchParam={4}&orderNo={5}&tradeDate={6}&accNo={7}&accDate={8}&orderStatus={9}",
                        dict["apiName"],
                        dict["notifyTime"],
                        dict["tradeAmt"],
                        dict["merchNo"],
                        dict["merchParam"],
                        dict["orderNo"],
                        dict["tradeDate"],
                        dict["accNo"],
                        dict["accDate"],
                        dict["orderStatus"]);
                string sigString = dict["signMsg"];
                string notifyType = dict["notifyType"];

                sigString = sigString.Replace("\r", "").Replace("\n", "");
                bool verifyResult = MobaopaySignUtil.Instance.verifyData(sigString, srcString);
                string veryfyDesc = verifyResult ? "签名验证通过" : "签名验证失败";

                // 取出用于显示的各个数据，这里只是为了演示，实际应用中应该不需要把这些数据显示到页面上。
                string apiName = dict["apiName"];
                string notifyTime = dict["notifyTime"];
                tradeAmt = dict["tradeAmt"]; //交易金额
                string merchNo = dict["merchNo"]; //商户号
                string merchParam = dict["merchParam"]; //商户参数，来自支付请求中的商户参数，原物返回，方便商户异步处理需要传递数据
                string orderNo = dict["orderNo"]; //商户订单号
                string tradeDate = dict["tradeDate"]; //商户交易日期
                string accNo = dict["accNo"]; //支付平台订单号
                string accDate = dict["accDate"]; //支付平台订单日期
                string orderStatus = dict["orderStatus"]; //订单状态：0-未支付，1-成功，2-失败；实际上只有成功才会发送通知

                if (verifyResult)
                {
                    if (orderStatus == "1")
                    {
                        bool blfine = false;

                        if (!blfine)
                        {
                            Base.Utility.Log.Error(String.Format("{0},在线充值paymentid:{1},{2}元", strTipMsg, orderNo, tradeAmt));
                        }
                        // 验证签名通过后，商户系统回写“SUCCESS”以表明商户收到了通知
                        if (notifyType == "1")
                        {
                            responseText = "SUCCESS";
                            //Response.Write("SUCCESS");
                        }
                        isResponse = true;
                        succMsg = strTipMsg;
                    }
                    else
                    {
                        errorMsg = "第三方充值错误，请联系客服!";
                    }
                }
                else
                {
                    errorMsg = "第三方充值错误，请联系客服!";
                    Base.Utility.Log.Error(String.Format("交易签名无效,在线充值paymentid:{0} ", orderNo));
                }
            }

            #endregion

            return new ResultCallBack(isResponse, responseText, errorMsg, succMsg, tradeAmt);
        }

        /// <summary>
        /// Mo宝扩展
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRequestPost()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            NameValueCollection coll = HttpContext.Current.Request.Form;

            foreach (string s in coll.AllKeys)
            {
                dict.Add(s, coll.Get(s));
            }
            return dict;
        }
    }

    /// <summary>
    /// 智付在线支付 
    /// </summary>
    public class PaymentZF : PaymentGateway
    {
        /// <summary>
        /// 在线支付接口
        /// </summary>
        /// <param name="bankType">支付银行</param>
        /// <param name="billNO">订单号</param>
        /// <param name="billAmount">订单金额</param>
        /// <param name="product_name">商品名称</param>
        /// <param name="product_remark">订单备注</param>
        /// <param name="strHomeUrl">提交路径</param>
        public override string PayOnline(string bankType, string billNO, string billAmount, string product_name, string product_remark, string strHomeUrl)
        {
            //页面提交参数
            Dictionary<string, string> payData = new Dictionary<string, string>();
            StringBuilder sbHtml = new StringBuilder();


            string merhantID = ConfigHelper.AppSettings("hfb_merhantID");
            string merhantKey = ConfigHelper.AppSettings("hfb_merhantKey");

            string bank_code = string.Empty;
            string input_charset = string.Empty;
            string interface_version = string.Empty;
            string merchant_code = string.Empty;
            string notify_url = string.Empty;
            string order_amount = string.Empty;
            string order_no = string.Empty;


            string order_time = string.Empty;
            product_name = string.Empty;

            string return_url = string.Empty;
            string service_type = string.Empty;
            string sign_type = string.Empty;
            string sign = string.Empty;

            #region  智付银行名称对应银行ID

            switch (bankType)
            {
                case "1": //中国工商
                    bank_code = "ICBC";
                    break;
                case "2": //中国建设
                    bank_code = "CCB";
                    break;
                case "3": //中国农业
                    bank_code = "ABC";
                    break;
                case "4": //中国银行
                    bank_code = "BOCSH";
                    break;
                case "5": //中国邮政
                    bank_code = "PSBC";
                    break;
                case "6": //中国光大
                    bank_code = "CEB";
                    break;
                case "7": //招商银行
                    bank_code = "CMB";
                    break;
                case "8": //广发银行
                    bank_code = "GDB";
                    break;
                case "9": //中国民生
                    bank_code = "CMBC";
                    break;
                case "10": //平安银行
                    bank_code = "PAB";
                    break;
                case "11": //浦发银行
                    bank_code = "SPDB";
                    break;
                case "12": //中信银行
                    bank_code = "CNCB";
                    break;
                case "13": //交通银行
                    bank_code = "BOCOM";
                    break;
                case "14": //兴业银行
                    bank_code = "CIB";
                    break;
                case "15": //上海银行
                    bank_code = "SHB-NET-B2C";
                    break;
                case "16": //杭州银行
                    bank_code = "HZBANK-NET-B2C";
                    break;
                case "17": //北京银行
                    bank_code = "BCCB-NET-B2C";
                    break;
                case "18": //东亚银行
                    bank_code = "BEA";
                    break;
                default:
                    bank_code = "ICBC";
                    break;
            }
            #endregion

            interface_version = "V3.0";
            merchant_code = merhantID;
            // 后台通知地址(必填)
            notify_url = PayIntFaceType.GetNotify_Url("zf_notify_url");
            return_url = PayIntFaceType.GetReturn_Url("zf_return_url");
            // 页面跳转同步通知地址(选填)

            order_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // 商品名称（必填）
            product_name = "No1.Recharge";
            order_no = billNO.Trim();
            input_charset = "UTF-8";
            sign_type = "RSA-S";
            service_type = "direct_pay"; //业务类型(必填)
            order_amount = billAmount.Trim();




            ////////////////组装签名参数//////////////////

            string product_code = "A1";
            string product_num = "1";
            string product_desc = "test";
            string extra_return_param = "pay";
            string extend_param = "";
            string signStr = "";
            string client_ip = IpHelper.GetWebClientIp();
            string pay_type = "";
            string redo_flag = "";
            string show_url = "";


            #region 组织订单信息
            //---
            //组织订单信息
            if (bank_code != "")
            {
                signStr = signStr + "bank_code=" + bank_code + "&";
            }
            if (client_ip != "")
            {
                signStr = signStr + "client_ip=" + client_ip + "&";
            }
            if (extend_param != "")
            {
                signStr = signStr + "extend_param=" + extend_param + "&";
            }
            if (extra_return_param != "")
            {
                signStr = signStr + "extra_return_param=" + extra_return_param + "&";
            }
            if (input_charset != "")
            {
                signStr = signStr + "input_charset=" + input_charset + "&";
            }
            if (interface_version != "")
            {
                signStr = signStr + "interface_version=" + interface_version + "&";
            }
            if (merchant_code != "")
            {
                signStr = signStr + "merchant_code=" + merchant_code + "&";
            }
            if (notify_url != "")
            {
                signStr = signStr + "notify_url=" + notify_url + "&";
            }
            if (order_amount != "")
            {
                signStr = signStr + "order_amount=" + order_amount + "&";
            }
            if (order_no != "")
            {
                signStr = signStr + "order_no=" + order_no + "&";
            }
            if (order_time != "")
            {
                signStr = signStr + "order_time=" + order_time + "&";
            }
            if (pay_type != "")
            {
                signStr = signStr + "pay_type=" + pay_type + "&";
            }
            if (product_code != "")
            {
                signStr = signStr + "product_code=" + product_code + "&";
            }
            if (product_desc != "")
            {
                signStr = signStr + "product_desc=" + product_desc + "&";
            }
            if (product_name != "")
            {
                signStr = signStr + "product_name=" + product_name + "&";
            }
            if (product_num != "")
            {
                signStr = signStr + "product_num=" + product_num + "&";
            }
            if (redo_flag != "")
            {
                signStr = signStr + "redo_flag=" + redo_flag + "&";
            }
            if (return_url != "")
            {
                signStr = signStr + "return_url=" + return_url + "&";
            }
            if (service_type != "")
            {
                signStr = signStr + "service_type=" + service_type;
            }
            if (show_url != "")
            {
                signStr = signStr + "&show_url=" + show_url;
            }
            #endregion
            //--

            //商家私钥 // "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALf/+xHa1fDTCsLYPJLHy80aWq3djuV1T34sEsjp7UpLmV9zmOVMYXsoFNKQIcEzei4QdaqnVknzmIl7n1oXmAgHaSUF3qHjCttscDZcTWyrbXKSNr8arHv8hGJrfNB/Ea/+oSTIY7H5cAtWg6VmoPCHvqjafW8/UP60PdqYewrtAgMBAAECgYEAofXhsyK0RKoPg9jA4NabLuuuu/IU8ScklMQIuO8oHsiStXFUOSnVeImcYofaHmzIdDmqyU9IZgnUz9eQOcYg3BotUdUPcGgoqAqDVtmftqjmldP6F6urFpXBazqBrrfJVIgLyNw4PGK6/EmdQxBEtqqgXppRv/ZVZzZPkwObEuECQQDenAam9eAuJYveHtAthkusutsVG5E3gJiXhRhoAqiSQC9mXLTgaWV7zJyA5zYPMvh6IviX/7H+Bqp14lT9wctFAkEA05ljSYShWTCFThtJxJ2d8zq6xCjBgETAdhiH85O/VrdKpwITV/6psByUKp42IdqMJwOaBgnnct8iDK/TAJLniQJABdo+RodyVGRCUB2pRXkhZjInbl+iKr5jxKAIKzveqLGtTViknL3IoD+Z4b2yayXg6H0g4gYj7NTKCH1h1KYSrQJBALbgbcg/YbeU0NF1kibk1ns9+ebJFpvGT9SBVRZ2TjsjBNkcWR2HEp8LxB6lSEGwActCOJ8Zdjh4kpQGbcWkMYkCQAXBTFiyyImO+sfCccVuDSsWS+9jrc5KadHGIvhfoRjIj2VuUKzJ+mXbmXuXnOYmsAefjnMCI6gGtaqkzl527tw=";
            //私钥转换成C#专用私钥
            string merPriKey = HttpHelp.RSAPrivateKeyJava2DotNet(merhantKey);
            //签名
            string signData = HttpHelp.RSASign(signStr, merPriKey);
            //将signData进行UrlEncode编码

            payData.Add("bank_code", bank_code);//银行代码  
            payData.Add("input_charset", "UTF-8");
            payData.Add("interface_version", interface_version);
            payData.Add("merchant_code", merchant_code);
            payData.Add("notify_url", notify_url);
            payData.Add("order_amount", order_amount);
            payData.Add("order_no", order_no);
            payData.Add("order_time", order_time);
            payData.Add("product_name", product_name);

            payData.Add("client_ip", client_ip);
            payData.Add("extend_param", extend_param);
            payData.Add("extra_return_param", extra_return_param);
            payData.Add("product_code", product_code);
            payData.Add("product_desc", product_desc);
            payData.Add("product_num", product_num);

            payData.Add("return_url", return_url);
            payData.Add("service_type", service_type);
            payData.Add("sign_type", sign_type);
            payData.Add("sign", signData);
            payData.Add("show_url", show_url);
            payData.Add("redo_flag", redo_flag);

            payData.Add("pay_type", pay_type);


            foreach (KeyValuePair<string, string> temp in payData)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }
            return sbHtml.ToString();

        }

        /// <summary>
        /// 智付支付回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ResultCallBack RechargeCallBack()
        {
            bool isResponse = false;
            string responseText = "";
            string strTipMsg = String.Empty;
            string errorMsg = string.Empty;
            string succMsg = string.Empty;

            #region 智付
            // 校验返回数据包 
            // 获取智付反馈信息 (商户号)
            string merchant_code = HttpContext.Current.Request.Form["merchant_code"].ToString().Trim();
            // 通知类型
            string notify_type = HttpContext.Current.Request.Form["notify_type"].ToString().Trim();
            // 通知校验ID
            string notify_id = HttpContext.Current.Request.Form["notify_id"].ToString().Trim();
            // 接口版本
            string interface_version = HttpContext.Current.Request.Form["interface_version"].ToString().Trim();
            // 签名方式
            string sign_type = HttpContext.Current.Request.Form["sign_type"].ToString().Trim();
            // 签名
            string dinpaySign = HttpContext.Current.Request.Form["sign"].ToString().Trim();
            // 商家订单号
            string order_no = HttpContext.Current.Request.Form["order_no"].ToString().Trim();
            // 商家订单时间
            string order_time = HttpContext.Current.Request.Form["order_time"].ToString().Trim();
            // 商家订单金额
            string order_amount = HttpContext.Current.Request.Form["order_amount"].ToString().Trim();
            // 智付交易定单号
            string trade_no = HttpContext.Current.Request.Form["trade_no"].ToString().Trim();
            // 智付交易时间
            string trade_time = HttpContext.Current.Request.Form["trade_time"].ToString().Trim();
            // 交易状态 SUCCESS 成功  FAILED 失败
            string trade_status = HttpContext.Current.Request.Form["trade_status"].ToString().Trim();
            // 银行交易流水号
            string bank_seq_no = HttpContext.Current.Request.Form["bank_seq_no"];

            string extra_return_param = HttpContext.Current.Request.Form["extra_return_param"].ToString().Trim();
            bool blSuccess = false;

            /**
       *签名顺序按照参数名a到z的顺序排序，若遇到相同首字母，则看第二个字母，以此类推，
      *同时将商家支付密钥key放在最后参与签名，组成规则如下：
      *参数名1=参数值1&参数名2=参数值2&……&参数名n=参数值n
      **/

            //组织订单信息
            string signStr = "";

            if (bank_seq_no != "")
            {
                signStr = signStr + "bank_seq_no=" + bank_seq_no + "&";
            }
            if (extra_return_param != "")
            {
                signStr = signStr + "extra_return_param=" + extra_return_param + "&";
            }
            if (interface_version != "")
            {
                signStr = signStr + "interface_version=" + interface_version + "&";
            }
            if (merchant_code != "")
            {
                signStr = signStr + "merchant_code=" + merchant_code + "&";
            }
            if (notify_id != "")
            {
                signStr = signStr + "notify_id=" + notify_id + "&";
            }
            if (notify_type != "")
            {
                signStr = signStr + "notify_type=" + notify_type + "&";
            }
            if (order_amount != "")
            {
                signStr = signStr + "order_amount=" + order_amount + "&";
            }
            if (order_no != "")
            {
                signStr = signStr + "order_no=" + order_no + "&";
            }
            if (order_time != "")
            {
                signStr = signStr + "order_time=" + order_time + "&";
            }
            if (trade_no != "")
            {
                signStr = signStr + "trade_no=" + trade_no + "&";
            }
            if (trade_status != "")
            {
                signStr = signStr + "trade_status=" + trade_status + "&";
            }
            if (trade_time != "")
            {
                signStr = signStr + "trade_time=" + trade_time;
            }
            if (sign_type == "RSA-S") //RSA-S的验签方法
            {
                //使用智付公钥对返回的数据验签
                string dinpayPubKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWOq5aHSTvdxGPDKZWSl6wrPpn" +
                    "MHW+8lOgVU71jB2vFGuA6dwa/RpJKnz9zmoGryZlgUmfHANnN0uztkgwb+5mpgme" +
                    "gBbNLuGqqHBpQHo2EsiAhgvgO3VRmWC8DARpzNxknsJTBhkUvZdy4GyrjnUrvsAR" +
                    "g4VrFzKDWL0Yu3gunQIDAQAB";
                //将智付公钥转换成C#专用格式
                dinpayPubKey = HttpHelp.RSAPublicKeyJava2DotNet(dinpayPubKey);
                //验签
                bool result = HttpHelp.ValidateRsaSign(signStr, dinpayPubKey, dinpaySign);
                if (result == true)
                {
                    //如果验签结果为true，则对订单进行更新
                    //订单更新完之后打印SUCCESS
                    blSuccess = true;
                }
                else
                {
                    //验签失败
                    blSuccess = false;
                }
            }

            string strType = "1";
            if (blSuccess)
            {
                if ("page_notify" == notify_type)
                {
                    strType = "1";
                }
                else if ("offline_notify" == notify_type)
                {
                    strType = "2";
                }
                // 对充值记录进行在线审核
                bool blfine = false;
                if (!blfine)
                {
                    Log.Error(String.Format("{0},strPaymentId:{1}", strTipMsg, order_no));
                }
                // 在接收到支付结果通知后，判断是否进行过业务逻辑处理，不要重复进行业务逻辑处理
                if (trade_status == "SUCCESS")
                {
                    if ("page_notify" == notify_type)
                    {
                        errorMsg = strTipMsg;
                    }
                    else if ("offline_notify" == notify_type)
                    {
                        errorMsg = strTipMsg;
                        // 如果是服务器返回则需要回应一个特定字符串'SUCCESS',且在'SUCCESS'之前不可以有任何其他字符输出,保证首先输出的是'SUCCESS'字符串
                        //HttpContext.Response.Output.Write("SUCCESS");
                        isResponse = true;
                        responseText = "SUCCESS";
                        succMsg = (
                             "<br>交易流水号:" + trade_no +
                             "<br>商户订单号:" + order_no +
                             "<br>交易金额:" + order_amount +
                             "<br>交易币种:人民币" +
                             "<br>订单完成时间:" + trade_time);
                    }

                }
                else
                {
                    errorMsg = "支付失败!" +
                                         "<br>接口类型:Buy<br>" +
                                         "<br>交易流水号:" + trade_no +
                                         "<br>商户订单号:" + order_no +
                                         "<br>交易金额:" + order_amount +
                                         "<br>交易币种:人民币<br>订单完成时间:" + trade_time +
                                         "<br>回调方式:" + notify_type;
                }
            }
            else
            {
                errorMsg = "交易签名无效！";
            }

            #endregion 智付

            return new ResultCallBack(isResponse, responseText, errorMsg, succMsg, order_amount);
        }
    }


}
