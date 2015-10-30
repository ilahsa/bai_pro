using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		ConfigSection：
	///			Local：	对应XML的配置
	///			HA：	对应HADB中的集中配置
	///			Section中会包含Item，或Field，但是Item不可以再包含Item
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class IICConfigSectionAttribute : Attribute
	{
		public IICConfigSectionAttribute(string sectionName)
		{
			SectionName = sectionName;
		}

		public string SectionName
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get { return _isRequired; }
			set { _isRequired = value; }
		}

		private bool _isRequired = true;
	}

	/// <summary>
	///		ConfigField:
	///			可以包含很多的Field，
	///				Local	XML Attribute
	///				HA:		DB中的Item
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class IICConfigFieldAttribute : Attribute
	{
		public IICConfigFieldAttribute(string fieldName)
		{
			FieldName = fieldName;
		}

		public string FieldName
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get;
			set;
		}

		public object DefaultValue
		{
			get;
			set;
		}
	}

	/// <summary>
	///		ConfigItem:
	///			可以包含很多的Field，
	///				Local	XML Attribute
	///				HA:		DB中的Item
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
	public class IICConfigItemAttribute : Attribute
	{
		public IICConfigItemAttribute(string itemName)
		{
			ItemName = itemName;
		}

		public IICConfigItemAttribute(string itemName, string keyField)
		{
			ItemName = itemName;
			KeyField = keyField;
		}

		public string ItemName
		{
			get;
			set;
		}

		public string KeyField
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get { return _isRequired; }
			set { _isRequired = value; }
		}

		private bool _isRequired = true;
	}

	/// <summary>
	///		ItemCollection
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class IICConfigItemCollectionAttribute : Attribute
	{
		public IICConfigItemCollectionAttribute(string itemName)
        {
			ItemName = itemName;
        }

		public string ItemName
		{
			get;
			set;
		}

		public string DefaultXml
		{
			get;
			set;
		}
	}
}
