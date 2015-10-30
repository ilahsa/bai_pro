/*
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Imps.Services.CommonV4
{
	public class Dichotomy<K, V> where K: IComparable
	{
		private class KeyValueObject
		{
			public K beginKey = default(K);
			public K endKey = default(K);
			public V obj = default(V);

			public KeyValueObject(K begin, K end, V value)
			{
				beginKey = begin;
				endKey = end;
				obj = value;
			}
		}

		private IICReaderWriterLock _lockSegs = new IICReaderWriterLock();

		private SortedList<K, KeyValueObject> _searchEntrys
			= new SortedList<K, KeyValueObject>();

		public Dichotomy()
		{

		}

		public void Clear()
		{
			using (IICLockRegion region = _lockSegs.LockForWrite()) {
				_searchEntrys.Clear();
			}
		}

		public void Add(K beginKey, K endKey, V value)
		{
			using (IICLockRegion region = _lockSegs.LockForWrite()) {
				KeyValueObject itemKey;
				if (_searchEntrys.TryGetValue(beginKey, out itemKey)) {
					throw new ArgumentException(string.Format("key already exists! {0}", beginKey));
					//itemKey.beginKey = beginKey;
					//itemKey.endKey = endKey;
					//itemKey.obj = value;
				} else
					_searchEntrys.Add(beginKey, new KeyValueObject(beginKey, endKey, value));
			}
		}

		public V Search(K key)
		{
			int dummy;
			return Search(key, out dummy);
		}

		public V Search(K key, out int index)
		{
			if (_searchEntrys.Count == 0) {
				index = -1;
				return default(V);
			}

			using (IICLockRegion region = _lockSegs.LockForRead()) {
				int begin = 0;
				int end = _searchEntrys.Count - 1;

				KeyValueObject seg;
				while (end - begin > 1) {
					int middle = (end + begin) / 2;
					seg = _searchEntrys.Values[middle];
					int compareResult = seg.beginKey.CompareTo(key);

					if (compareResult == 0) {
						index = middle;
						return seg.obj;
					} else {
						if (key.CompareTo(seg.beginKey) > 0)
							begin = middle;
						else
							end = middle;
					}
				}

				if (end == begin) {
					index = begin;
					seg = _searchEntrys.Values[begin];
					if (key.CompareTo(seg.beginKey) >= 0 && key.CompareTo(seg.endKey) <= 0)
						return seg.obj;
				} else {
					seg = _searchEntrys.Values[begin];
					if (key.CompareTo(seg.beginKey) >= 0 && key.CompareTo(seg.endKey) <= 0) {
						index = begin;
						return seg.obj;
					}

					seg = _searchEntrys.Values[end];
					if (key.CompareTo(seg.beginKey) >= 0 && key.CompareTo(seg.endKey) <= 0) {
						index = end;
						return seg.obj;
					}
				}
			}

			index = -1;
			return default(V);
		}

		public V[] Search(K begin, K end)
		{
			int beginIndex;
			V beginSeg = Search(begin, out beginIndex);

			int endIndex;
			V endSeg = Search(end, out endIndex);

			if (beginSeg != null && endSeg != null) {
				V[] segs = new V[endIndex - beginIndex + 1];
				for (int index = 0; index < segs.Length; ++index)
					segs[index] = _searchEntrys.Values[beginIndex + index].obj;
				return segs;
			}
			return null;
		}
	}
}
