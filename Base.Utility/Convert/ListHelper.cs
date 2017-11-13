using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Base.Utility
{
    /// <summary>
    /// List 操作(dataTable、dataSet 等之间的转换)
    /// </summary>
    public class ListHelper
    {
        /// <summary>
        /// 将 dataTable 转换成 List
        /// </summary>
        /// <typeparam name="T">要转换成的对象</typeparam>
        /// <param name="dataTable">要转换的DataTable</param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(DataTable dataTable)
        {
            T obj = default(T);
            List<T> objList = new List<T>();
            PropertyInfo[] objProperties = null;
            string propertieName = null;

            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    obj = Activator.CreateInstance<T>();
                    objProperties = obj.GetType().GetProperties();
                    foreach (PropertyInfo properties in objProperties)
                    {
                        if (properties != null)
                            propertieName = properties.Name;

                        if (propertieName != null && dataTable.Columns.Contains(propertieName))
                        {
                            object value = row[propertieName];
                            if (value.GetType() == typeof(System.DBNull))
                                value = null;

                            properties.SetValue(obj, value, null);
                        }
                    }

                    objList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objList;
        }

        /// <summary>
        /// 将 List 转换成 dataTable
        /// </summary>
        /// <typeparam name="T">要转换成的对象</typeparam>
        /// <param name="objList">要转换的List</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(List<T> objList)
        {
            if (objList == null || objList.Count <= 0)
                return null;

            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] objProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            try
            {
                DataColumn column;
                DataRow row;
                PropertyInfo property;
                string propertyName = null;

                foreach (T obj in objList)
                {
                    if (obj == null)
                        continue;

                    row = dt.NewRow();
                    for (int i = 0, j = objProperties.Length; i < j; i++)
                    {
                        property = objProperties[i];
                        propertyName = property.Name;

                        if (propertyName != null && dt.Columns[propertyName] == null)
                        {
                            column = new DataColumn(propertyName, property.PropertyType);
                            dt.Columns.Add(column);
                        }

                        row[propertyName] = property.GetValue(obj, null);
                    }

                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }
        
        /// <summary>
        /// 将 List 转换成 dataTable
        /// </summary>
        /// <typeparam name="T">要转换成的对象</typeparam>
        /// <param name="objList">要转换的objList</param>
        /// <returns></returns>
        public static DataSet ConvertToDataSet<T>(List<T> objList)
        {
            if (objList == null || objList.Count <= 0)
                return null;

            DataSet ds = new DataSet();

            try
            {
                DataTable dt = ConvertToDataTable(objList);
                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

    }
}
