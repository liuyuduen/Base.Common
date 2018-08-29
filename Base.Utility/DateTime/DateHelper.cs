using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility
{
    /// <summary>
    /// 日期/时间类工具
    /// </summary>
    public class DateHelper
    {

        /// <summary>
        /// 返回两个日期的时间差
        /// </summary>
        /// <param name="datepart">DatePart枚举值</param>
        /// <param name="starttime">起始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <returns></returns>
        public static long DateDiff(DatePart datepart, DateTime starttime, DateTime endtime)
        {
            long rtn = 0;
            TimeSpan start = new TimeSpan(starttime.Ticks);
            TimeSpan end = new TimeSpan(endtime.Ticks);
            TimeSpan delta = end.Subtract(start);
            long year = endtime.Year - starttime.Year;
            long month = year * 12 + (endtime.Month - starttime.Month);
            long day = (long)delta.TotalDays;
            long hour = (long)delta.TotalHours;
            long minute = (long)delta.TotalMinutes;
            long second = (long)delta.TotalSeconds;
            long milliseconds = (long)delta.TotalMilliseconds;
            switch (datepart)
            {
                case DatePart.YY:
                    rtn = year;
                    break;
                case DatePart.MM:
                    rtn = month;
                    break;
                case DatePart.DD:
                    rtn = day;
                    break;
                case DatePart.HH:
                    rtn = hour;
                    break;
                case DatePart.MI:
                    rtn = minute;
                    break;
                case DatePart.SS:
                    rtn = second;
                    break;
                case DatePart.MS:
                    rtn = milliseconds;
                    break;
            }
            return rtn;
        }
        /// <summary>
        /// 获取中文星期
        /// </summary>
        /// <param name="Week">英文星期，如：monday</param>
        /// <returns>一、二、三...日</returns>
        public static string GetChsWeek(string Week)
        {
            Week = Week.ToLower();
            switch (Week)
            {
                case "monday":
                    Week = "一";
                    break;
                case "tuesday":
                    Week = "二";
                    break;
                case "wednesday":
                    Week = "三";
                    break;
                case "thursday":
                    Week = "四";
                    break;
                case "friday":
                    Week = "五";
                    break;
                case "saturday":
                    Week = "六";
                    break;
                case "sunday":
                    Week = "日";
                    break;
                default:
                    Week = "";
                    break;
            }

            return Week;
        }

        /// <summary>
        /// 获取英文月份
        /// </summary>
        /// <param name="Week">英文星期，如：monday</param>
        /// <returns>一、二、三...日</returns>
        public static string GetEnMonth(int month)
        {
            string strEnMonth = string.Empty;
            switch (month)
            {
                case 1:
                    strEnMonth = "January";
                    break;
                case 2:
                    strEnMonth = "February";
                    break;
                case 3:
                    strEnMonth = "March";
                    break;
                case 4:
                    strEnMonth = "April";
                    break;
                case 5:
                    strEnMonth = "May";
                    break;
                case 6:
                    strEnMonth = "June";
                    break;
                case 7:
                    strEnMonth = "July";
                    break;
                case 8:
                    strEnMonth = "August";
                    break;
                case 9:
                    strEnMonth = "September";
                    break;
                case 10:
                    strEnMonth = "October";
                    break;
                case 11:
                    strEnMonth = "November";
                    break;
                case 12:
                    strEnMonth = "December";
                    break;
                default:
                    strEnMonth = "";
                    break;
            }

            return strEnMonth;
        }

        /// <summary>
        /// 获得今天星期几的数字
        /// </summary>
        /// <returns></returns>
        public static string GetWeek()
        {
            return DateTime.Now.DayOfWeek.ToString("d");
        }

        public static DateTime MinValue
        {
            get { return Convert.ToDateTime("1753-01-01"); }
        }

        /// <summary>
        /// 获得时间的唯一值(年月日时分秒)
        /// </summary>
        public static string TimeGuid
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmss"); }
        }

        /// <summary>
        /// 无效日期，专用于插入数据库的日期无效值
        /// </summary>
        public static DateTime InvalidDateTime
        {
            get { return Convert.ToDateTime("1800-08-08"); }
        }

        /// <summary>
        /// 是否无效日期
        /// </summary>
        /// <param name="dt">需要判断的日期</param>
        /// <returns>bool</returns>
        public static bool IsInvalidDateTime(Object dt)
        {
            return IsInvalidDateTime(Convert.ToDateTime(dt));
        }

        /// <summary>
        /// 是否无效日期
        /// </summary>
        /// <param name="dt">需要判断的日期</param>
        /// <returns>bool</returns>
        public static bool IsInvalidDateTime(DateTime dt)
        {

            return (dt.ToString("yyyy-MM-dd") == InvalidDateTime.ToString("yyyy-MM-dd"));
        }
    }
    /// <summary>
    /// 日期枚举值
    /// </summary>
    public enum DatePart
    {
        /// <summary>
        /// 年
        /// </summary>
        YY,
        /// <summary>
        /// 月
        /// </summary>
        MM,
        /// <summary>
        /// 日
        /// </summary>
        DD,
        /// <summary>
        /// 时
        /// </summary>
        HH,
        /// <summary>
        /// 分
        /// </summary>
        MI,
        /// <summary>
        /// 秒
        /// </summary>
        SS,
        /// <summary>
        /// 毫秒
        /// </summary>
        MS
    }
}
