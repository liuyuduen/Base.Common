using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeaRun.Utilities.Base.Extension
{
    public interface ICustomerProvider
    {
        /// <summary>
        /// 写入登录信息
        /// </summary>
        /// <param name="user">成员信息</param>
        void AddCurrent(ICustomer user);
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        ICustomer Current();
        /// <summary>
        /// 删除当前用户
        /// </summary>
        void EmptyCurrent();
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns>True:过期</returns>
        bool IsOverdue();
    }
}
