using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Observation
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ObserverFieldAttribute: Attribute
	{
		private string _fieldName = string.Empty;
		private FieldInfo _fieldInfo = null;
		private ObserverColumnType _fieldType; 

		public string FieldName
		{
			get { return _fieldName; }
			set { _fieldName = value; }
		}

		public ObserverColumnType FieldType
		{
			get { return _fieldType; }
			set { _fieldType = value; }
		}

		public FieldInfo FieldInfo
		{
			get { return _fieldInfo; }
			set { _fieldInfo = value; }
		}

		public ObserverFieldAttribute()
			: this(ObserverColumnType.String)
		{
		}

		public ObserverFieldAttribute(ObserverColumnType fieldType)
		{
			_fieldType = fieldType;
		}
	}
}

/*
 * Database
 * IICGDB
 * IICUPDB.1
 *		StoredProc.1	Call Count, AvgElapseMs, ErrorCount
 * IICUPDB.2
 *		
 * IICUPDB.3
 * ...
 * 
 * Value	连接串
 * Version	N/A
 */

/*
 * Config	Config.Value, Config.Section, Config.Table
 * ConfigPath
 * ConfigKey
 * Version: ConfigVersion
 */

/*
 * RouteTable
 * ConfigPath
 * ConfigKey
 * Version 
 */

/*
 * HashDirector
 * HashName
 */

/*
 * PerformanceCounter
 * Key1,
 * Key2,
 * Key3
 * 
 */
