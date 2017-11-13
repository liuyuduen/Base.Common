using Base.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base.Utility
{
    public static class EnumHelper
    {

        /// <summary>
        /// 转换成对应的枚举项
        /// </summary>
        public static T ToEnum<T>(this string value)
        {
            T oOut = default(T);
            Type t = typeof(T);
            foreach (FieldInfo fi in t.GetFields())
            {
                if (StringHelper.Matches(fi.Name, value))
                    oOut = (T)fi.GetValue(null);
            }

            return oOut;
        }
    }
}
