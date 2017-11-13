using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Base.Utility
{
    /// <summary>
    /// 验证类
    /// </summary>
    public class ValidateHelper
    {
       

        #region 数字类验证

        /// <summary>
        /// 整数(负整数 + 0 + 正整数)
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static bool IsValidInt(string strInt)
        {
            return Regex.IsMatch(strInt, @"^-?\d+$");
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static bool IsValidIntPositive(string strInt)
        {
            return Regex.IsMatch(strInt, @"^[0-9]*[1-9][0-9]*$");
        }


        /// <summary>
        /// 负整数
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static bool IsValidIntNegative(string strInt)
        {
            return Regex.IsMatch(strInt, @"^-[0-9]*[1-9][0-9]*$");
        }

        /// <summary>
        /// 非正整数（负整数 + 0）
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static bool IsValidIntNonPositive(string strInt)
        {
            return Regex.IsMatch(strInt, @"^((-\d+)|(0+))$");
        }

        /// <summary>
        /// 非负整数（正整数 + 0） 
        /// </summary>
        /// <param name="strInt"></param>
        /// <returns></returns>
        public static bool IsValidIntNonNegative(string strInt)
        {
            return Regex.IsMatch(strInt, @"^\d+$");
        }

        /// <summary>
        /// 浮点数(注意，正整数如145也是正浮点数)
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static bool IsValidFloat(string strFloat)
        {
            return Regex.IsMatch(strFloat, @"^(-?\d+)(\.\d+)?$");
        }

        /// <summary>
        /// 正浮点数（注意，正整数如145也是正浮点数）
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static bool IsValidFloatPositive(string strFloat)
        {
            return Regex.IsMatch(strFloat, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
        }

        /// <summary>
        /// 负浮点数（注意，负整数如-145也是负浮点数） 
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static bool IsValidFloatNegative(string strFloat)
        {
            return Regex.IsMatch(strFloat, @"^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$");
        }

        /// <summary>
        /// 非正浮点数（负浮点数 + 0;注意，负整数如-145也是负浮点数）
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static bool IsValidFloatNonPositive(string strFloat)
        {
            return Regex.IsMatch(strFloat, @"^((-\d+(\.\d+)?)|(0+(\.0+)?))$");
        }

        /// <summary>
        /// 非负浮点数（正浮点数 + 0；注意，正整数如145也是正浮点数） 
        /// </summary>
        /// <param name="strFloat"></param>
        /// <returns></returns>
        public static bool IsValidFloatNonNegative(string strFloat)
        {
            return Regex.IsMatch(strFloat, @"^\d+(\.\d+)?$");
        }

        /// <summary>
        /// 是否是正确的时间，占定位分隔符为:
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public static bool IsValidTime(string strTime)
        {
            return Regex.IsMatch(strTime, @"^(2[0-3]|[0-1]?\d):([0-5]?\d):([0-5]?\d)$");
        }

        /// <summary>
        /// 是否是百分比数
        /// </summary>
        /// <param name="strPercent"></param>
        /// <returns></returns>
        public static bool IsValidPercent(string strPercent)
        {
            return Regex.IsMatch(strPercent, @"^\d+(\.\d+)?%$");
        }

        #endregion

        #region 字母类验证

        /// <summary>
        /// 由26个英文字母组成的字符串(含大小写字母)
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidLetters(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[A-Za-z]+$");
        }

        /// <summary>
        /// 由26个英文字母的大写组成的字符串 
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidLettersUpper(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[A-Z]+$");
        }

        /// <summary>
        /// 由26个英文字母的小写组成的字符串 
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidLettersLower(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[a-z]+$");
        }

        /// <summary>
        /// 由数字和26个英文字母(含大小写字母)组成的字符串
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidLettersNum(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 由数字、26个英文字母或者下划线组成的字符串
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidLettersNumOrUnderLine(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[A-Za-z0-9_]+$");
        }

        /// <summary>
        /// 必须有数字和字母还有一些特殊字符(`~!@#$%\^&*()_+-=)，其他特殊字符不包括，并且长度范围要求是6到16
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsLettersIncludeNum(string testStr)
        {
            return Regex.IsMatch(testStr, @"^(?=.*?[a-zA-Z])(?=.*?[0-9])[a-zA-Z0-9`~!@#$%\^&*()_+-=]{6,16}$");
        }

        /// <summary>
        /// 必须有数字和字母
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsLettersAndNumbers(string testStr)
        {
            return Regex.IsMatch(testStr, @"\d+") && Regex.IsMatch(testStr, "[a-zA-Z]");
        }

        /// <summary>
        /// 有效用用户名
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidUserName(string testStr)
        {
            return Regex.IsMatch(testStr, "^[a-zA-Z0-9_-]{6,16}$") && Regex.IsMatch(testStr, @"\d+") && Regex.IsMatch(testStr, "[a-zA-Z]");
        }

        #endregion

        #region 常用功能验证

        /// <summary>
        /// email地址
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsQQ(string testStr)
        {
            //QQ暂时只能支持11位，判定为15位内（可能15？）
            return Regex.IsMatch(testStr, @"^[1-9]\d{4,14}$");
        }

        /// <summary>
        /// email地址
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string testStr)
        {
            return Regex.IsMatch(testStr, @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$");
        }


        /// <summary>
        /// url网址(http: // 开头，后面至少带一个字符，但是不能点号结尾)
        /// </summary>
        /// <param name="testStr">如：http: //www.baidu.com或http ://baidu.com都可以，但是http ://或http ://baidu.不符合</param>
        /// <returns></returns>
        public static bool IsValidUrl(string testStr)
        {
            //return Regex.IsMatch(testStr, @"^[a-zA-z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$");
            return Regex.IsMatch(testStr, @"^(http://|https://)?([\w-]+\.)+[\w-]+(/[\w-\./?%=]*)?");
            //return Regex.IsMatch(testStr, @"^(http://|https://)?(\w+\.){1,3} (com(\.cn)?|cn|net|info|org|us|tk)\b");

        }

        /// <summary>
        /// 验证相对地址
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsVaildRelative(string testStr)
        {
            return Regex.IsMatch(testStr, @"^((/[\w\d_\-\.]+)+\??)[\w\-\./?%=]*");
        }

        /// <summary>
        /// IP地址
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidIP(string testStr)
        {
            return Regex.IsMatch(testStr, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        }

        /// <summary>
        /// 电话号码(支持手机号码，3-4位区号，7-8位直播号码，1－4位分机号)
        /// 说明：
        /// 1.手机号须11位及以上，10位会报错，可以带+86，如：13945678901、+8613945678901(但是AA13945678901也合法，哈哈);
        /// 2.可以是直接是7-8位电话号码，如：86666688,075586666688
        /// 3.使用区号或分机号须带中线-，必须配合7-8位直播号码使用，如：1234-12345678、1234-12345678-1234、12345678-1234
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidTel(string testStr)
        {
            return Regex.IsMatch(testStr, @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)");
        }

        /// <summary>
        /// 中国身份证号码
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidPersonalID(string testStr)
        {
            return Regex.IsMatch(testStr, @"\d{18}|\d{15}");
        }

        /// <summary>
        /// 中国邮政编码
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidZipCode(string testStr)
        {
            return Regex.IsMatch(testStr, @"[1-9]{1}(\d+){5}");
        }

        /// <summary>
        /// YYYY-MM-DD基本上把闰年和2月等的情况都考虑进去了，2009-2-30是非法的
        /// </summary>
        /// <param name="testStr">如：2009-9-9</param>
        /// <returns></returns>
        public static bool IsValidDate(string testStr)
        {
            return Regex.IsMatch(testStr, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        /// <summary>
        /// 中文字符
        /// 只要含了汉字就通过验证，如："Silver李程华Lee"也是通过验证的。
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidGBK(string testStr)
        {
            return Regex.IsMatch(testStr, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 匹配双字节字符(包括汉字在内)
        /// 只要含了双字节字符就通过验证，如："Silver李程华Lee"也是通过验证的。
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsValidDoubleByte(string testStr)
        {
            return Regex.IsMatch(testStr, @"[^\x00-\xff]");
        }

        #endregion


    }
}
