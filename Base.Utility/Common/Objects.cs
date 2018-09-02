using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Base.Utility
{
    public static class Objects
    {
        /// <summary>
        /// 实现浅拷贝两对象相同名称的属性
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity">目标对象</param>
        /// <param name="isNeedNullValue">连同NULL值属性也拷贝时，设置为true</param>
        public static void CopyTo(this object entity, object targetEntity, bool isNeedNullValue)
        {
            PropertyInfo[] entityProperties = entity.GetType().GetProperties();
            PropertyInfo[] targetEntityProperties = targetEntity.GetType().GetProperties();

            IDictionary<string, PropertyInfo> entityPropertiesNamePair = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pInfo in entityProperties)
            {
                if (!pInfo.CanRead) continue;
                entityPropertiesNamePair[pInfo.Name] = pInfo;
            }

            var targetPInfos = from targetPropertyInfo in targetEntityProperties
                               where entityPropertiesNamePair.Keys.Contains(targetPropertyInfo.Name)
                               select targetPropertyInfo;
            foreach (PropertyInfo targetPropertyInfo in targetPInfos)
            {
                if (!targetPropertyInfo.CanWrite) continue;
                object entityPInfoValue = entityPropertiesNamePair[targetPropertyInfo.Name].GetValue(entity, null);
                if ((isNeedNullValue) || (entityPInfoValue != null))
                {
                    targetPropertyInfo.SetValue(targetEntity, entityPInfoValue, null);
                }
            }
        }

        /// <summary>
        /// 实现浅拷贝两对象相同名称的属性，null值属性不拷贝
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity"></param>
        public static void CopyTo(this object entity, object targetEntity)
        {
            CopyTo(entity, targetEntity, false);
        }

        /// <summary>
        /// 读取DataRow["Column"]的值,忽略掉DBNull
        /// </summary>
        public static string IgnoreDbNull(this object obj)
        {
            return obj == System.DBNull.Value ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 如果对象为Null则返回string.Empty
        /// </summary>
        public static string Null2StringEmpty(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return obj.ToString();
        }


        /// <summary>
        /// 将输入数据转化为指定类型的数据
        /// </summary>
        /// <typeparam name="T">要转成的特定类型，如int、string、double</typeparam>
        /// <param name="obj">输入的值</param>
        /// <returns>转换后的结果</returns>
        public static T To<T>(this object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()) || obj.ToString().Trim().Length <= 0)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(obj.ToString().TrimEnd().TrimStart(), typeof(T));
        }
    }
}
