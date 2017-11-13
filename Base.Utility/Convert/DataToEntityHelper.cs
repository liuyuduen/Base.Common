using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base.Utility
{
    public class DataToEntityHelper<T> where T : new()
    {

        /// <summary>
        /// Hashtable转换实体类
        /// </summary>
        public static T HashtableToModel<T>(DataRow dtRow)
        {
            T model = Activator.CreateInstance<T>();
            Type type = model.GetType();
            //遍历每一个属性
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = dtRow[prop.Name];
                if (prop.PropertyType.ToString() == "System.Nullable`1[System.DateTime]")
                {
                    value = Convert.ToDateTime(value);
                }
                prop.SetValue(model, prop.PropertyType, null);
            }
            return model;
        }

        public static List<T> DataTableToList(DataTable dt)
        {
            // 定义集合
            List<T> ts = new List<T>();

            string tempName = "";

            // 获得此模型的公共属性
            PropertyInfo[] propertys = typeof(T).GetProperties();

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();

                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;

                    // 检查DataTable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }

                ts.Add(t);
            }

            return ts;
        }

        /// <summary>
        /// 将 List 转换成 dataTable
        /// </summary>
        /// <typeparam name="T">要转换成的对象</typeparam>
        /// <param name="objList">要转换的List</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(List<T> objList)
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

    }
}
