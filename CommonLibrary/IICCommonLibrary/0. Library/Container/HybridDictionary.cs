using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public class HybridDictionary<K, V>: IDictionary<K, V>, ICollection<KeyValuePair<K, V>>
	{
		#region Private Fields
		private HybridDictionary _innerDic;
		#endregion

		#region Cosntructor
		public HybridDictionary()
		{
			_innerDic = new HybridDictionary();
		}
		#endregion

		#region IDictionary<K,V> Members

		public void Add(K key, V value)
		{
			lock (_innerDic.SyncRoot) {
				_innerDic.Add(key, value);
			}
		}

		public bool ContainsKey(K key)
		{
			lock (_innerDic.SyncRoot) {
				return _innerDic.Contains(key);
			}
		}

		public bool Remove(K key)
		{
			lock (_innerDic.SyncRoot) {
				bool hasKey = _innerDic.Contains(key);
				_innerDic.Remove(key);
				return hasKey;
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			lock (_innerDic.SyncRoot) {
				object v = _innerDic[key];
				if (v == null) {
					value = default(V);
					return false;
				} else {
					value = (V)v;
					return true;
				}
			}
		}

		public ICollection<K> Keys
		{
			get
			{
				lock (_innerDic.SyncRoot) {
					List<K> ret = new List<K>();
					foreach (K k in _innerDic.Keys) {
						ret.Add((K)k);
					}
					return ret;
				}
			}
		}

		public ICollection<V> Values
		{
			get {
				lock (_innerDic.SyncRoot) {
					List<V> ret = new List<V>();
					foreach (V v in _innerDic.Values) {
						ret.Add(v);
					}
					return ret;
				}
			}
		}

		public V this[K key]
		{
			get
			{
				lock (_innerDic.SyncRoot) {
					return (V)_innerDic[key];
				}
			}
			set
			{
				lock (_innerDic.SyncRoot) {
					_innerDic[key] = value;
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<K,V>> Members

		public void Add(KeyValuePair<K, V> item)
		{
			lock (_innerDic.SyncRoot) {
				_innerDic.Add(item.Key, item.Value);
			}
		}

		public void Clear()
		{
			lock (_innerDic.SyncRoot) {
				_innerDic.Clear();
			}
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			lock (_innerDic.SyncRoot) {
				return _innerDic.Contains(item.Key);
			}
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			lock (_innerDic.SyncRoot) {
				foreach (DictionaryEntry e in _innerDic) {
					array[arrayIndex++] = new KeyValuePair<K, V>((K)e.Key, (V)e.Value);
				}
			}
		}

		public int Count
		{
			get { return _innerDic.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			lock (_innerDic.SyncRoot) {
				return Remove(item.Key);
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<K,V>> Members

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			lock (_innerDic.SyncRoot) {
				foreach (DictionaryEntry e in _innerDic) {
					yield return new KeyValuePair<K, V>((K)e.Key, (V)e.Value);
				}
			}
		}

		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (_innerDic.SyncRoot) {
				foreach (DictionaryEntry e in _innerDic) {
					yield return new KeyValuePair<K, V>((K)e.Key, (V)e.Value);
				}
			}
		}
		#endregion
	}
}
