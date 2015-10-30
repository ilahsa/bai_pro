using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Field)]
	public class IICCodeTableFieldAttribute: Attribute
	{
		private string _fieldName;
		private bool _isKeyField;
		private bool _isTrim;
		private bool _isRequired;

		public string FieldName
		{
			get { return _fieldName; }
		}

		public bool IsKeyField
		{
			get { return _isKeyField; }
			set { _isKeyField = true; }
		}

		public bool TrimString
		{
			get { return _isTrim; }
			set { _isTrim = value; }
		}

		public bool IsRequired
		{
			get { return _isRequired; }
			set { _isRequired = value; }
		}

		public IICCodeTableFieldAttribute(string fieldName)
            : this(fieldName, false)
		{
		}

		public IICCodeTableFieldAttribute(string fieldName, bool isKeyField)
		{
			_fieldName = fieldName;
			_isKeyField = isKeyField;
			_isTrim = true;
			_isRequired = true;
		}

		public override string ToString()
		{
			if (_isKeyField) 
				return string.Format("KeyField: {0}", _fieldName);
			else 
				return string.Format("TableField: {0}", _fieldName);
		}
	}
}
