using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Base.Utility
{
    /// <summary>
    /// 格式化工具
    /// </summary>
    public class FormatHelper
    {

        /// <summary>
        /// 格式化SQL语句..
        /// </summary>
        /// <param name="str">输入语句</param>
        /// <returns>返回语句</returns>
        string CheckSql(string str)
        {
            if (str == "" || str == null)
            {
                return str;
            }
            else
            {
                string _str;
                _str = str;

                _str = _str.Replace("and", "");
                _str = _str.Replace("exec", "");
                _str = _str.Replace("insert", "");
                _str = _str.Replace("select", "");
                _str = _str.Replace("delete", "");
                _str = _str.Replace("update", "");
                _str = _str.Replace("count", "");
                _str = _str.Replace("*", "");
                _str = _str.Replace("chr", "");
                _str = _str.Replace("mid", "");
                _str = _str.Replace("master", "");
                _str = _str.Replace("truncate", "");
                _str = _str.Replace("char", "");
                _str = _str.Replace("declare", "");

                return _str;
            }
        }

        /// <summary>
        /// 用于格式化RSS样式
        /// </summary>
        /// <param name="input">输入内容</param>
        /// <returns>输出内容</returns>
        public string FormatForXML(object input)
        {
            string data;
            try
            {
                data = input.ToString();
                data = data.Replace("&", "&amp;");
                data = data.Replace("/", "&quot;");
                data = data.Replace("'", "&qapos;");
                data = data.Replace("<", "&lt;");
                data = data.Replace(">", "&gt");
            }
            catch (System.NullReferenceException)
            {
                data = (string)input;
            }
            return data;
        }

        /// <summary>
        /// 将一段字符中以某个字符隔离存储在数组中。
        /// </summary>
        /// <param name="ftext">需要隔离的一段字符</param>
        /// <param name="fchar">分隔的字符</param>
        /// <returns>返回到其存储的数组中</returns>
        public string[] Farray(string ftext, string fchar, out int fcount)
        {
            //通过函数获得数组字符串
            string[] sArray = ftext.Split(char.Parse(fchar));
            int sLong;

            sLong = sArray.Length;

            string[] Fvalue = new string[sLong];

            int i = 0;
            foreach (string j in sArray)
            {
                Fvalue[i] = j.ToString();
                i++;
            }
            fcount = sLong;
            return Fvalue;
        }

        /// <summary>
        /// 格式化当前字符，并裁取其长度...
        /// </summary>
        /// <param name="Ltext">字符类型</param>
        /// <param name="Sizes">长度</param>
        /// <returns>近回裁取后的字符</returns>
        public string Rlength(object Ltext, int Sizes)
        {
            string TempText;
            try
            {
                if (Ltext.ToString().Length > Sizes)
                {
                    TempText = Ltext.ToString().Substring(0, Sizes);
                }
                else
                {
                    TempText = Ltext.ToString();
                }
            }
            catch
            {
                TempText = (string)Ltext;
            }
            return TempText;
        }

        /// <summary>
        /// 格式化当前字符，并裁取其长度...
        /// </summary>
        /// <param name="Ltext">字符类型</param>
        /// <param name="Sizes">长度</param>
        /// <param name="Output">裁取其长度后的标记。</param>
        /// <returns>近回裁取后的字符</returns>
        public string Rlength(object Ltext, int Sizes, string Output)
        {
            string TempText;
            try
            {
                if (Ltext.ToString().Length > Sizes)
                {
                    TempText = Ltext.ToString().Substring(0, Sizes) + Output;
                }
                else
                {
                    TempText = Ltext.ToString();
                }
            }
            catch
            {
                TempText = (string)Ltext;
            }
            return TempText;
        }

        /// <summary>
        /// 格式化文本输入的时间
        /// </summary>
        /// <remarks>
        /// 郭建军：20131015 将类型为1的指定日期格式
        /// </remarks>
        /// <param name="Rvalue">输入的文本时间</param>
        /// <param name="Rtype">类型</param>
        /// <param name="Rtext">显示输出中间字符</param>
        /// <returns>返回格式化的时间</returns>
        public string Rtime(object Rvalue, int Rtype, string Rtext)
        {
            string rtext;
            switch (Rtype.ToString())
            {
                case "0":
                    rtext = DateTime.Parse(Rvalue.ToString()).ToShortDateString().ToString();
                    break;

                case "1":
                    rtext = DateTime.Parse(Rvalue.ToString()).ToString("yyyy-MM-dd");

                    break;

                default:
                    rtext = Rvalue.ToString();
                    break;
            }
            rtext = rtext.Replace("-", Rtext);
            return rtext;
        }

        /// <summary>
        /// 格式化文本输入的时间
        /// </summary>
        /// <param name="Rvalue">输入的文本时间</param>
        /// <param name="Rtype">类型</param>
        /// <returns>返回格式化的时间</returns>
        public string Rtime(object Rvalue, int Rtype)
        {
            string rtext;
            switch (Rtype.ToString())
            {
                case "0":
                    rtext = DateTime.Parse(Rvalue.ToString()).ToShortDateString().ToString();
                    break;

                case "1":
                    rtext = DateTime.Parse(Rvalue.ToString()).ToShortTimeString().ToString();
                    break;

                case "2":
                    rtext = DateTime.Parse(Rvalue.ToString()).Month.ToString() + "-" + DateTime.Parse(Rvalue.ToString()).Day.ToString();
                    break;

                default:
                    rtext = Rvalue.ToString();
                    break;
            }
            return rtext;
        }

        void Slist(CheckBoxList mycheck, object ftext)
        {
            //通过函数获得数组字符串
            string[] sArray = ftext.ToString().Split(',');
            int sLong;

            sLong = sArray.Length;

            string[] Fvalue = new string[sLong];

            int i = 0;
            foreach (string j in sArray)
            {
                Fvalue[i] = j.ToString();
                Slist(mycheck, Fvalue[i]);
                i = i + 1;
            }
        }

        public string Slist(CheckBoxList mycheck)
        {
            string Rtext = null;
            int fcount;
            fcount = mycheck.Items.Count;
            for (int i = 0; i < fcount; i++)
            {
                if (Rtext == null)
                {
                    Rtext = mycheck.Items[i].Value.ToString();
                }
                else
                {
                    Rtext = Rtext + "," + mycheck.Items[i].Value.ToString();
                }
            }

            return Rtext;
        }

        /// <summary>
        /// 格式化当前SelectList，并将符合条件Item进行选择....
        /// </summary>
        /// <param name="WebContr">控件名称...</param>
        /// <param name="SelecedValue">作用其值</param>
        public void Slist(DropDownList WebContr, string SelecedValue)
        {
            foreach (ListItem myItem in WebContr.Items)
            {
                if (myItem.Value.ToString().Trim() == SelecedValue)
                {
                    myItem.Selected = true;
                    return;
                }
                else
                {
                    myItem.Selected = false;
                }
            }
        }


        /// <summary>
        /// 格式化当前ListBox，并将符合条件Item进行选择....
        /// </summary>
        /// <param name="WebContr">控件名称...</param>
        /// <param name="SelecedValue">作用其值</param>
        public void Slist(ListBox WebContr, string SelecedValue)
        {
            foreach (ListItem myItem in WebContr.Items)
            {
                if (myItem.Value.ToString().Trim() == SelecedValue)
                {
                    myItem.Selected = true;
                    return;
                }
                else
                {
                    myItem.Selected = false;
                }
            }
        }

        /// <summary>
        /// 格式化当前CheckBoxList，并将符合条件Item进行选择....
        /// </summary>
        /// <param name="WebContr">控件名称...</param>
        /// <param name="SelecedValue">作用其值</param>
        public void Slist(CheckBoxList WebContr, string SelecedValue)
        {
            foreach (ListItem myItem in WebContr.Items)
            {
                if (myItem.Value.ToString().Trim() == SelecedValue)
                {
                    myItem.Selected = true;
                    return;
                }
                else
                {
                    myItem.Selected = false;
                }
            }
        }


        /// <summary>
        /// 返回CheckList控件是否被选中....
        /// </summary>
        /// <param name="Rlist"></param>
        /// <param name="SelectValue"></param>
        public void Slist(RadioButtonList Rlist, string SelectValue)
        {
            int Counts = Rlist.Items.Count;
            for (int i = 0; i < Counts; i++)
            {
                try
                {
                    if (Rlist.Items[i].Value.ToString() == SelectValue.ToString())
                    {
                        Rlist.Items[i].Selected = true;
                    }
                    else
                    {
                        Rlist.Items[i].Selected = false;
                    }
                }
                catch
                {
                    Rlist.Items[0].Selected = true;
                }
            }
        }


        //来源IDKin----------------------------------------------

        /// <summary>
        /// 将指定的Object值转换为double型值
        /// </summary>
        /// <param name="obj">指定的对象</param>
        /// <returns></returns>
        public static double ToDouble(object obj)
        {
            try
            {
                return double.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 转化为百分数
        /// </summary>
        /// <param name="Dividend">分子</param>
        /// <param name="Divisor">分母</param>
        /// <returns>如;5%</returns>
        public static string GetPercent(double Dividend, double Divisor)
        {
            if (Divisor == 0.0)
                return "";

            if (Dividend == 0.0)
                return "";

            Dividend = Dividend / Divisor * 100;

            if (Dividend > 100.0 || Dividend < 0.0)
                return "<font color=red>" + Dividend.ToString("0.00") + "%</font>";
            else
                return Dividend.ToString("0.00") + "%";
        }

        /// <summary>
        /// 格式化数量及一般数字
        /// </summary>
        /// <param name="NumObj">数字</param>
        /// <returns></returns>        
        public static string FormatNum(object NumObj)
        {
            double NumValue = ToDouble(NumObj);

            if (NumValue < 0.0)
                return "<font color=red>" + NumValue.ToString("#,###.#") + "</font>";
            else if (NumValue == 0.0)
                return "<div align=center>-</div>";
            else
                return NumValue.ToString("#,###.#");
        }

        /// <summary>
        /// 格式化金钱，人民币等
        /// </summary>
        /// <param name="MoneyObj"></param>
        /// <returns></returns>       
        public static string FormatFee(object MoneyObj)
        {
            double MoneyValue = ToDouble(MoneyObj);

            if (MoneyValue < 0.0)
                return "<font color=red>" + MoneyValue.ToString("#,###.00") + "</font>";
            else if (MoneyValue == 0.0)
                return "<div align=center>-</div>";
            else
                return MoneyValue.ToString("#,###.00");
        }

        /// <summary>
        /// Author:zouqi
        /// Description:将输入数据转化为指定类型的数据
        /// </summary>
        /// <typeparam name="T">要转成的特定类型，如int、string、double</typeparam>
        /// <param name="inputValue">输入的值</param>
        /// <returns>转换后的结果</returns>
        public static T ChangeInputType<T>(object inputValue)
        {
            try
            {
                if (inputValue == null || string.IsNullOrEmpty(inputValue.ToString()) || inputValue.ToString().Trim().Length <= 0)
                    return default(T);

                return (T)Convert.ChangeType(inputValue.ToString().TrimEnd().TrimStart(), typeof(T));
            }
            catch (Exception ex)
            { 
                return default(T);
            }
        }
 
    }
}
