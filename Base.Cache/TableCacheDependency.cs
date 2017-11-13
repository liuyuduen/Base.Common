using Base.Utility;
using System; 
using System.Web.Caching;

namespace Base.Cache
{

    public class TableCacheDependency
    {

        /// <summary>
        /// 获得某个表的依赖项集合
        /// </summary>
        /// <param name="configName">配置文件名称</param>
        /// <returns>依赖项集合</returns>
        public static AggregateCacheDependency GetTableDependency(string configName)
        {

            // 定义依赖项集合
            AggregateCacheDependency cd = new AggregateCacheDependency();
            // 获得缓存数据库名称
            string dbName = ConfigHelper.CacheDBName;
            // 获得表名称集合
            string[] tables = ConfigHelper.AppSettings(configName).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // 循环添加依赖项
            foreach (string tbName in tables)
            {
                cd.Add(new SqlCacheDependency(dbName, tbName));
            }
            return cd;
        }

        public static AggregateCacheDependency GetUserDependency()
        {
            return GetTableDependency("UserDataDependency");
        }

        public static AggregateCacheDependency GetStudentDependency()
        {
            return GetTableDependency("StudentDataDependency");
        }
    }
}
