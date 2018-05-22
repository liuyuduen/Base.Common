using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base.Utility
{

    public static class EntityObjectExtentions
    {
        #region data to entity hepers

        public static T ToObject<T>(this IDataReader dataReader, bool strict = true)
        {
            if (dataReader == null)
            {
                return default(T);
            }

            var cols = new List<string>();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                cols.Add(dataReader.GetName(i));
            }

            // Create a new type of the entity
            Type t = typeof(T);
            T returnObject = Activator.CreateInstance<T>();

            PropertyInfo[] perperties = t.GetProperties();
            foreach (PropertyInfo prop in perperties)
            {
                if (!prop.CanWrite)
                    continue;

                object val = null;
                if (cols.Contains(prop.Name))
                {
                    val = dataReader[prop.Name];
                }
                else
                {
                    // 严格模式，无法在结果集中找到指定的列时抛出异常
                    if (strict)
                        throw new IndexOutOfRangeException(string.Format("Can not found column: {0}", prop.Name));
                    else
                        continue;
                }

                SetPropertyValue(returnObject, prop, val);
            }

            return returnObject;
        }
        public static T ToObject<T>(this DataRow dataRow)
        {
            if (dataRow == null)
            {
                return default(T);
            }

            // Create a new type of the entity
            Type t = typeof(T);
            T returnObject = Activator.CreateInstance<T>();

            foreach (DataColumn col in dataRow.Table.Columns)
            {
                string colName = col.ColumnName;

                // Look for the object's property with the columns name, ignore case
                PropertyInfo pInfo = t.GetProperty(colName.ToLower(),
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // did we find the property and can write ?
                if (pInfo == null || !pInfo.CanWrite)
                {
                    continue;
                }

                object val = dataRow[colName];

                SetPropertyValue(returnObject, pInfo, val);
            }

            // return the entity object with values
            return returnObject;
        }
        public static T ToObject<T>(this DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return default(T);
            }

            DataRow dataRow = dataTable.Rows[0];

            return dataRow.ToObject<T>();
        }
        public static List<T> ToObjectCollection<T>(this DataTable dataTable)
        {

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return dataTable.Rows.ToObjectCollection<T>();
            }

            return new List<T>();
        }
        public static List<T> ToObjectCollection<T>(this DataRowCollection dataRows)
        {
            List<T> collection = new List<T>();

            if (dataRows != null && dataRows.Count > 0)
            {
                foreach (DataRow row in dataRows)
                {
                    collection.Add(row.ToObject<T>());
                }
            }

            return collection;
        }
        private static void SetPropertyValue(object obj, PropertyInfo property, object value)
        {
            // is this a Nullable<> type
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            if (underlyingType != null)
            {
                if (value is System.DBNull)
                {
                    value = null;
                }
                else
                {
                    // Convert the db type into the T we have in our Nullable<T> type
                    value = Convert.ChangeType(value, underlyingType);
                }
            }
            else
            {
                // Convert the db type into the type of the property in our entity
                value = Convert.ChangeType(value, property.PropertyType);
            }

            // Set the value of the property with the value from the db
            property.SetValue(obj, value, null);
        }

        #endregion
    }
}
