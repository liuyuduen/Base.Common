
using Base.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Base.Utility
{
    /// <summary>
    /// xml处理类
    /// </summary>
    public class XmlHelper
    {
        // 定义XML文档
        private XmlDocument doc = null;
        // 定义路径
        private string fileName;

        /// <summary>
        /// XML文件路径
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        public XmlHelper(string fileName)
        {
            if (File.Exists(fileName))
            {
                // 产生XML文档
                doc = new XmlDocument();
                // 加载文件
                doc.Load(fileName);

                // 保存文件路径
                this.fileName = fileName;
            }
            else
            {
                throw new FileNotFoundException(fileName + "，XML文件不存在");
            }
        }

        // 保存文档
        public void Save()
        {
            // 保存
            doc.Save(fileName);
        }

        /// <summary>
        /// 设置XML文件某节点的值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeValue">节点值</param>
        public void SetContent(string nodeName, string nodeValue)
        {
            // 获得节点
            XmlNode node = GetNode(nodeName);

            // 设置值
            node.InnerText = nodeValue;
        }

        /// <summary>
        /// 获得XML文件某节点的值
        /// </summary>
        /// <param name="fileName">XML文件名</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点值</returns>
        public string GetContent(string nodeName)
        {
            // 获得指定的节点并获得值
            return GetNode(nodeName).InnerText;
        }


        private XmlNode GetNode(string nodeName)
        {
            // 查询节点
            XmlNode node = doc.SelectSingleNode("//" + nodeName);

            if (node == null)
            {
                throw new ApplicationException(nodeName + "，XML节点不存在");
            }
            else
            {
                return node;
            }
        }


        public static object Extension { get; private set; }
        #region SetInnerText
        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="filePath">xml 文件的绝对路径</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="innerText">innerText 值</param>
        /// <returns>是否成功</returns>
        public static bool SetInnerText(string filePath, string tagName, string innerText)
        {
            bool resultValue = true;

            if (!String.IsNullOrEmpty(filePath))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);//取得 xml 文件

                    resultValue = SetInnerText(xmlDoc, tagName, innerText);
                    if (resultValue)
                        xmlDoc.Save(filePath);
                }
                catch (Exception ex)
                {
                    Log.Error("设置“" + tagName + "”标签的 innerText 出错", ex);
                    resultValue = false;
                }
            }

            return resultValue;
        }

        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="xmlDoc">xml 内容</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="innerText">innerText 值</param>
        /// <returns>是否成功执行操作</returns>
        public static bool SetInnerText(XmlDocument xmlDoc, string tagName, string innerText)
        {
            try
            {
                //遍历各个节点，取出属性值
                if (xmlDoc != null && !String.IsNullOrEmpty(tagName) && !String.IsNullOrEmpty(innerText))
                {
                    foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
                    {
                        SetInnerText(xmlNode, tagName, innerText);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("设置“" + tagName + "”标签的 innerText 出错", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="parentNode">xml 节点</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="innerText">innerText 值</param>
        private static void SetInnerText(XmlNode parentNode, string tagName, string innerText)
        {
            //设置当前节点的innerText
            if (parentNode.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
                parentNode.InnerText = innerText;

            //设置当前节点的所有子节点的innerText
            foreach (XmlNode xmlNode in parentNode.ChildNodes)
            {
                SetInnerText(xmlNode, tagName, innerText);
            }
        }
        #endregion

        #region GetInnerText
        /// <summary>
        /// 取得标签的innerText
        /// </summary>
        /// <param name="filePath">xml 文件的绝对路径</param>
        /// <param name="tagName">标签名称</param>
        /// <returns>innerText集合</returns>
        public static List<string> GetInnerText(string filePath, string tagName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.Load(filePath);//取得 xml 文件
                }
                catch (Exception ex)
                {
                    Log.Error("取得“" + tagName + "”标签的 innerText 出错", ex);
                }

                return GetInnerText(xmlDoc, tagName);
            }

            return new List<string>();
        }

        /// <summary>
        /// 取得标签的innerText
        /// </summary>
        /// <param name="xmlDoc">xml 文件内容</param>
        /// <param name="tagName">标签名称</param>
        /// <returns>innerText 集合</returns>
        public static List<string> GetInnerText(XmlDocument xmlDoc, string tagName)
        {
            List<string> innerTextList = new List<string>();//innerText集合

            try
            {
                //遍历各个节点，取出属性值
                if (xmlDoc != null && !String.IsNullOrEmpty(tagName) && innerTextList != null)
                {
                    foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
                    {
                        GetInnerText(xmlNode, tagName, innerTextList);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("取得“" + tagName + "”标签的 innerText 出错", ex);
            }

            return innerTextList;
        }

        /// <summary>
        /// 取得标签的innerText
        /// </summary>
        /// <param name="parentNode">xmlNode</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="innerTextList">innerText 集合</param>
        private static void GetInnerText(XmlNode parentNode, string tagName, List<string> innerTextList)
        {
            //判断当前节点是否存在innerText，并取得innerText
            if (parentNode.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
            {
                if (innerTextList != null && !String.IsNullOrEmpty(parentNode.InnerText))
                    innerTextList.Add(parentNode.InnerText);
            }

            //判断当前节点的所有子节点是否innerText，并取得innerText
            foreach (XmlNode xmlNode in parentNode.ChildNodes)
            {
                GetInnerText(xmlNode, tagName, innerTextList);
            }
        }
        #endregion

        #region GetAttribute
        /// <summary>
        /// 取得标签的属性值
        /// </summary>
        /// <param name="filePath">xml 文件的绝对路径</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值集合</returns>
        public static List<string> GetAttribute(string filePath, string tagName, string attributeName)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();

                try
                {
                    xmlDoc.Load(filePath);//取得 xml 文件
                }
                catch (Exception ex)
                {
                    Log.Error("取得“" + tagName + "”标签的“" + attributeName + "”属性值出错", ex);
                }

                return GetAttribute(xmlDoc, tagName, attributeName);
            }

            return new List<string>();
        }

        /// <summary>
        /// 取得标签的属性值
        /// </summary>
        /// <param name="xmlDoc">xml 文件内容</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值集合</returns>
        public static List<string> GetAttribute(XmlDocument xmlDoc, string tagName, string attributeName)
        {
            List<string> attributeList = new List<string>();//属性集合

            try
            {
                //遍历各个节点，取出属性值
                if (xmlDoc != null && !String.IsNullOrEmpty(tagName) && !String.IsNullOrEmpty(attributeName) && attributeList != null)
                {
                    foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
                    {
                        GetAttribute(xmlNode, tagName, attributeName, attributeList);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("取得“" + tagName + "”标签的“" + attributeName + "”属性值出错", ex);
            }

            return attributeList;
        }

        /// <summary>
        /// 取得标签的属性值
        /// </summary>
        /// <param name="parentNode">xml 节点</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeList">属性值集合</param>
        private static void GetAttribute(XmlNode parentNode, string tagName, string attributeName, List<string> attributeList)
        {
            XmlAttribute attribute = null;//属性值

            //判断当前节点是否存在此属性，并取得属性值
            if (parentNode.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
            {
                attribute = parentNode.Attributes[attributeName];

                if (attribute != null && !String.IsNullOrEmpty(attribute.Value) && attributeList != null)
                    attributeList.Add(attribute.Value);
            }

            //判断当前节点的所有子节点是否存在此属性，并取得属性值
            foreach (XmlNode xmlNode in parentNode.ChildNodes)
            {
                GetAttribute(xmlNode, tagName, attributeName, attributeList);
            }
        }
        #endregion

        #region SetAttrbute
        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="filePath">xml 文件的绝对路径</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">属性值</param>
        /// <returns>是否成功</returns>
        public static bool SetAttribute(string filePath, string tagName, string attributeName, string attributeValue)
        {
            bool resultValue = true;

            if (!String.IsNullOrEmpty(filePath))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);//取得 xml 文件

                    resultValue = SetAttribute(xmlDoc, tagName, attributeName, attributeValue);
                    if (resultValue)
                        xmlDoc.Save(filePath);
                }
                catch (Exception ex)
                {
                    Log.Error("设置“" + tagName + "”标签的“" + attributeName + "”属性值(" + attributeValue + ")出错", ex);
                    resultValue = false;
                }
            }

            return resultValue;
        }

        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="xmlDoc">xml 内容</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">属性值</param>
        /// <returns>是否成功执行操作</returns>
        public static bool SetAttribute(XmlDocument xmlDoc, string tagName, string attributeName, string attributeValue)
        {
            try
            {
                //遍历各个节点，设置属性值
                if (xmlDoc != null && !String.IsNullOrEmpty(tagName) && !String.IsNullOrEmpty(attributeName) && !String.IsNullOrEmpty(attributeValue))
                {
                    foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes)
                    {
                        SetAttribute(xmlNode, tagName, attributeName, attributeValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("设置“" + tagName + "”标签的“" + attributeName + "”属性值(" + attributeValue + ")出错", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置标签的属性值
        /// </summary>
        /// <param name="parentNode">xml 节点</param>
        /// <param name="tagName">标签名称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeValue">属性值</param>
        private static void SetAttribute(XmlNode parentNode, string tagName, string attributeName, string attributeValue)
        {
            XmlAttribute attribute;//属性值

            //判断当前节点是否存在此属性，并设置属性值
            if (parentNode.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
            {
                attribute = parentNode.Attributes[attributeName];

                if (attribute == null)
                    ((XmlElement)parentNode).SetAttribute(attributeName, attributeValue);
                else
                    attribute.Value = attributeValue;
            }

            //判断当前节点的所有子节点是否存在此属性，并设置属性值
            foreach (XmlNode xmlNode in parentNode.ChildNodes)
            {
                SetAttribute(xmlNode, tagName, attributeName, attributeValue);
            }
        }
        #endregion

        /// <summary>
        /// 转换 xml 文件内容为 html
        /// </summary>
        /// <param name="str">要转换的 xml</param>
        /// <returns></returns>
        public static string EnCodeXmlStr(string str)
        {
            return str.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

    }
}
