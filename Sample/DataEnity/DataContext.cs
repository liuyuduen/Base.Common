using Sample.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sample.DataEntity
{
    public class DataContext : DbContext
    {
        public DataContext()
             : base("DbConnectionString")
        {
        }

        public DbSet<UserInfo> Users { get; set; }
    }
}