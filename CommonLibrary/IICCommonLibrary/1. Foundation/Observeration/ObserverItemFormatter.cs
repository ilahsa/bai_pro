using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Observation
{
	public class ObserverItemFormatter
	{
		#region static fields
		private static object _syncRoot = new object();
		private static Dictionary<Type, ObserverItemFormatter> _formatters = new Dictionary<Type, ObserverItemFormatter>();

		public static ObserverItemFormatter GetFormatter(Type type)
		{
			lock (_syncRoot) {
				ObserverItemFormatter formatter; 
				if (!_formatters.TryGetValue(type, out formatter)) {
					formatter = new ObserverItemFormatter(type);
					_formatters.Add(type, formatter);
				}
				return formatter;
			}
		}
		#endregion

		private string[] _columns = null;
		private ObserverColumnType[] _columnTypes = null;
		private ObserverFieldAttribute[] _attrs = null;
		
		public ObserverItemFormatter(Type type)
		{
			List<ObserverFieldAttribute> attrs = new List<ObserverFieldAttribute>();

			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			foreach (FieldInfo field in type.GetFields(flags)) {
				var attr = AttributeHelper.TryGetAttribute<ObserverFieldAttribute>(field);

				if (attr == null)
					continue;

				if (string.IsNullOrEmpty(attr.FieldName)) {
					attr.FieldName = field.Name;
				}
				attr.FieldInfo = field;
				attrs.Add(attr);
			}

			_attrs = attrs.ToArray();

			_columns = new string[_attrs.Length];
			_columnTypes = new ObserverColumnType[_attrs.Length];

			for (int i = 0; i < _attrs.Length; i++) {
				_columns[i] = _attrs[i].FieldName;
				_columnTypes[i] = _attrs[i].FieldType;
			}
		}


		public string[] Columns
		{
			get { return _columns; }
		}

		public ObserverColumnType[] ColumnTypes
		{
			get { return _columnTypes; }
		}

		public string[] GetRow(ObserverItem item)
		{
			string[] row = new string[_attrs.Length];
			for (int i = 0; i < _attrs.Length; i++) {
				FieldInfo field = _attrs[i].FieldInfo;
				object val = field.GetValue(item);
				if (val != null)
					row[i] = val.ToString();
				else
					row[i] = string.Empty;
			}

			return row;
		}
	}
}
