using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		Value值为一个list的的K-V结构
	///		内部为线程安全
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	public class DictionaryList<K, V>
	{
		public enum UpdateMode
		{
			New,
			Delete,
			Update
		}

		private Object _syncRoot = new Object();
		private Dictionary<K, List<V>> _table;

		public DictionaryList()
		{
			_table = new Dictionary<K, List<V>>();
		}

		public void FillWith(List<V> list, Func<V, K> getKeyFunc)
		{
			lock (_syncRoot) {
				foreach (V v in list) {
					K key = getKeyFunc(v);
					Put(key, v);
				}
			}
		}

		public void FillWithMultiKeys(List<V> list, Func<V, IEnumerable<K>> getKeysFunc)
		{
			lock (_syncRoot) {
				foreach (V v in list) {
					foreach (var k in getKeysFunc(v)) {
						Put(k, v);
					}
				}
			}
		}

		public IEnumerable<K> Keys()
		{
			List<K> keys = new List<K>();

			lock (_syncRoot) {
				foreach (K key in _table.Keys) {
					keys.Add(key);
				}
			}
			return keys;
		}

		public List<V> Get(K key)
		{
			lock (_syncRoot) {
				List<V> ret;
				if (_table.TryGetValue(key, out ret)) {
					return ret;
				} else {
					return null;
				}
			}
		}

		public void Put(K key, V value)
		{
			lock (_syncRoot) {
				List<V> list;
				if (!_table.TryGetValue(key, out list)) {
					list = new List<V>();
					_table.Add(key, list);
				}
				list.Add(value);
			}
		}


		public void Remove(K k)
		{
			lock (_syncRoot) {
				_table.Remove(k);
			}
		}
		/// <summary>
		///		寻找table与rval中的不同，逐条用新值触发unmatchCallback
		///		New: rval中新增的值，
		///		Update: 变更的值
		///		Delete:	删除的值
		/// </summary>
		/// <param name="rval"></param>
		/// <param name="unmatchCallback"></param>
		public void CompareAll(DictionaryList<K, V> rval, Action<K, List<V>, UpdateMode> unmatchCallback)
		{
			List<ComboClass<K, List<V>, UpdateMode>> updateLists = new List<ComboClass<K,List<V>,UpdateMode>>();
			lock (_syncRoot) {
				foreach (var e in _table) {
					List<V> llist = e.Value;
					List<V> rlist;
					if (rval._table.TryGetValue(e.Key, out rlist)) {
						if (!ListEqual(llist, rlist)) {
							updateLists.Add(new ComboClass<K,List<V>,UpdateMode>(e.Key, rlist, UpdateMode.Update));
						}
					} else {
						updateLists.Add(new ComboClass<K,List<V>,UpdateMode>(e.Key, null, UpdateMode.Delete));
					}
				}

				foreach (var e in rval._table) {
					List<V> rlist = e.Value;
					List<V> llist;
					if (!_table.TryGetValue(e.Key, out llist)) {
						updateLists.Add(new ComboClass<K,List<V>,UpdateMode>(e.Key, rlist, UpdateMode.New));
					}
				}
			}

			foreach (var l in updateLists) {
				unmatchCallback(l.V1, l.V2, l.V3);
			}
		}

		/**
		 * 
		 * 这是一个o(n*n)的算法，但是输入数据量很小，所以可以接受
		 * TODO 需要详细测试论证算法正确
		 * @param llist
		 * @param rlist
		 * @return
		 */
		public static bool ListEqual(List<V> llist, List<V> rlist)
		{
			if (llist.Count != rlist.Count)
				return false;

			int matchs = 0;
			bool[] matching = new bool[llist.Count];
			for (int i = 0; i < llist.Count; i++) {
				V lval = llist[i];
				for (int j = 0; j < rlist.Count; j++) {
					V rval = rlist[j];
					if ((lval == null && rval == null) || (lval != null && lval.Equals(rval))) {
						//
						// 必须兼容在一个list中存在相同值的情况
						// 一个右值只能match一次
						if (!matching[j]) {
							matching[j] = true;
							matchs++;
							break;
						}
					}
				}
			}
			return matchs == llist.Count;
		}
	}
}