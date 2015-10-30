using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	public class IICCodeTable<K, V>: IICCodeTableBase, IEnumerable<KeyValuePair<K, V>> where V: IICCodeTableItem
	{
		public IICCodeTable(IICConfigTableBuffer buffer)
			: base(buffer.TableName, buffer.Version)
		{
			_innerTable = new Dictionary<K, V>();
			int columnCount = buffer.ColumnNames.Length;

			Type keyType = typeof(K);
			Type valueType = typeof(V);
			bool simpleKey = keyType.BaseType != typeof(IICCodeTableItemKey);

			FieldInfo[] valueFields = new FieldInfo[columnCount];
			FieldInfo[] keyFields = new FieldInfo[columnCount];
			IICCodeTableFieldAttribute[] valueAttrs = new IICCodeTableFieldAttribute[columnCount];
			IICCodeTableFieldAttribute[] keyAttrs = new IICCodeTableFieldAttribute[columnCount];

			FieldInfo[] fields = valueType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields) {
				IICCodeTableFieldAttribute attr = AttributeHelper.TryGetAttribute<IICCodeTableFieldAttribute>(field);

				if (attr != null) {
					bool find = false;
					for (int i = 0; i < columnCount; i++) {
						if (buffer.ColumnNames[i] == attr.FieldName) {
							valueFields[i] = field;
							valueAttrs[i] = attr;
							find = true;
							if (simpleKey && attr.IsKeyField) {
								keyFields[i] = field;
							}
							break;
						}
					}
					if (!find && attr.IsRequired) {
						throw new ConfigurationNotFoundException(TableName, "", attr.FieldName); 
					}
				}
			}

			if (!simpleKey) {
				fields = keyType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (FieldInfo field in fields) {
					IICCodeTableFieldAttribute attr = AttributeHelper.TryGetAttribute<IICCodeTableFieldAttribute>(field);

					if (attr != null) {
						bool find = false;
						for (int i = 0; i < columnCount; i++) {
							if (buffer.ColumnNames[i] == attr.FieldName) {
								keyFields[i] = field;
								keyAttrs[i] = attr;
								find = true;
								break;
							}
						}
						if (!find) {
							throw new ConfigurationNotFoundException(TableName, "", attr.FieldName);
						}
					}
				}
			} 

			foreach (RpcClass<string[]> row in buffer.Rows) {
				object key = null;
				object value = Activator.CreateInstance(valueType);

				if (!simpleKey)
					key = Activator.CreateInstance(keyType);

				for (int i = 0; i < columnCount; i++) {
					if (valueFields[i] == null)
						continue;

					string valStr = row.Value[i];
					if (valueAttrs[i].TrimString)
						valStr = valStr.Trim();

					object fieldValue = ObjectHelper.ConvertTo(valStr, valueFields[i].FieldType);
					valueFields[i].SetValue(value, fieldValue);
					if (keyFields[i] != null) {
						if (simpleKey) {
							key = fieldValue;
						} else {
							keyFields[i].SetValue(key, fieldValue);
						}
					}
				}

				K k1 = (K)key;
				V v1 = (V)value;

                if (!_innerTable.ContainsKey(k1))
                    _innerTable.Add(k1, v1);
			}
		}

		public V this[K key]
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					V val;
					if (_innerTable.TryGetValue(key, out val)) {
						return val;
					} else {
						throw new ConfigurationNotFoundException(TableName, key.ToString(), string.Empty);
					}
				}
			}
		}

		public bool TryGetItem(K key, out V item)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerTable.TryGetValue(key, out item);
			}
		}

		public IEnumerable<K> Keys
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					foreach (K key in _innerTable.Keys)
						yield return key;
				}
			}
		}

		public IEnumerable<V> Items
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					foreach (V item in _innerTable.Values)
						yield return item;
				}
			}
		}

		public int Count
		{
			get { return _innerTable.Count; }
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				foreach (KeyValuePair<K, V> item in _innerTable) {
					yield return item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				foreach (KeyValuePair<K, V> item in _innerTable) {
					yield return new DictionaryEntry(item.Key, item.Value);
				}
			}
		}

		public void RunAfterLoad()
		{
			lock (_innerLock) {
				foreach (var k in _innerTable)
					k.Value.AfterLoad();
			}
		}
		
		private Dictionary<K, V> _innerTable;
		private IICReaderWriterLock _innerLock = new IICReaderWriterLock();
	}
}
