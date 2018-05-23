using Base.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Entity
{
    /// <summary>
    /// 创建一个EF实体框架 上下文
    /// </summary>
    public partial class DataContext : DbContext
    {
        public DataContext()
            : base(ConfigHelper.DB_CONNECTION_STRING)
        {
        }
        public DbSet<T_User> T_User { get; set; }

    }
}
