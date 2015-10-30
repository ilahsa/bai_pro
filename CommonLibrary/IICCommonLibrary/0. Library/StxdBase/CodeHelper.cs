/*
 CodeHelper.cs

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
using System.Text ;
using System.Collections;

namespace Kumaraji.Xml.Stxd
{
	public class CodeHelper
	{
		private CodeHelper(){}

        public const string NullValue = "" ;
        public const string NullString = "null" ;

		private static Hashtable _keyWords ;


		static CodeHelper()
		{
			_keyWords = new Hashtable() ;

			_keyWords.Add("abstract", null) ;
			_keyWords.Add("as", null) ;
			_keyWords.Add("base", null) ;
			_keyWords.Add("bool", null) ;
			_keyWords.Add("break", null) ;
			_keyWords.Add("byte", null) ;
			_keyWords.Add("case", null) ;
			_keyWords.Add("catch", null) ;
			_keyWords.Add("char", null) ;
			_keyWords.Add("checked", null) ;
			_keyWords.Add("class", null) ;
			_keyWords.Add("const", null) ;
			_keyWords.Add("continue", null) ;
			_keyWords.Add("decimal", null) ;
			_keyWords.Add("default", null) ;
			_keyWords.Add("delegate", null) ;
			_keyWords.Add("do", null) ;
			_keyWords.Add("double", null) ;
			_keyWords.Add("else", null) ;
			_keyWords.Add("enum", null) ;
			_keyWords.Add("event", null) ;
			_keyWords.Add("explicit", null) ;
			_keyWords.Add("extern", null) ;
			_keyWords.Add("false", null) ;
			_keyWords.Add("finally", null) ;
			_keyWords.Add("fixed", null) ;
			_keyWords.Add("float", null) ;
			_keyWords.Add("for", null) ;
			_keyWords.Add("foreach", null) ;
			_keyWords.Add("goto", null) ;
			_keyWords.Add("if", null) ;
			_keyWords.Add("implicit", null) ;
			_keyWords.Add("in", null) ;
			_keyWords.Add("int", null) ;
			_keyWords.Add("interface", null) ;
			_keyWords.Add("internal", null) ;
			_keyWords.Add("is", null) ;
			_keyWords.Add("lock", null) ;
			_keyWords.Add("long", null) ;
			_keyWords.Add("namespace", null) ;
			_keyWords.Add("new", null) ;
			_keyWords.Add("null", null) ;
			_keyWords.Add("object", null) ;
			_keyWords.Add("operator", null) ;
			_keyWords.Add("out", null) ;
			_keyWords.Add("override", null) ;
			_keyWords.Add("params", null) ;
			_keyWords.Add("private", null) ;
			_keyWords.Add("protected", null) ;
			_keyWords.Add("public", null) ;
			_keyWords.Add("readonly", null) ;
			_keyWords.Add("ref", null) ;
			_keyWords.Add("return", null) ;
			_keyWords.Add("sbyte", null) ;
			_keyWords.Add("sealed", null) ;
			_keyWords.Add("short", null) ;
			_keyWords.Add("sizeof", null) ;
			_keyWords.Add("stackalloc", null) ;
			_keyWords.Add("static", null) ;
			_keyWords.Add("string", null) ;
			_keyWords.Add("struct", null) ;
			_keyWords.Add("switch", null) ;
			_keyWords.Add("this", null) ;
			_keyWords.Add("throw", null) ;
			_keyWords.Add("true", null) ;
			_keyWords.Add("try", null) ;
			_keyWords.Add("typeof", null) ;
			_keyWords.Add("uint", null) ;
			_keyWords.Add("ulong", null) ;
			_keyWords.Add("unchecked", null) ;
			_keyWords.Add("unsafe", null) ;
			_keyWords.Add("ushort", null) ;
			_keyWords.Add("using", null) ;
			_keyWords.Add("virtual", null) ;
			_keyWords.Add("void", null) ;
			_keyWords.Add("volatile", null) ;
			_keyWords.Add("while", null) ;
		}


		public static string TypeToCode(string type)
		{
			switch(type)
			{
                case "bool" :
				case "Boolean" :
				case "System.Boolean" :
                    return "bool";
                case "bool?":
                case "Boolean?":
                case "System.Boolean?":
                    return "bool?";

                case "char" :
				case "Char" :
				case "System.Char" :
					return "char" ;
                case "char?":
                case "Char?":
                case "System.Char?":
                    return "char?";

                case "sbyte" :
				case "SByte" :
				case "System.SByte" :
                    return "sbyte";
                case "sbyte?":
                case "SByte?":
                case "System.SByte?":
                    return "sbyte?";

                case "byte" :
				case "Byte" :
				case "System.Byte" :
                    return "byte";
                case "byte?":
                case "Byte?":
                case "System.Byte?":
                    return "byte?";

                case "short" :
				case "Int16" :
				case "System.Int16" :
                    return "short";
                case "short?":
                case "Int16?":
                case "System.Int16?":
                    return "short?";

                case "ushort" :
				case "UInt16" :
				case "System.UInt16" :
					return "ushort" ;
                case "ushort?":
                case "UInt16?":
                case "System.UInt16?":
                    return "ushort?";

                case "int" :
				case "Int32" :
				case "System.Int32" :
					return "int" ;
                case "int?":
                case "Int32?":
                case "System.Int32?":
                    return "int?";

                case "uint" :
				case "UInt32" :
				case "System.UInt32" :
					return "uint" ;
                case "uint?":
                case "UInt32?":
                case "System.UInt32?":
                    return "uint?";

                case "long" :
				case "Int64" :
				case "System.Int64" :
                    return "long";
                case "long?":
                case "Int64?":
                case "System.Int64?":
                    return "long?";

                case "ulong" :
				case "UInt64" :
				case "System.UInt64" :
                    return "ulong";
                case "ulong?":
                case "UInt64?":
                case "System.UInt64?":
                    return "ulong?";

                case "float" :
				case "Single" :
				case "System.Single" :
                    return "float";
                case "float?":
                case "Single?":
                case "System.Single?":
                    return "float?";

                case "double" :
				case "Double" :
				case "System.Double" :
                    return "double";
                case "double?":
                case "Double?":
                case "System.Double?":
                    return "double?";

                case "decimal" :
				case "Decimal" :
				case "System.Decimal" :
                    return "decimal";
                case "decimal?":
                case "Decimal?":
                case "System.Decimal?":
                    return "decimal?";

                case "DateTime":
                case "System.DateTime":
                    return "DateTime";
                case "DateTime?":
                case "System.DateTime?":
                    return "DateTime?";

                case "TimeSpan":
                case "System.TimeSpan":
                    return "TimeSpan";
                case "TimeSpan?":
                case "System.TimeSpan?":
                    return "TimeSpan?";

                case "Guid":
                case "System.Guid":
                    return "Guid";
                case "Guid?":
                case "System.Guid?":
                    return "Guid?";


				case "String" :
				case "System.String" :
					return "string" ;

				default :
					return type ;
			}
		}

		public static string ValueToCode(string value, string type)
		{
            if ( value == null || value == NullValue )
                return "null";

			switch(type)
			{
				case "bool" :
				case "Boolean" :
				case "System.Boolean" :
                case "bool?":
                case "Boolean?":
                case "System.Boolean?":
                    if (bool.Parse(value))
						return "true" ;
					else
						return "false" ;

                case "char":
                case "Char":
                case "System.Char":
				case "char?" :
				case "Char?" :
				case "System.Char?" :
					return string.Format("'{0}'", value[0]) ;

				case "sbyte" :
				case "SByte" :
				case "System.SByte" :
                case "sbyte?":
                case "SByte?":
                case "System.SByte?":
                    return sbyte.Parse(value).ToString();

				case "byte" :
				case "Byte" :
				case "System.Byte" :
                case "byte?":
                case "Byte?":
                case "System.Byte?":
                    return byte.Parse(value).ToString();

				case "short" :
				case "Int16" :
				case "System.Int16" :
                case "short?":
                case "Int16?":
                case "System.Int16?":
                    return short.Parse(value).ToString();

				case "ushort" :
				case "UInt16" :
				case "System.UInt16" :
                case "ushort?":
                case "UInt16?":
                case "System.UInt16?":
                    return ushort.Parse(value).ToString();

				case "int" :
				case "Int32" :
				case "System.Int32" :
                case "int?":
                case "Int32?":
                case "System.Int32?":
                    return int.Parse(value).ToString();

				case "uint" :
				case "UInt32" :
				case "System.UInt32" :
                case "uint?":
                case "UInt32?":
                case "System.UInt32?":
                    return string.Format("{0}U", uint.Parse(value));

				case "long" :
				case "Int64" :
				case "System.Int64" :
                case "long?":
                case "Int64?":
                case "System.Int64?":
                    return string.Format("{0}L", long.Parse(value));

				case "ulong" :
				case "UInt64" :
				case "System.UInt64" :
                case "ulong?":
                case "UInt64?":
                case "System.UInt64?":
                    return string.Format("{0}UL", ulong.Parse(value));

				case "single" :
				case "Single" :
				case "System.Single" :
                case "single?":
                case "Single?":
                case "System.Single?":
                    return string.Format("{0}F", float.Parse(value));

				case "double" :
				case "Double" :
				case "System.Double" :
                case "double?":
                case "Double?":
                case "System.Double?":
                    return string.Format("{0}D", double.Parse(value));

				case "decimal" :
				case "Decimal" :
				case "System.Decimal" :
                case "decimal?":
                case "Decimal?":
                case "System.Decimal?":
                    return string.Format("{0}D", double.Parse(value));

				case "string" :
				case "String" :
				case "System.String" :
					if( value.Length < 1 )
						return "string.Empty" ;
					return string.Format("\"{0}\"", value.Replace("\"", "\\\""));

				case "DateTime" :
				case "System.DateTime" :
                case "DateTime?":
                case "System.DateTime?":
					return string.Format("DateTime.Parse(\"{0}\")", TypeConvert.ToString(DateTime.Parse(value)));

				case "TimeSpan" :
				case "System.TimeSpan" :
                case "TimeSpan?":
                case "System.TimeSpan?":
					return string.Format("TimeSpan.Parse(\"{0}\")", TimeSpan.Parse(value));

				case "Guid" :
				case "System.Guid" :
                case "Guid?":
                case "System.Guid?":
					return string.Format("new Guid(\"{0}\")", new Guid(value));

				default :
					return value ;
			}
		}

		public static string ValueToString(string value, string type)
		{
            if( value == null || value == NullValue )
                return NullString ;

			switch(type)
			{
				case "bool" :
				case "Boolean" :
                case "System.Boolean":
                case "bool?":
                case "Boolean?":
                case "System.Boolean?":
					if( bool.Parse(value))
						return "\"True\"" ;
					else
						return "\"False\"" ;

				case "char" :
				case "Char" :
                case "System.Char":
                case "char?":
                case "Char?":
                case "System.Char?":
					return string.Format("\"{0}\"", value[0]) ;

				case "sbyte" :
				case "SByte" :
                case "System.SByte":
                case "sbyte?":
                case "SByte?":
                case "System.SByte?":
					return string.Format("\"{0}\"", sbyte.Parse(value));

				case "byte" :
				case "Byte" :
                case "System.Byte":
                case "byte?":
                case "Byte?":
                case "System.Byte?":
					return string.Format("\"{0}\"", byte.Parse(value));

				case "short" :
				case "Int16" :
                case "System.Int16":
                case "short?":
                case "Int16?":
                case "System.Int16?":
					return string.Format("\"{0}\"", short.Parse(value));

				case "ushort" :
				case "UInt16" :
                case "System.UInt16":
                case "ushort?":
                case "UInt16?":
                case "System.UInt16?":
					return string.Format("\"{0}\"", ushort.Parse(value));

				case "int" :
				case "Int32" :
                case "System.Int32":
                case "int?":
                case "Int32?":
                case "System.Int32?":
					return string.Format("\"{0}\"", int.Parse(value));

				case "uint" :
				case "UInt32" :
                case "System.UInt32":
                case "uint?":
                case "UInt32?":
                case "System.UInt32?":
					return string.Format("\"{0}\"", uint.Parse(value));

				case "long" :
				case "Int64" :
                case "System.Int64":
                case "long?":
                case "Int64?":
                case "System.Int64?":
					return string.Format("\"{0}\"", long.Parse(value));

				case "ulong" :
				case "UInt64" :
                case "System.UInt64":
                case "ulong?":
                case "UInt64?":
                case "System.UInt64?":
					return string.Format("\"{0}\"", ulong.Parse(value));

				case "single" :
				case "Single" :
                case "System.Single":
                case "single?":
                case "Single?":
                case "System.Single?":
					return string.Format("\"{0}\"", float.Parse(value));

				case "double" :
				case "Double" :
                case "System.Double":
                case "double?":
                case "Double?":
                case "System.Double?":
					return string.Format("\"{0}\"", double.Parse(value));

				case "decimal" :
				case "Decimal" :
                case "System.Decimal":
                case "decimal?":
                case "Decimal?":
                case "System.Decimal?":
					return string.Format("\"{0}\"", double.Parse(value));

				case "string" :
				case "String" :
				case "System.String" :
					if( value.Length < 1 )
						return "string.Empty" ;
					return string.Format("\"{0}\"", value.Replace("\\", "\\\\").Replace("\"", "\\\""));

				case "DateTime" :
                case "System.DateTime":
                case "DateTime?":
                case "System.DateTime?":
					return string.Format("\"{0}\"", TypeConvert.ToString(DateTime.Parse(value)));

				case "TimeSpan" :
                case "System.TimeSpan":
                case "TimeSpan?":
                case "System.TimeSpan?":
					return string.Format("\"{0}\"", TimeSpan.Parse(value));

				case "Guid" :
                case "System.Guid":
                case "Guid?":
                case "System.Guid?":
					return string.Format("\"{0}\"", new Guid(value));

				default :
					return value ;
			}
		}

		public static string CreatePublicName(string name)
		{
			string[] names = SplitName(name);

			StringBuilder str = new StringBuilder();

			foreach(string s in names)
			{
				if( s == null || s.Length < 1 )
					continue ;

				str.Append(char.ToUpper(s[0]));
				if( s.Length > 1 )
					str.Append(s.Substring(1));
			}

			return str.ToString() ;
		}

		public static string CreateVariableName(string name)
		{
			name = CreatePublicName(name) ;
			name = char.ToLower(name[0]).ToString() + name.Substring(1) ;

			if( _keyWords.Contains(name) )
				return string.Format("@{0}", name);

			return name ;
		}

		public static string CreateFieldName(string name)
		{
			name = CreatePublicName(name) ;
			return string.Format("_{0}{1}", char.ToLower(name[0]), name.Substring(1)) ;
		}

		public static string[] SplitName(string name)
		{
			return name.Split('-', ':', '#', '.');
		}
	}
}
