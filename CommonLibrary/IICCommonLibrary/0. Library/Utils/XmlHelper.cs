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
        /// ��html��ʽ�ı�ת���ɴ��ı���ʽ
        /// </summary>
        /// <param name="htmlText">html��ʽ�ı�</param>
        /// <returns>ת����Ĵ��ı�</returns>
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
        /// ��ȡ�ڵ���ĳһ�����Ե�ֵ�����attributeΪ�գ��򷵻������ڵ��InnerText�����򷵻ؾ���attribute��ֵ
        /// </summary>
        /// <param name="path">xml�ļ�·��</param>
        /// <param name="node">�ڵ�</param>
        /// <param name="attribute">�ڵ��е�����</param>
        /// <returns>���attributeΪ�գ��򷵻������ڵ��InnerText�����򷵻ؾ���attribute��ֵ</returns>
        /// ʹ��ʵ��: XMLHelper.Read(path, "PersonF/person[@Name='Person2']", "");
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
        /// ��ڵ������ӽڵ�Ԫ�أ�����
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="node">Ҫ�����Ľڵ�</param>
        /// <param name="element">Ҫ���ӵĽڵ�Ԫ�أ��ɿտɲ��ա��ǿ�ʱ�����µ�Ԫ�أ���������Ԫ�ص�����</param>
        /// <param name="attribute">Ҫ���ӵĽڵ����ԣ��ɿտɲ��ա��ǿ�ʱ����Ԫ��ֵ���������Ԫ��ֵ</param>
        /// <param name="value">Ҫ���ӵĽڵ�ֵ</param>
        /// ʹ��ʵ����XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "", "88");
        /// XMLHelper.Insert(path, "PersonF/person[@Name='Person2']", "", "ID", "88");
        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                //���element�������Ӹ����� 
                if (string.IsNullOrEmpty(element))
                {
                    //���attribute��Ϊ�գ����Ӹ�����
                    if (!string.IsNullOrEmpty(attribute))
                    {

                        XmlElement xe = (XmlElement)xn;
                        // <person Name="Person2" ID="88"> XMLHelper.Insert(path, "PersonF/person[@Name='Person2']","Num", "ID", "88");
                        xe.SetAttribute(attribute, value);
                    }
                }
                else //���element��Ϊ�գ���preson�����ӽڵ�   
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
        /// �޸Ľڵ�ֵ
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="node">Ҫ�޸ĵĽڵ�</param>
        /// <param name="attribute">���������ǿ�ʱ�޸Ľڵ������ֵ�������޸Ľڵ�ֵ</param>
        /// <param name="value">����ֵ</param>
        /// ʵ�� XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
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
                    xe.InnerText = value;//ԭ<ID>2</ID> �ı�:<ID>888</ID>  XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "", "888");
                else
                    xe.SetAttribute(attribute, value); //ԭ<ID Num="3">888</ID> �ı�<ID Num="999">888</ID>    XMLHelper.Update(path, "PersonF/person[@Name='Person3']/ID", "Num", "999"); 
                doc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="node">Ҫɾ���Ľڵ�</param>
        /// <param name="attribute">���ԣ�Ϊ����ɾ�������ڵ㣬��Ϊ����ɾ���ڵ��е�����</param>
        /// ʵ����XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
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
                    xn.ParentNode.RemoveChild(xn);// <ID Num="999">888</ID>�������ڵ㽫���Ƴ�  XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "");
                else
                    xe.RemoveAttribute(attribute);//<ID Num="999">888</ID> ��Ϊ<ID>888</ID> XMLHelper.Delete(path, "PersonF/person[@Name='Person3']/ID", "Num");
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
����

6��XML�ļ�

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
