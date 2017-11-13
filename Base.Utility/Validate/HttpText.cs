using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Base.Utility.Validate
{

    /// <summary>
    /// 操作字符串的方法
    /// </summary>
    public static class HttpText
    {


        /// <summary>
        /// 检查该字符串是否为空
        /// </summary>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public static string FormatWith(this string source, params object[] args)
        {
            return String.Format(source, args);
        }


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
        /// 清除html/text中的换行代码及空间
        /// </summary>
        /// <param name="str">html/text代码</param>
        /// <param name="Code">编码：gb2312,utf-8</param>
        /// <returns>返回被替换过的代码</returns>
        public static string ClearSpace(string str, string Code)
        {
            string _space;

            //基本去掉
            str = str.Trim();
            if (Code.ToLower() == "gb2312")
            {
                _space = str.Replace("\r\n", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "");
            }
            else
            {
                _space = str.Replace("E2 80 A8", "");
            }

            return _space;
        }

        /// <summary>
        /// 去除所有的html标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearAllHtml(string str)
        {
            string _str = Regex.Replace(str, "<[^>]*>", "");
            return ClearSpace(_str, "gb2312");
        }

        /// <summary>
        /// 清除html/text中的换行代码及空间
        /// </summary>
        /// <param name="str">html/text代码</param>
        /// <param name="Code">编码：gb2312,utf-8</param>
        /// <returns>返回被替换过的代码</returns>
        public static string ClearBuyNumberSpace(string str, string Code)
        {
            string _space;

            //基本去掉
            if (String.IsNullOrEmpty(str)) { return string.Empty; }
            str = str.Trim();
            if (Code.ToLower() == "gb2312")
            {
                _space = str.Replace("\n", "").Replace(" ", "");
            }
            else
            {
                _space = str.Replace("E2 80 A8", "");
            }

            return _space;
        }

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
        /// 过滤HTML代码（常规替换方法）
        /// </summary>
        /// <param name="Htmlstring">Html代码</param>
        /// <returns>返回过滤过的Html代码</returns>
        public static string ClearHtml(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "\x00a1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "\x00a2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "\x00a3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "\x00a9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("<.*?>", "");
            Htmlstring.Replace(" ", "");
            Htmlstring.Replace("'", "");
            Htmlstring.Replace("&nbsp;", "");
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "<br />", RegexOptions.IgnoreCase);
            if (HttpContext.Current == null)
                Htmlstring = HttpUtility.HtmlEncode(Htmlstring).Trim();//这样的处理方法貌似不可行，CCM
            else
                Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="strString"></param>
        /// <returns></returns>
        public static string GetHtml(string strString)
        {
            if (HttpContext.Current == null)
                strString = HttpUtility.HtmlDecode(strString).Trim();//这样的处理方法貌似不可行，CCM
            else
                strString = HttpContext.Current.Server.HtmlDecode(strString).Trim();
            return strString;
        }

        /// <summary>
        /// 将Html代码通过自定义方式编码
        /// </summary>
        /// <param name="html">需要编码Html代码</param>
        /// <returns>返回编码过的HTML</returns>
        public static string HtmlEecode(string html)
        {
            if (html.Trim() == "")
            {
                return "";
            }
            html = html.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "#apos;").Replace("&", "&amp;");
            return html;
        }

        /// <summary>
        ///  将Html代码通过自定义方式解码
        /// </summary>
        /// <param name="strChar">需要解码的Html代码</param>
        /// <returns>返回解码的Html代码</returns>
        public static string HtmlDecode(string strChar)
        {
            if (string.IsNullOrEmpty(strChar.Trim()))
            {
                return "";
            }
            strChar = strChar.Replace("&amp;", "&");
            strChar = strChar.Replace("&quot;", "\"");
            strChar = strChar.Replace("&lt;", "<");
            strChar = strChar.Replace("&gt;", ">");
            strChar = strChar.Replace("#apos;", "'");
            return strChar.Trim();
        }

        /// <summary>
        /// 将TexBoxt的文本转换为Html格式表在
        /// </summary>
        /// <param name="Text">来自TextBox的文本</param>
        public static string TextToHtml(string Text)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return "";
            }
            else
            {
                Text = Text.Replace("\r\n", "<br />");
                Text = Text.Replace("  ", "&nbsp;&nbsp;");
            }

            return Text.Trim();
        }

        /// <summary>
        /// 将Html格式转换到TextBox的文本显示
        /// </summary>
        /// <param name="Html">来自HTML编码</param>
        public static string HtmlToText(string Html)
        {
            if (string.IsNullOrEmpty(Html))
            {
                return "";
            }
            else
            {
                Html = Html.Replace("<br />", "\r\n");
                Html = Html.Replace("&nbsp;&nbsp;", "  ");
            }

            return Html.Trim();
        }

        /// <summary>
        /// 格式化Sql语句中的各种出错的符号
        /// </summary>
        /// <param name="str">需格式化的字符串</param>
        /// <returns>返回格式化的字符</returns>
        public static string FormatSql(string str)
        {
            string str2 = "";
            if (!str.Equals(""))
            {
                str2 = str.ToLower().Replace("'", "").Replace("\"", "").Replace("!", "").Replace("~", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("*", "").Replace("|", "").Replace("--", "").Replace("delete", "").Replace("drop", "").Replace("select", "").Replace("where", "").Replace("from", "").Replace("<", "").Replace(">", "").Replace("//", "");
            }
            return str2;
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
        /// HTML代码过滤（标签白表）待完善
        /// </summary>
        /// author:chuck
        /// <param name="Htmlstring">Html代码</param>
        /// <returns>返回过滤过的Html代码</returns>
        public static string ClearHtml_White(string Htmlstring)
        {
            //标签白表
            //Htmlstring = Regex.Replace(Htmlstring, "<div[*]>", "<div[*]>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</div>", "</div>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<ul>", "<ul>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</ul>", "</ul>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<ol>", "<ol>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</ol>", "</ol>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<li[*]>", "<li[*]>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</li>", "</li>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<p>", "<p>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</p>", "</p>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<em>", "<em>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</em>", "</em>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<strong>", "<strong>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</strong>", "</strong>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<a[*]>", "<a[*]>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "</a>", "</a>", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<img[*]>", "<img[*]>", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, "<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, "<(.[^li][^/li][^ol][^/ol][^ul][^/ul][^div][^/div][^p][^/p][^em][^/em][^strong][^/strong][^a][^/a][^img][^>]*)>", "", RegexOptions.IgnoreCase);
            //从web.config配置白标签
            Htmlstring = Regex.Replace(Htmlstring, System.Configuration.ConfigurationManager.AppSettings["WhiteLabel"], "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(iexcl|#161);", "\x00a1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(cent|#162);", "\x00a2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(pound|#163);", "\x00a3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, "&(copy|#169);", "\x00a9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("<.*?>", "");
            Htmlstring.Replace(" ", "");
            Htmlstring.Replace("'", "");
            Htmlstring.Replace("&nbsp;", "");
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "<br />", RegexOptions.IgnoreCase);
            if (HttpContext.Current == null)
                Htmlstring = HttpUtility.HtmlEncode(Htmlstring).Trim();//这样的处理方法貌似不可行，CCM
            else
                Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
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


        /// <summary>
        /// 转化特殊符号为下划线
        /// </summary>
        /// <param name="str">待转化的字符串</param>
        /// <returns>返回被替换过的代码</returns>
        public static string ConvertToUrl(string str)
        {
            str = str.Trim().Replace(" ", "_");
            return str;
        }
    }
}
