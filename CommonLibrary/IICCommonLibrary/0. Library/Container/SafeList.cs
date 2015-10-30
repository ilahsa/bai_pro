/*
 * A Thread Safety List with owner data protection.
 * 
 * Author:	Lei Gao
 * Craeted:	2008-04-04
 */ 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Imps.Services.CommonV4
{
    [Serializable]
    public sealed class SafeList<T> : IList<T>, ISerializable
	{
		#region Private Fields
		private List<T> _innerList;
		private IICReaderWriterLock _innerLock;
		private object _ownerSecret;
		#endregion

		#region Constructors
		/// <summary>
		///		Create a Thread Safety List of T
		/// </summary>
		public SafeList(): this(null)
		{
		}

		/// <summary>
		///		Create a Thread Safety List of T with a owner secret
		/// </summary>
		/// <param name="ownerSecret">only who has the ownerSecret can modify this list</param>
		public SafeList(object ownerSecret)
		{
			_ownerSecret = ownerSecret;
			_innerList = new List<T>();
			_innerLock = new IICReaderWriterLock();
		}


		/// <summary>
		///		Create a Thread Safety List of T with a owner secret and initial innet List
		/// </summary>
		/// <param name="ownerSecret">only who has the ownerSecret can modify this list</param>
		/// <param name="innerList">initial innet List</param>
		public SafeList(object ownerSecret, List<T> innerList)
		{
			_ownerSecret = ownerSecret;
			_innerList = innerList;
			_innerLock = new IICReaderWriterLock();
		}


        private SafeList(SerializationInfo info, StreamingContext context)
        {
            _innerList = (List<T>)info.GetValue("innerList", typeof(List<T>));
            _ownerSecret = info.GetValue("ownerSecret", typeof(object));
            _innerLock = new IICReaderWriterLock();
        }

		#endregion

		#region Private Methods
		private void ValidateOwnerSecret(object ownerSecret)
		{
			if (_ownerSecret == null)
				return;

			if (ownerSecret != _ownerSecret) {
				string msg = string.Format("Try modify a SafeList<{0}, {1}> without vaild ownerSecret.", typeof(T).Name);
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
			ValidateOwnerSecret(secretObject);
			return _innerLock;
		}

		public List<T> GetInnerList()
		{
			return GetInnerList(null);
		}

		public List<T> GetInnerList(object secretObject)
		{
			ValidateOwnerSecret(secretObject);
			return _innerList;
		}

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("innerList", _innerList);
            info.AddValue("ownerSecret", _ownerSecret);
        }
		#endregion

		#region IList<T> Members
		public int IndexOf(T item)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerList.IndexOf(item);
			}
		}

		public void Insert(int index, T item)
		{
			Insert(null, index, item);
		}

		public void Insert(object ownerSecret, int index, T item)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerList.Insert(index, item);
			}
		}

		public void RemoveAt(int index)
		{
			RemoveAt(null, index);
		}

		public void RemoveAt(object ownerSecret, int index)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerList.RemoveAt(index);
			}
		}

		public T this[int index]
		{
			get
			{
				using (IICLockRegion region = _innerLock.LockForRead()) {
					return _innerList[index];
				}
			}
			set
			{
				SetItem(null, index, value);
			}
		}

		public void SetItem(object ownerSecret, int index, T item)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerList[index] = item;
			}
		}
		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			Add(null, item);
		}
		
		public void Add(object ownerSecret, T item)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerList.Add(item);
			}
		}

		public void Clear()
		{
			Clear(null);
		}

		public void Clear(object ownerSecret)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_innerList.Clear();
			}
		}

		public bool Contains(T item)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerList.Contains(item);
			}			
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				_innerList.CopyTo(array, arrayIndex);
			}
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			return Remove(null, item);
		}

		public bool Remove(object ownerSecret, T item)
		{
			ValidateOwnerSecret(ownerSecret);

			using (IICLockRegion region = _innerLock.LockForWrite()) {
				return _innerList.Remove(item);
			}
		}
		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			using (IICLockRegion region = _innerLock.LockForRead()) {
				return _innerList.GetEnumerator();
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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