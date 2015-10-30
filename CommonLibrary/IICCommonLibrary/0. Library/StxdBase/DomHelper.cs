/*
 DomHelper.cs

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

namespace Kumaraji.Xml.Stxd
{
	public class DomHelper
	{
		private DomHelper(){}


		public static void AddAttribute(XmlNode node, string name, string value)
		{
			XmlAttribute attr = node.OwnerDocument.CreateAttribute(name);
			attr.Value = value == null ? string.Empty : value ;
			node.Attributes.Append(attr);
		}

		public static void AddAttribute(XmlNode node, string name, string value, string defaultValue)
		{
            if (value == null)
                return;

			if( value == defaultValue )
				return ;

			AddAttribute(node, name, value);
		}
	}
}
