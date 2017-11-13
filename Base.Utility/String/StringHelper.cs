using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Base.Utility
{

    /// <summary>
    /// 操作字符串的方法
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 检查该字符串是否为空
        /// </summary>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// 判断对象是否为正确的日期值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Boolean。</returns>
        public static bool IsDateTime(object obj)
        {
            try
            {
                DateTime dt = DateTime.Parse(ToObjectString(obj));
                if (dt > DateTime.MinValue && DateTime.MaxValue > dt)
                    return true;
                return false;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Int32值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Int32值。</returns>
        public static bool IsInt(object obj)
        {
            try
            {
                int.Parse(ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Long值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Long值。</returns>
        public static bool IsLong(object obj)
        {
            try
            {
                long.Parse(ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Float值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Float值。</returns>
        public static bool IsFloat(object obj)
        {
            try
            {
                float.Parse(ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Double值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Double值。</returns>
        public static bool IsDouble(object obj)
        {
            try
            {
                double.Parse(ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// 判断对象是否为正确的Decimal值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Decimal值。</returns>
        public static bool IsDecimal(object obj)
        {
            try
            {
                decimal.Parse(ToObjectString(obj));
                return true;
            }
            catch
            { return false; }
        }
        /// <summary>   
        /// 判断用户输入是否为日期   
        /// </summary>   
        /// <param ></param>   
        /// <returns></returns>   
        /// <remarks>   
        /// 可判断格式如下（其中-可替换为.，不影响验证)   
        /// YYYY | YYYY-MM |YYYY.MM| YYYY-MM-DD|YYYY.MM.DD | YYYY-MM-DD HH:MM:SS | YYYY.MM.DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF | YYYY.MM.DD HH:MM:SS:FF (年份验证从1000到2999年)
        /// </remarks>   
        public static bool IsDateTime(string strValue)
        {
            if (strValue == null || strValue == "")
            {
                return false;
            }
            string regexDate = @"[1-2]{1}[0-9]{3}((-|[.]){1}(([0]?[1-9]{1})|(1[0-2]{1}))((-|[.]){1}((([0]?[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1})))( (([0-1]{1}[0-9]{1})|2[0-3]{1}):([0-5]{1}[0-9]{1}):([0-5]{1}[0-9]{1})(\.[0-9]{3})?)?)?)?$";
            if (Regex.IsMatch(strValue, regexDate))
            {
                //以下各月份日期验证，保证验证的完整性   
                int _IndexY = -1;
                int _IndexM = -1;
                int _IndexD = -1;
                if (-1 != (_IndexY = strValue.IndexOf("-")))
                {
                    _IndexM = strValue.IndexOf("-", _IndexY + 1);
                    _IndexD = strValue.IndexOf(":");
                }
                else
                {
                    _IndexY = strValue.IndexOf(".");
                    _IndexM = strValue.IndexOf(".", _IndexY + 1);
                    _IndexD = strValue.IndexOf(":");
                }
                //不包含日期部分，直接返回true   
                if (-1 == _IndexM)
                {
                    return true;
                }
                if (-1 == _IndexD)
                {
                    _IndexD = strValue.Length + 3;
                }
                int iYear = Convert.ToInt32(strValue.Substring(0, _IndexY));
                int iMonth = Convert.ToInt32(strValue.Substring(_IndexY + 1, _IndexM - _IndexY - 1));
                int iDate = Convert.ToInt32(strValue.Substring(_IndexM + 1, _IndexD - _IndexM - 4));
                //判断月份日期   
                if ((iMonth < 8 && 1 == iMonth % 2) || (iMonth > 8 && 0 == iMonth % 2))
                {
                    if (iDate < 32)
                    { return true; }
                }
                else
                {
                    if (iMonth != 2)
                    {
                        if (iDate < 31)
                        { return true; }
                    }
                    else
                    {
                        //闰年   
                        if ((0 == iYear % 400) || (0 == iYear % 4 && 0 < iYear % 100))
                        {
                            if (iDate < 30)
                            { return true; }
                        }
                        else
                        {
                            if (iDate < 29)
                            { return true; }
                        }
                    }
                }
            }
            return false;
        }

        #region 类型转换

        /// <summary>
        /// 返回对象obj的String值,obj为null时返回空值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>字符串。</returns>
        public static string ToObjectString(object obj)
        {
            return null == obj ? String.Empty : obj.ToString();
        }
        /// <summary>
        /// 取得Int值,如果为Null 则返回０
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetInt(object obj)
        {
            if (obj != null)
            {
                int i;
                int.TryParse(obj.ToString(), out i);
                return i;
            }
            else
                return 0;
        }

        public static float GetFloat(object obj)
        {
            float i;
            float.TryParse(obj.ToString(), out i);
            return i;
        }

        /// <summary>
        /// 取得Int值,如果不成功则返回指定exceptionvalue值
        /// </summary>
        /// <param name="obj">要计算的值</param>
        /// <param name="exceptionvalue">异常时的返回值</param>
        /// <returns></returns>
        public static int GetInt(object obj, int exceptionvalue)
        {
            if (obj == null)
                return exceptionvalue;
            if (string.IsNullOrEmpty(obj.ToString()))
                return exceptionvalue;
            int i = exceptionvalue;
            try { i = Convert.ToInt32(obj); }
            catch { i = exceptionvalue; }
            return i;
        }

        /// <summary>
        /// 取得byte值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte Getbyte(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return byte.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 获得Long值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetLong(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return long.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 取得Long值,如果不成功则返回指定exceptionvalue值
        /// </summary>
        /// <param name="obj">要计算的值</param>
        /// <param name="exceptionvalue">异常时的返回值</param>
        /// <returns></returns>
        public static long GetLong(object obj, long exceptionvalue)
        {
            if (obj == null)
            {
                return exceptionvalue;
            }
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return exceptionvalue;
            }
            long i = exceptionvalue;
            try
            {
                i = Convert.ToInt64(obj);
            }
            catch
            {
                i = exceptionvalue;
            }
            return i;
        }

        /// <summary>
        /// 取得Decimal值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal GetDecimal(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return decimal.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString());
            else
                return DateTime.Now;
            //return DateTime.MinValue;
        }

        /// <summary>
        /// 取得DateTime值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(object obj)
        {
            if (obj != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString());
            else
                return null;
        }
        /// <summary>
        /// 格式化日期 yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFormatDateTime(object obj, string Format)
        {
            if (obj != null && obj.ToString() != null && obj.ToString() != "")
                return DateTime.Parse(obj.ToString()).ToString(Format);
            else
                return "";
        }
        /// <summary>
        /// Json 的日期格式与.Net DateTime类型的转换
        /// </summary>
        /// <param name="jsonDate">Date(1242357713797+0800)</param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(5, jsonDate.Length - 6) + "+0800";
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }
        /// <summary>
        /// 取得bool值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetBool(object obj)
        {
            if (obj != null)
            {
                bool flag;
                bool.TryParse(obj.ToString(), out flag);
                return flag;
            }
            else
                return false;
        }

        /// <summary>
        /// 取得byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Byte[] GetByte(object obj)
        {
            if (obj.ToString() != null && obj.ToString() != "")
            {
                return (Byte[])obj;
            }
            else
                return null;
        }

        /// <summary>
        /// 取得string值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetString(object obj)
        {
            if (obj != null && obj != DBNull.Value)
                return obj.ToString();
            else
                return "";
        }


        /// <summary>
        /// 格式化字符串
        /// </summary>
        public static string FormatWith(this string source, params object[] args)
        {
            return String.Format(source, args);
        }


        /// <summary>
        /// 把字符串转换为Int
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>转换后的结果</returns>
        public static int StrToInt32(string str, int defaultValue)
        {
            int i = 0;
            return Int32.TryParse(str, out i) ? i : defaultValue;
        }

        /// <summary>
        /// 转小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToLong(this string str)
        {
            if (!str.IsNullOrEmpty())
                return str.ToLong();
            else
                return "";
        }

        /// <summary>
        /// 转大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpper(this string str)
        {
            if (!str.IsNullOrEmpty())
                return str.ToUpper();
            else
                return "";
        }

        #endregion


        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        public static string RepairZero(string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";

            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }

            //连接text
            temp += text;

            //返回补足0的字符串
            return temp;
        }
        #endregion

        #region 连接字符串

        public static string Contact(params string[] appends)
        {
            return Contact("", appends);
        }

        public static string Contact(string separator, params string[] appends)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string str in appends)
            {
                if (!string.IsNullOrEmpty(str.Trim()))
                {
                    if (sb.Length == 0)
                    {
                        sb.AppendFormat(str);
                    }
                    else
                    {
                        sb.Append(separator);
                        sb.Append(str);
                    }
                }
            }

            return sb.ToString();
        }

        #endregion

        #region 截取字符串

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="length">最大长度</param>
        /// <returns>截取后的字符串</returns>
        public static string GetCut(string sourceString, int length)
        {
            return GetCut(sourceString, length, "");
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="length">最大长度</param>
        /// <param name="replaceStr">替换被截取掉的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetCut(string sourceString, int length, string replaceStr)
        {
            if (!string.IsNullOrEmpty(sourceString) && sourceString.Length > length)
            {
                return sourceString.Substring(0, length) + replaceStr;
            }
            return sourceString;
        }


        /// <summary>
        /// 将字符串超过指定长度的部分截断,中文按2位,英文1位
        /// </summary>
        /// <param name="source">待截断的字符串</param>
        /// <param name="length">长度</param>
        /// <returns>返回指定长度的部分</returns>
        public static string Cutstring(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            byte[] bytesOfSource = System.Text.Encoding.Default.GetBytes(source);

            if (length > bytesOfSource.Length)
                return source;

            Regex reg = new Regex("[^\x00-\xff]+", RegexOptions.Compiled);

            char[] charsOfSource = source.ToCharArray();

            StringBuilder resultBuilder = new StringBuilder();

            int index = 0;

            for (int i = 0; i < charsOfSource.Length; i++)
            {
                if (reg.IsMatch(charsOfSource[i].ToString()))
                {
                    if (length - index > 1)
                    {
                        resultBuilder.Append(charsOfSource[i]);
                    }

                    index += 2;
                }
                else
                {
                    resultBuilder.Append(charsOfSource[i]);

                    index = index + 1;
                }

                if (index >= length)
                    break;
            }

            return resultBuilder.ToString();
        }
        #endregion

        #region 检查某集合中是否包含某字符串

        /// <summary>
        /// 检查某集合中是否包含某字符串(区分大小写)
        /// </summary>
        /// <param name="findString">要查询的字符串</param>
        /// <param name="allStr">被检查的集合</param>
        /// <returns>bool</returns>
        public static bool Contains(string findString, List<string> allStr)
        {
            return allStr.Contains(findString);
        }

        /// <summary>
        /// 检查某集合中是否包含某数组中任意一个(区分大小写)
        /// </summary>
        /// <param name="findString">要查询的数组</param>
        /// <param name="allStr">被检查的集合</param>
        /// <returns>bool</returns>
        public static bool Contains(string[] findString, List<string> allStr)
        {
            foreach (string str in findString)
            {
                if (allStr.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 判断是否与指定字符号相同
        /// </summary>
        public static bool Matches(this string source, string compare)
        {
            return String.Equals(source, compare, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 判断是否与指定字符号相同(去掉首尾空格后)
        /// </summary>
        public static bool MatchesTrimmed(this string source, string compare)
        {
            return String.Equals(source.Trim(), compare.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion


        /// <summary>
        /// 将字符串转换为Byte数组
        /// </summary>
        /// <param name="CharType">编码方式[utf-8;gb2312]</param>
        /// <param name="CharStr">需要转换的字符串</param>
        /// <returns>返回Byte数组对象</returns>
        public static byte[] getByte(string CharType, string CharStr)
        {
            Encoding myEncoding = Encoding.GetEncoding(CharType);

            return myEncoding.GetBytes(CharStr);
        }


        /// <summary>
        /// 获得双字节字符串的字节数 
        /// </summary>
        /// <param name="str">要检测的字符串</param>
        /// <returns>返回字节数</returns>
        public static int GetStrLength(string str)
        {
            byte[] bytes = new ASCIIEncoding().GetBytes(str);
            int num = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x3f)
                {
                    num++;
                }
                num++;
            }
            return num;
        }

        /// <summary>
        /// 取字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(string str)
        {
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(str);
            double len_Temp = 0;
            for (int i = 0; i < buffer.Length - 1; i = i + 2)
            {
                if (buffer[i + 1] == 0)  //Unicode编码每个字符占两个byte，英文字符高位为0
                    len_Temp += 0.5;
                else
                    len_Temp++;
            }

            string[] arr1 = len_Temp.ToString("0.00").Split('.');
            int ReturnValue = Convert.ToInt32(arr1[0]);
            if ((len_Temp % ReturnValue) != 0)
                ReturnValue++;

            return ReturnValue;
        }

        /// <summary>
        /// 将字符串数组转换成用标示符隔开的连续字符串
        /// </summary>
        /// <param name="array">字符串数组</param>
        /// <param name="flag">隔开的标示符</param>
        /// <returns>返回字符串</returns>
        public static string ConverArrayToString(string[] array, string flag)
        {
            if (array.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (string str in array)
            {
                builder.Append(flag);
                builder.Append(str);
            }
            return builder.ToString().Substring(1);
        }

        /// <summary>
        /// 输出换行格式
        /// </summary>
        /// <returns>返回换行格式代码</returns>
        public static string aLine(int LineNum)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < LineNum; i++)
            {
                builder.Append("\r\n");
            }

            return builder.ToString();
        }

        /// <summary>
        /// 输出空格的字符
        /// </summary>
        /// <param name="SpaceNum">空格的数量</param>
        /// <returns>返回输出空格的格式的字符</returns>
        public static string aSpace(int SpaceNum)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < SpaceNum; i++)
            {
                builder.Append("\t");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 传入的字符前加入空格且换行
        /// </summary>
        /// <param name="aSpaceNum">空格数</param>
        /// <param name="sText">传入的字符</param>
        /// <param name="aLineNum">换行数</param>
        /// <returns>返回带空格和换行格式的字符</returns>
        public static string aSL(int aSpaceNum, string sText, int aLineNum)
        {
            string _aSl;
            _aSl = aSpace(aSpaceNum) + sText + aLine(aLineNum);
            return _aSl;
        }

        /// <summary>
        /// 移除当前字符串从开始到结束位置的字符
        /// </summary>
        /// <param name="str">传入字符</param>
        /// <param name="Start">开始位置</param>
        /// <param name="Num">结速位置</param>
        public static string Remove(string str, int Start, int Num)
        {
            return str.Remove(Start, Num);
        }

        /// <summary>
        /// 取指定长度的字符串:字符串如果超过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>
        /// 取指定长度的字符串:字符串如果超过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (p_StartIndex >= p_SrcString.Length)
                    {
                        return "";
                    }
                    else
                    {
                        return p_SrcString.Substring(p_StartIndex,
                                                       ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                    }
                }
            }


            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }



                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                    {
                        nRealLength = p_Length + 1;
                    }

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }

    }
}
