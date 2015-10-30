/*
 ixmlnode.cs

 leevi 2003-5-29
 
/////////////////////////////////////////////////////////////////////////////////

 copyright (c) 2004 leevi

 this library is free software; you can redistribute it and/or
 modify it under the terms of the gnu lesser general public
 license as published by the free software foundation; either
 version 2.1 of the license, or (at your option) any later version.
 
 this library is distributed in the hope that it will be useful,
 but without any warranty; without even the implied warranty of
 merchantability or fitness for a particular purpose. see the gnu
 lesser general public license for more details.
 
 you should have received a copy of the gnu lesser general public
 license along with this library; if not, write to the free software
 foundation, inc., 59 temple place, suite 330, boston, ma 02111-1307 usa
 
*/
using System;
using System.Xml;

namespace Kumaraji.Xml.Stxd
{
	public interface IXmlNode
	{
		IXmlDocument Document { get ; set ; }
		IXmlNode Parent { get ; set ; }

		void FromXmlNode(XmlNode node) ;
		XmlNode ToXmlNode(XmlDocument doc, string name);

		void ReadFrom(XmlReader reader) ;
		void WriteTo(XmlWriter writer, string name) ;
	}
}
