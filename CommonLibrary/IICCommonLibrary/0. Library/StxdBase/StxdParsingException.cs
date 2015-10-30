/*
 StxdParsingException.cs

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
using System.Runtime.Serialization;

namespace Kumaraji.Xml.Stxd
{
	[Serializable]
	public class StxdParsingException : ApplicationException
	{
		public StxdParsingException()
		{
		}

		public StxdParsingException(string msg)
			: base(msg)
		{
		}

		public StxdParsingException(string msg, Exception innerException)
		: base(msg, innerException)
		{
		}

		protected StxdParsingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public static StxdParsingException Create(string format, params object[] args)
		{
			if( args.Length < 1 )
				return new StxdParsingException(format);
			else
				return new StxdParsingException(string.Format(format, args));
		}		

		public static StxdParsingException Create(Exception innerException, string format, params object[] args)
		{
			if( args.Length < 1 )
				return new StxdParsingException(format, innerException);
			else
				return new StxdParsingException(string.Format(format, args), innerException);
		}

		public static StxdParsingException 
			CreateInvalidAttribute(string nodeName, string attrName, string attrValue, Exception innerException)
		{
			return Create(innerException, "Invalid Attribute Format", nodeName, attrName, attrValue);
		}

		public static StxdParsingException 
			CreateAbsenceOfAttribute(string nodeName, string attrName)
		{
			return Create("Absence of Attribute Format", nodeName, attrName);
		}
	}
}
