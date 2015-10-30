using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class TableFieldAttribute: Attribute
	{
		public TableFieldAttribute(string name)
		{
			ColumnName = name;
		}

		public string ColumnName
		{
			get;
			set;
		}

		public Type FieldType
		{
			get;
			set;
		}

		internal FieldInfo Field
		{
			get;
			set;
		}

		internal DataColumn Column
		{
			get;
			set;
		}

		public object Context
		{
			get;
			set;
		}
	}
}
