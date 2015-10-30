using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public interface IConfigCollection
	{
		void Add(object key, object value);
	}

	[Serializable]
	public class IICConfigItemCollection<K, V>: IConfigCollection, IEnumerable<KeyValuePair<K, V>> where V: IICConfigItem
	{
		private Dictionary<K, V> _innerDic = new Dictionary<K, V>();
		private object _syncRoot = new object();

		public void Add(object key, object value)
		{
			_innerDic.Add((K)key, (V)value);
		}

		public IEnumerable<K> Keys
		{
			get 
			{
				foreach (K k in _innerDic.Keys)
					yield return k;
			}
		}

		public IEnumerable<V> Values
		{
			get 
			{
				foreach (V v in _innerDic.Values)
					yield return v;
			}
		}

		public V this[K key]
		{
			get { return _innerDic[key]; }
		}

		public bool TryGetValue(K key, out V value)
		{
			return _innerDic.TryGetValue(key, out value);
		}

		public object this[object key]
		{
			get { return _innerDic[(K)key]; }
		}

		public int Count
		{
			get { return _innerDic.Count; }
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			foreach (KeyValuePair<K, V> pair in _innerDic) {
				yield return pair;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerDic.GetEnumerator();
		}
	}
}
