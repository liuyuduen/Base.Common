using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Base.Utility
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public class SerializerTool
    {
        /// <summary>
        /// 创建XmlWriter
        /// </summary>
        /// <param name="output">流对象</param>
        /// <returns>返回XmlWriter</returns>
        public static XmlWriter CreateXmlWriter(object output)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = true;
            settings.CloseOutput = true;
            if (output is string)
            {
                return XmlWriter.Create(output as string, settings);
            }
            else if (output is System.Text.StringBuilder)
            {
                return XmlWriter.Create(output as System.Text.StringBuilder, settings);
            }
            else if (output is TextWriter)
            {
                return XmlWriter.Create(output as TextWriter, settings);
            }

            return null;
        }

        /// <summary>
        /// 创建XmlReader
        /// </summary>
        /// <param name="output">流对象</param>
        /// <returns>返回XmlReader</returns>
        public static XmlReader CreateXmlReader(object output)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ValidationType = ValidationType.Schema;
            if (output is string)
            {
                return XmlReader.Create(output as string, settings);
            }
            else if (output is TextReader)
            {
                return XmlReader.Create(output as TextReader, settings);
            }

            return null;
        }

        /// <summary>
        /// 反序列化为对象

        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="content">要反序列化字符串的对象</param>
        /// <returns>返回反序列化的对象</returns>
        public static T DeserializeFromString<T>(
            Type type,
            string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return default(T);
            }

            using (TextReader reader = new StringReader(content))
            {
                using (XmlReader reader2 = SerializerTool.CreateXmlReader(reader))
                {
                    XmlSerializer xz = new XmlSerializer(type);
                    return (T)xz.Deserialize(reader2);
                }
            }
        }

        /// <summary>
        /// 反序列化为对象

        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="xmlContent">要反序列化XmlDocument的对象</param>
        /// <returns>返回反序列化的对象</returns>
        public static T DeserializeFromXMLDocument<T>(
            Type type,
            XmlDocument xmlContent)
        {
            if (xmlContent == null)
            {
                return default(T);
            }

            using (TextReader reader = new StringReader(xmlContent.OuterXml))
            {
                using (XmlReader reader2 = SerializerTool.CreateXmlReader(reader))
                {
                    XmlSerializer xz = new XmlSerializer(type);
                    return (T)xz.Deserialize(reader2);
                }
            }
        }


        /// <summary>
        /// 序列化对象

        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="type">序列的对象</param>
        /// <returns>返回序列化的字符串内容</returns>
        public static string SerializeToString<T>(T type)
        {
            if (type == null)
            {
                return null;
            }


            System.Text.StringBuilder xmContent = new System.Text.StringBuilder();
            using (XmlWriter sw = SerializerTool.CreateXmlWriter(xmContent))
            {
                XmlSerializer xz = new XmlSerializer(typeof(T));

                xz.Serialize(sw, type);
            }

            return xmContent.ToString();
        }


        /// <summary>
        /// 序列化对象到XMLDocument
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="type">序列的对象</param>
        /// <returns>返回序列化的XMLDocument</returns>
        public static XmlDocument SerializeToXMLDocument<T>(T type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            System.Text.StringBuilder xmContent = new System.Text.StringBuilder();
            using (XmlWriter sw = SerializerTool.CreateXmlWriter(xmContent))
            {
                XmlSerializer xz = new XmlSerializer(typeof(T));
                xz.Serialize(sw, type);
            }

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmContent.ToString());
            return xd;
        }
    }
}
