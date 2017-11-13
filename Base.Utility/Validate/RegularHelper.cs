using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Base.Utility
{
    /// <summary>
    /// 正则表达式的相关操作
    /// </summary>
    public class RegularTool
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <param name="regularStr">正则表达式</param>
        /// <param name="validStr">要进行验证的str</param>
        /// <returns>是否符合正则表达式</returns>
        public static bool Regular(string regularStr, string validStr)
        {
            Regex re = null;
            re = new Regex(regularStr);
            if (re.Match(validStr).Success)
                return true;

            return false;
        }

    }

}
