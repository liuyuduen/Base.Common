using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Base.Utility
{
    /// <summary>
    /// 类对象反射解析器
    /// </summary>
    public class ObjectReflectHelper
    {
        public static FieldsCollection GetPublicPropertyInfo(object obj)
        {
            PropertyInfo[] info = GetPropertyInfo(obj, obj.GetType(), true);
            FieldsCollection fn = new FieldsCollection();
            if (info != null)
            {
                foreach (PropertyInfo item in info)
                {
                    fn.Fields.Add(new Field
                    {
                        Name = item.Name,
                        Value = item.GetValue(obj, null),
                        Type = null
                    });
                }
            }
            return fn;
        }

        /// <summary>
        /// 获取对象的属性信息
        /// </summary>
        /// <param name="obj">要解析的对象</param>
        /// <param name="isPublic">是否包含公开级别的属性</param>
        /// <returns></returns>
        private static PropertyInfo[] GetPropertyInfo(object obj, Type obj_type, bool isPublic)
        {
            MemberInfo[] info = null;
            if (isPublic)
            {
                info = obj_type.FindMembers(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, null);
            }
            else
            {
                info = obj_type.FindMembers(MemberTypes.Property, BindingFlags.Instance | BindingFlags.NonPublic, null, null);

            }
            List<PropertyInfo> buf = new List<PropertyInfo>();
            foreach (MemberInfo f in info)
            {
                PropertyInfo property1 = f.ReflectedType.GetProperty(f.Name);
                if (property1 != null)
                {
                    buf.Add(property1);
                }
                //object rt = property1.GetValue(obj, null);
            }
            return buf.ToArray();
        }
    }

    /// <summary>
    /// 类成员属性定义
    /// </summary>
    public class Field
    {
        /// <summary>
        /// 获取或设置实体类属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 获取或设置类型名称
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 获取或设置实体对象的值
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// 类成员属性集合
    /// </summary>
    public class FieldsCollection
    {
        /// <summary>
        /// 获取或设置类成员集合
        /// </summary>
        public List<Field> Fields = new List<Field>();

        /// <summary>
        /// 当属性名中带有&lt;&gt;的时候，截取中间的名称，并且返回列表
        /// </summary>
        /// <returns></returns>
        public Field[] GetFieldByProperty()
        {
            List<Field> result = new List<Field>();

            foreach (Field item in Fields)
            {
                Field fd = new Field();
                fd.Value = item.Value;
                fd.Type = item.Type;

                if (item.Name.IndexOf('<') > -1)
                {
                    Match m = Regex.Match(item.Name, "<(.*)?>");
                    fd.Name = m.Groups[1].Value;
                }
                else
                {
                    fd.Name = item.Name;
                }

                result.Add(fd);
            }
            return result.ToArray();
        }
    }
}
