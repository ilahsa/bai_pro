/*
 XmlReaderHelper.cs

 Leevi 2003-5-29
 
/////////////////////////////////////////////////////////////////////////////////

 Copyright (C) 2004 Leevi

 This library is free software; you can redistribute it and/or
 modify it under the terms of the GNU Lesser General Public
 License as published by the Free Software Foundation; either
 version 2.1 of the License, or (at your option) any later version.
 
 This library is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 Lesser General Public License for more details.
 
 You should have received a copy of the GNU Lesser General Public
 License along with this library; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 
*/
using System;
using System.Xml;
using System.Text;

namespace Kumaraji.Xml.Stxd
{
	public class XmlReaderHelper
	{
		private XmlReaderHelper(){}

		public static string ReadInnerText(XmlReader reader)
		{
			if( reader.IsEmptyElement )
				return string.Empty ;

			StringBuilder str = new StringBuilder();
			while (reader.Read()) 
			{
				switch (reader.NodeType) 
				{
					case XmlNodeType.Element:
						str.Append(ReadInnerText(reader));
						break;
					case XmlNodeType.Text:
						str.Append(reader.Value);
						break;
					case XmlNodeType.CDATA:
						str.Append(reader.Value);
						break;
					case XmlNodeType.EndElement:
						return str.ToString();
					default :
						break ;
				}
			}
			return str.ToString();
        }

        public static string ReadInnerXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return string.Empty;

            StringBuilder str = new StringBuilder();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text :
                        str.Append(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        return str.ToString();

                    default:
                        str.Append(reader.ReadOuterXml());
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                str.Append(reader.Value);
                                break;
                            }
                            else
                            {
                                str.Append(reader.ReadOuterXml());
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement)
                            return str.ToString();

                        break;
                }
            }
            return str.ToString();
        }
	}
}
