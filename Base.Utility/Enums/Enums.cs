using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility
{

    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// 数据库类型：Oracle
        /// </summary>
        Oracle,
        /// <summary>
        /// 数据库类型：SqlServer
        /// </summary>
        SqlServer,
        /// <summary>
        /// 数据库类型：Access
        /// </summary>
        Access,
        /// <summary>
        /// 数据库类型：MySql
        /// </summary>
        MySql,
        /// <summary>
        /// 数据库类型：SQLite
        /// </summary>
        SQLite
    }

    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatus
    {
        Enable = 0,
        Locking = 1,
        Disable = 2,
        Delete = 3
    }

    /// <summary>
    /// 客户端
    /// </summary>
    public enum UserAgent : uint
    {
        /// <summary>
        /// Windows桌面系统（电脑下单）
        /// </summary>
        Windows_NT = 0,

        /// <summary>
        /// android系统
        /// </summary>
        Android = 1,

        /// <summary>
        /// iPhone IOS
        /// </summary>
        iPhone = 2,

        /// <summary>
        /// iPod IOS
        /// </summary>
        iPod = 3,

        /// <summary>
        /// iPad IOS
        /// </summary>
        iPad = 4,

        /// <summary>
        /// Windows Phone
        /// </summary>
        Windows_Phone = 5,

        /// <summary>
        /// QQ手机浏览器
        /// </summary>
        MQQBrowser = 6,

        /// <summary>
        /// 塞班系统
        /// </summary>
        Symbian = 7,

        /// <summary>
        /// 苹果桌面系统（电脑下单）
        /// </summary>
        Macintosh = 8,

        /// <summary>
        /// SAMSUNG/HTC MP7
        /// </summary>
        compatible = 9
    }
}
