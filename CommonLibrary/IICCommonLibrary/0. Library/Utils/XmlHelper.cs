/*
 * uu II XmlHelper
 * used in XmlEncode
 * 
 * gaolei@uu.bjmcc.net
 * created:	2006-04-25
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Imps.Services.CommonV4
{
    /// <summary>
    ///	    Xml Encode
    /// </summary>
    public static class XmlHelper
    {

        public static string Encode(string s)
        {
            return Encode(s, true);
        }

        //
        // Any occurrence of & must be replaced by &amp;
        // Any occurrence of < must be replaced by &lt;
        // Any occurrence of > must be replaced by &gt;
        // Any occurrence of " (double quote) must be replaced by &quot;
        // Any occurrence of ' (simple quote) must be replaced by &apos;
        // Any character not in {#x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD]} must be masked or replace with &#x...
        public static string Encode(string s, bool maskInvaildCharacter)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            StringBuilder ret = null;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];

                string ts = null;
                bool invalidChar = false;

                if ((ch >= 0x20 && ch <= 0xD7FF) || (ch >= 0xe000 && ch <= 0xfffd) || ch == '\t' || ch == '\r' || ch == '\n')
                {
                    switch (ch)
                    {
                        case '<':
                            ts = "&lt;";
                            break;
                        case '>':
                            ts = "&gt;";
                            break;
                        case '\"':
                            ts = "&quot;";
                            break;
                        case '\'':
                            ts = "&apos;";
                            break;
                        case '&':
                            ts = "&amp;";
                            break;
                        case '\u000a':
                            ts = "&#xa;";
                            break;
                        case '\u000d':
                            ts = "&#xd;";
                            break;
                        default:
                            // null
                            break;
                    }
                }
                else
                {
                    // invaild character
                    invalidChar = true;
                }

                if (ret == null && (ts != null || invalidChar))
                {
                    ret = new StringBuilder();
                    ret.Append(s.Substring(0, i));
                }

                if (ts != null)
                {
                    ret.Append(ts);
                }
                else if (invalidChar)
                {
                    if (!maskInvaildCharacter)
                        ret.AppendFormat("&#x{0:x};", (ushort)ch);
                }
                else if (ret != null)
                {
                    ret.Append(ch);
                }
            }

            if (ret == null)
                return s;
            else
                return ret.ToString();
        }

        // 
        // Mask Invalid Characters
        // Character not in {#x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD]} will be masked
        public static string MaskInvalidCharacters(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            StringBuilder ret = null;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];

                bool invalidChar = !(
                    (ch >= 0x20 && ch <= 0xD7FF) ||
                    (ch >= 0xe000 && ch <= 0xfffd) ||
                    ch == '\t' ||
                    ch == '\r' ||
                    ch == '\n');

                if (ret == null && invalidChar)
                {
                    ret = new StringBuilder();
                    ret.Append(s.Substring(0, i));
                }

                if (!invalidChar && ret != null)
                {
                    ret.Append(ch);
                }
            }

            if (ret == null)
                return s;
            else
                return ret.ToString();
        }

        /// <summary>
        /// 将html格式文本转换成纯文本格式
        /// </summary>
        /// <param name="htmlText">html格式文本</param>
        /// <returns>转换后的纯文本</returns>
        public static String HtmlToText(String htmlText)
        {
            StringBuilder text = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader("<r>" + htmlText + "</r>")))
            {
                try
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Text:
                                text.Append(reader.Value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (XmlException ex)
                {
                    throw new XmlParseException("Xml has syntax error, parse failed.", ex);
                }
            }
            return text.ToString();
        }

        /// <summary>
        /// 读取节点中某一个属性的值。如果attribute为空，则返回整个节点的InnerText，否则返回具体attribute的值
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">节点中的属性</param>
        /// <returns>如果attribute为空，则返回整个节点的InnerText，否则返回具体attribute的值</returns>
        /// 使用实例: XMLHelper.Read(path, "PersonF/person[@Name='Person2']", "");
        ///  XMLHelper.Read(path, "PersonF/person[@Name='Person2']", "Name");
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return value;
        }

        /// <summary>
        /// 向节点中增加节点元素，属性
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">要操作的节点</param>
        /// <param name="element">要增加的节点元素，可空可不空。非空时插入新的元素，否则插入该元素的属性</param>
        /// <param name="attribute">要增加的节点属性，可空可不空。非空时插入元素值，否则插入元素值</param>
        /// <param name="value">要增加的节点值</param>
        /// 使用实例：XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                //如果element，则增加该属性 
                if (string.IsNullOrEmpty(element))
                {
                    //如果attribute不为空，增加该属性
                    if (!string.IsNullOrEmpty(attribute))
                    {

                        XmlElement xe = (XmlElement)xn;
                        // <person Name="Person2" ID="88"> XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
                        xe.SetAttribute(attribute, value);
                    }
                }
                else //如果element不为空，则preson下增加节点   
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (string.IsNullOrEmpty(attribute))
                        // <person><Num>88</Num></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
                        xe.InnerText = value;
                    else
                        // <person> <Num ID="88" /></person>  XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 修改节点值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">要修改的节点</param>
        /// <param name="attribute">属性名，非空时修改节点的属性值，否则修改节点值</param>
        /// <param name="value">属性值</param>
        /// 实例 XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
        /// XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "Num", "999"); 
        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (string.IsNullOrEmpty(attribute))
                    xe.InnerText = value;//原<ID>2</ID> 改变:<ID>888</ID>  XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
                else
                    xe.SetAttribute(attribute, value); //原<ID Num="3">888</ID> 改变<ID Num="999">888</ID>    XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "Num", "999"); 
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">要删除的节点</param>
        /// <param name="attribute">属性，为空则删除整个节点，不为空则删除节点中的属性</param>
        /// 实例：XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
        /// XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (string.IsNullOrEmpty(attribute))
                    xn.ParentNode.RemoveChild(xn);// <ID Num="999">888</ID>的整个节点将被移除  XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
                else
                    xe.RemoveAttribute(attribute);//<ID Num="999">888</ID> 变为<ID>888</ID> XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

    public class XmlParseException : Exception
    {
        public XmlParseException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}


/*
public static void XMLMTest()
  {
      string path = "http://www.cnblogs.com/../Person.xml";
      XMLHelper.Read(path, "PersonF/person[@Name='Person2']", "Name");
 
      XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "Num", "ID", "88");
      XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "Num", "", "88");
      XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
 
      XMLHelper.Update(path, "PersonF/person[@Name='Person3']", "Num", "888");
      XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "Num", "999");
      XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
 
      XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
 
      XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
 
  }
　　

6、XML文件

<?xml version="1.0" encoding="utf-8"?>
<PersonF xmlns="" Name="work hard work smart!">
  <person Name="Person1">
    <ID>1</ID>
    <Name>XiaoA</Name>
    <Age>59</Age>
  </person>
  <person Name="Person2" ID="88">
    <ID>2</ID>
    <Name>XiaoB</Name>
    <Age>29</Age>
    <Num ID="88" />
    <Num>88</Num>
  </person>
  <person Name="Person3">
    <ID Num="999">888</ID>
    <Name>XiaoC</Name>
    <Age>103</Age>
  </person>
  <person Name="Person4">
    <ID>4</ID>
    <Name>XiaoD</Name>
    <Age>59</Age>
  </person>
  <person Name="Person5">
    <Name>work hard work smart!</Name>
    <ID>32</ID>
  </person>
  <person Name="Person5">
    <Name>work hard work smart!</Name>
    <ID>32</ID>
  </person>
</PersonF>  */
