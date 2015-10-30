using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class LazyQueue<T>: IDisposable
	{
		#region Private Fields
		private object _syncRoot = new object();
		private Queue<T> _queue = new Queue<T>();

		private Thread _thread;
		private int _lastTick;

		private string _queueName;
		private int _batchCount;
		private int _idleMs;
		private int _capacity;
		private Action<T[]> _dequeueAction;
		private ITracing _tracing;
		private LazyQueuePerfCounters _counters = null;
		#endregion

		#region Public Properties
		public int Capacity
		{
			get { return _capacity; }
			set { _capacity = value; }
		}

		public int ItemCount
		{
			get { return _queue.Count; }
		}
		#endregion

		#region Constructor
		public LazyQueue(string queueName, int batchCount, int idleMs, Action<T[]> dequeueAction)
			: this(queueName, batchCount, idleMs, dequeueAction, true)
		{
		}

		public LazyQueue(string queueName, int maxBatchCount, int maxIdleMs, Action<T[]> dequeueAction, bool enabledCounters)
		{
			_queueName = queueName;
			_batchCount = maxBatchCount;
			_idleMs = maxIdleMs;
			_dequeueAction = dequeueAction;
			_capacity = 65536;
			_lastTick = Environment.TickCount;

			_tracing = TracingManager.GetTracing("LazyQueue." + queueName);
			if (enabledCounters)
				_counters = IICPerformanceCounterFactory.GetCounters<LazyQueuePerfCounters>(queueName);

			_thread = new Thread(new ThreadStart(ThreadProc));
			_thread.Name = string.Format("LazyQueue<{0}>:{1}", typeof(T).Name, queueName);
			_thread.IsBackground = true;
			_thread.Start();
		}
		#endregion

		#region IDisposable Partten
		~LazyQueue()
		{
			Dispose(false);
		}

		/// <summary>
		/// 提供使用者一个显式释放资源的方法。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);	// Tell the GC not to execute the deconstructor any more
		}

		/// <summary>
		/// 真正的释放资源函数。
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (disposing) {
				// Clean up all managed resources
			}

			// Clean up all native resources
			if (_thread != null) {
				_thread.Abort();
				_thread = null;
			}
		}
		#endregion

		#region Public Methods
		public void Enqueue(T item)
		{
			if (_queue.Count > _capacity) {
				if (_counters != null)
					_counters.DiscardTotal.Increment();
				return;
			}

			lock (_syncRoot) {
				_queue.Enqueue(item);
			}

			if (_counters != null) {
				_counters.EnqueuePerSecond.Increment();
				_counters.EnqueueTotal.Increment();
				_counters.QueueLength.Increment();
			}
		}

		public void FlushCache()
		{
			while (_queue.Count > 0) {
				int queueCount = _queue.Count;
				int dequeueCount = queueCount > _batchCount ? _batchCount : queueCount;
				T[] items = DequeueItems(dequeueCount);
				DequeueAction(items);
			}
		}
		#endregion

		#region Private Methods
		private void ThreadProc()
		{
			while (true) {
				try {
					int nowTick = Environment.TickCount;
					int queueCount = _queue.Count;
					int dequeueCount;

					if (queueCount >= _batchCount) {
						dequeueCount = _batchCount;
					} else {
						int passedTick = nowTick - _lastTick;

						if (((passedTick > _idleMs) || (passedTick < -_idleMs)) && queueCount > 0) {
							dequeueCount = queueCount;
							_lastTick = nowTick;
						} else {
							dequeueCount = 0;
						}
					}

					if (dequeueCount > 0) {
						T[] items = DequeueItems(dequeueCount);
						DequeueAction(items);
					} else {
						Thread.Sleep(1);
					}
				} catch (ThreadAbortException) {
					Thread.ResetAbort();
					FlushCache();
					return;
				} catch (Exception ex) {
					_tracing.Error(ex, "ThreadProc Exception");
					SystemLog.Error(LogEventID.ServiceFailed, ex, "LazyQueue<{0}> Raise Exception", _queueName);
					Thread.Sleep(1000);
				}
			}
		}

		private T[] DequeueItems(int count)
		{
			T[] ret = new T[count];
			lock (_syncRoot) {
				if (count > _queue.Count) {
					count = _queue.Count;
					ret = new T[count];
				}
				for (int i = 0; i < count; i++) {
					T item = _queue.Dequeue();
					ret[i] = item;
				}
			}
			if (_counters != null)
				_counters.QueueLength.IncrementBy(- count);

			return ret;
		}

		private void DequeueAction(T[] items)
		{
			if (items.Length == 0)
				return;

			if (_counters != null) {
				_counters.DequeuePerSecond.Increment();
				_counters.DequeueTotal.Increment();
			}
			try {
				_dequeueAction(items);
			} catch (Exception ex) {
				_tracing.Error(ex, "LazyQueue Dequeue Failed");
				SystemLog.Error(LogEventID.ServerInnerFailed, ex, "LazyQueue<{0}> DequeueAction Failed", _queueName);

				if (_counters != null) {
					_counters.DequeueFailed.Increment();
				}
			}
		}
		#endregion

		#region PerformanceCounters
		[IICPerformanceCounters("Imps:LazyQueue")]
		private class LazyQueuePerfCounters
		{
			[IICPerformanceCounter("Enqueue /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter EnqueuePerSecond = null;

			[IICPerformanceCounter("Enqueue Total.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter EnqueueTotal = null;

			[IICPerformanceCounter("Dequeue /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter DequeuePerSecond = null;

			[IICPerformanceCounter("Dequeue Total.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter DequeueTotal = null;

			[IICPerformanceCounter("Dequeue Failed.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter DequeueFailed = null;

			[IICPerformanceCounter("Discard Total.", PerformanceCounterType.NumberOfItems32)]
			public IICPerformanceCounter DiscardTotal = null;

			[IICPerformanceCounter("Queue Length.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter QueueLength = null;
		}
		#endregion
	}
}
