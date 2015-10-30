using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class LRUCacheManager<K, V>: IEnumerable<KeyValuePair<K, V>>
	{
		private class LRUCacheNode
		{
			public long TimeKey;

			// private byte[] _buffer;
			
			public V Value
			{
				get;
				set;
				// get { return ProtoBufSerializer.FromByteArray<V>(_buffer); }
				// set { _buffer = ProtoBufSerializer.ToByteArray<V>(value); }
			}
		}

		private int _cacheCount;
		private string _cacheName;
		private object _syncRoot = new object();
		private Dictionary<K, LRUCacheNode> _data = new Dictionary<K, LRUCacheNode>();
		private SortedDictionary<long, K> _order = new SortedDictionary<long, K>();

		private PerfCounters _counters;
		private ITracing _tracing;

		public event Action<List<KeyValuePair<K, V>>> CacheEliminated = null;

		public LRUCacheManager(string name, int cacheCount)
		{
			if (!Stopwatch.IsHighResolution)
				throw new NotSupportedException();

			_cacheName = name;
			_cacheCount = cacheCount;
			_counters = IICPerformanceCounterFactory.GetCounters<PerfCounters>(_cacheName);
			_tracing = TracingManager.GetTracing("LRUCacheManager." + _cacheName);
		}

		public bool TryGetValue(K key, out V val)
		{
			_counters.CacheReadPerSec.Increment();
			LRUCacheNode node = null;
			lock (_syncRoot) {
				if (_data.TryGetValue(key, out node)) {
					_order.Remove(node.TimeKey);
					node.TimeKey = Stopwatch.GetTimestamp();
					AddOrder(node.TimeKey, key);
				}
			}
			if (node == null) {
				_counters.CacheHitRatio.IncreaseFraction(false);
				val = default(V);
				return false;
			} else {
				_counters.CacheHitRatio.IncreaseFraction(true);
				val = node.Value;
				return true;
			}
		}

		public void Put(K key, V val)
		{
			_counters.CacheWritePerSec.Increment();

			if (_data.Count >= _cacheCount) {
				lock (_syncRoot) {
					if (_data.Count >= _cacheCount) { // double check
						EliminateCache();
					}
				}
			}
			bool add = false;

			LRUCacheNode node = new LRUCacheManager<K, V>.LRUCacheNode();
			node.Value = val;
			node.TimeKey = Stopwatch.GetTimestamp();

			lock (_syncRoot) {
				add = !_data.ContainsKey(key);
				if (add) {
					_data.Add(key, node);
				} else {
					_data[key] = node;
				}
				AddOrder(node.TimeKey, key);
			}

			if (add) {
				_counters.CacheCount.Increment();
			}
		}

		public void Remove(K key)
		{
			lock (_syncRoot) {
				LRUCacheNode node;
				if (_data.TryGetValue(key, out node)) {
					_order.Remove(node.TimeKey);
					_data.Remove(key);
					_counters.CacheCount.Decrement();
				}
			}
		}

		public void Clear()
		{
			lock (_syncRoot) {
				_data.Clear();
				_order.Clear();
				_counters.CacheCount.SetRawValue(0);
			}
		}

		private void AddOrder(long tick, K key)
		{
			while (_order.ContainsKey(tick)) {
				tick++;
			}
			_order.Add(tick, key);
		}

		private void EliminateCache()
		{
			long stamp = Stopwatch.GetTimestamp();
			int n = (_cacheCount / 100) + 1;

			if (CacheEliminated != null) {
				List<KeyValuePair<K, V>> elimateDatas = new List<KeyValuePair<K, V>>();
				for (int i = 0; i < n; i++) {
					KeyValuePair<long, K> first = _order.First();
					LRUCacheNode node;
					_data.TryGetValue(first.Value, out node);

					_order.Remove(first.Key);
					_data.Remove(first.Value);
					elimateDatas.Add(new KeyValuePair<K,V>(first.Value, node.Value));

					_counters.CacheAvgLifeTimeMs.IncrementBy(stamp - first.Key);
				}

				CacheEliminated.BeginInvoke(
					elimateDatas,
					delegate(IAsyncResult ar) {
						try {
							CacheEliminated.EndInvoke(ar);
						} catch (Exception ex) {
							_tracing.Error(ex, "LRUCacheManager.CacheEliminated Failed");
						}
					},
					null
				);
			} else {
				for (int i = 0; i < n; i++) {
					KeyValuePair<long, K> first = _order.First();
					_order.Remove(first.Key);
					_data.Remove(first.Value);
					unchecked {
						long elapsed = (stamp - first.Key) * 1000 / Stopwatch.Frequency;
						_counters.CacheAvgLifeTimeMs.IncrementBy(elapsed);
					}
				}
			}

			_counters.CacheCount.SetRawValue(_data.Count);
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			lock (_syncRoot) {
				foreach (var k in _data)
					yield return new KeyValuePair<K, V>(k.Key, k.Value.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (_syncRoot) {
				foreach (var k in _data)
					yield return new KeyValuePair<K, V>(k.Key, k.Value.Value);
			}
		}

		[IICPerformanceCounters("Imps:LRUCacheManager")]
		private class PerfCounters
		{
			[IICPerformanceCounter("Read /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter CacheReadPerSec = null;

			[IICPerformanceCounter("Write /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter CacheWritePerSec = null;

			[IICPerformanceCounter("Cache Data Count.", PerformanceCounterType.NumberOfItems32)]
			public IICPerformanceCounter CacheCount = null;

			[IICPerformanceCounter("Cache Hit Ratio.", PerformanceCounterType.RawFraction)]
			public IICPerformanceCounter CacheHitRatio = null;

			[IICPerformanceCounter("Cache Life Time Avg Ms.", PerformanceCounterType.AverageCount64)]
			public IICPerformanceCounter CacheAvgLifeTimeMs = null;
		}
	}
}
