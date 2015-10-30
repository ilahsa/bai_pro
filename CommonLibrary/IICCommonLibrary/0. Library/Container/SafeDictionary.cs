/*
 * A Thread Safety Dictionary with owner protected
 * 
 * Author:	Lei Gao
 * Created:	2008-04-04
 */ 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class SafeDictionary<K, V> : IDictionary<K, V>
	{
		#region Private Fields
		private Dictionary<K, V> _innerDic;
		private IICReaderWriterLock _innerLock;
		private object _ownerSecret;
		private volatile bool _readonly;
		#endregion

		#region Constructors
		/// <summary>
		///		Create a theard safety Dictionary
		/// </summary>
		public SafeDictionary()
			: this(null)
		{
		}

		public bool ReadOnly
		{
			get { return _readonly; }
			set 
			{ 
				using (IICLockRegion region = _innerLock.LockForWrite()) {
					_readonly = value; 
				}
			}
		}


		/// <summary>
		///		Create a thread safety Dictionary with a owner secret.
		/// </summary>
		/// <param name="ownerSecret">only who has the ownerSecret can modify this dictionary</param>
		public SafeDictionary(object ownerSecret)
		{
			_readonly = false;
			_ownerSecret = ownerSecret;
			_innerDic = new Dictionary<K, V>();
			_innerLock = new IICReaderWriterLock();
		}

		/// <summary>
		///		Create a thread safety Dictionary with a owner secret and a initial Inner Dictionary;
		/// </summary>
		/// <param name="ownerSecret">only who has the ownerSecret can modify this dictionary</param>
		/// <param name="innerDic">initial Inner Dictionary</param>
		public SafeDictionary(object ownerSecret, Dictionary<K, V> innerDic)
		{
			_readonly = false;
			_ownerSecret = ownerSecret;
			_innerDic = innerDic;
			_innerLock = new IICReaderWriterLock();
		}
		#endregion

		#region Private methods
		private void Validate(object ownerSecret)
		{
			if (_ownerSecret == null)
				return;

			if (ownerSecret != _ownerSecret) {
				string msg = string.Format("Try modify a SafeDictionary<{0}, {1}> without vaild ownerSecret", typeof(K).Name, typeof(V).Name);
				throw new NotSupportedException(msg);
			}
		}
		#endregion

		#region Public Methods
		public IICReaderWriterLock GetInnerLock()
		{
			return GetInnerLock(null);
		}

		public IICReaderWriterLock GetInnerLock(object secretObject)
		{
			Validate(secretObject);
			return _innerLock;
		}

		public Dictionary<K, V> GetInnerDictionary()
		{
			return GetInnerDictionary(null);
		}

		public Dictionary<K, V> GetInnerDictionary(object secretObject)
		{
			Validate(secretObject);
			return _innerDic;
		}
		#endregion

		#region IDictionary<K, V> Members

		public void Add(K key, V value)
		{
			Add(null, key, value);
		}

		public void Add(object ownerSecret, K key, V value)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerDic.Add(key, value);
			}
		}

		public bool ContainsKey(K key)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerDic.ContainsKey(key);
			}
		}

		public ICollection<K> Keys
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					return new List<K>(_innerDic.Keys);
				}
			}
		}

		public bool Remove(K key)
		{
			return Remove(null,key);
		}

		public bool Remove(object ownerSecret, K key)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				return _innerDic.Remove(key);
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerDic.TryGetValue(key, out value);
			}
		}

		public ICollection<V> Values
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					return new List<V>(_innerDic.Values);
				}
			}
		}

		public V this[K key]
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					return _innerDic[key];
				}
			}
			set
			{
				SetValue(null, key, value);
			}
		}

		public void SetValue(object ownerSecret, K key, V value)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerDic[key] = value;
			}
		}
		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<K, V> item)
		{
			Add(null, item);
		}

		public void Add(object ownerSecret, KeyValuePair<K, V> item)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerDic.Add(item.Key, item.Value);
			}
		}

		public void Clear()
		{
			Clear(null);
		}

		public void Clear(object ownerSecret)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerDic.Clear();
			}
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				ICollection<KeyValuePair<K,V>> itf = _innerDic;
				return itf.Contains(item);
			}
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				ICollection<KeyValuePair<K,V>> itf = _innerDic;
				itf.CopyTo(array, arrayIndex);
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
			return Remove(null, item);
		}

		public bool Remove(object ownerSecret, KeyValuePair<K, V> item)
		{
			Validate(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				return _innerDic.Remove(item.Key);
			}
		}
		#endregion

		#region IEnumerable<KeyValuePair<K, V>> Members

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
            using (IICLockRegion region = _innerLock.LockForRead()) {
				foreach (KeyValuePair<K, V> k in _innerDic) {
					yield return k;
				}
            }
		}

		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				foreach (KeyValuePair<K, V> k in _innerDic) {
					yield return k;
				}
			}
		}
		#endregion
	}
}





/*
'JAPH V1.0';$/=0xE0;$^F=1<<5;$;=$^F>>3;$,=
'Nywx$           /\/\       x7Fy~'.
'Ersxl      ,"~~~    \      kde~b'.
'iv$Ti     /        @ \_    ox*Zo'.
'vp$Le  ~~|           __0   f*Bki'.
'goiv2     \||||--||-|/     ax0ox';
{$_=chr(ord(substr($,,$"++,1))-$;);print;
$"=($"&$/)+$^Fif$"%$^F>$;;redo if$"<=$^F*$;+$;;}
*/