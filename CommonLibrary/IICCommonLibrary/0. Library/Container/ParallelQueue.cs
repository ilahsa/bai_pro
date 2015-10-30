/*
 * 处理单入多出方式的LazyQueue, 超级Super
 * 使用线程池分发
 * 
 */

using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class ParallelQueue<K, V>
	{
		#region Const
		public const int MaxQueueCount = 8192;
		#endregion

		#region Inner Queue ParallelQueueUnit
		private class ParallelQueueUnit
		{
			public Queue<V> Queue = new Queue<V>();
			public int LastTick;
		}
		#endregion

		#region private fields
		private object _syncRoot = new object();
		private Dictionary<K, ParallelQueueUnit> _queues = new Dictionary<K, ParallelQueueUnit>();

		private Thread _thread;
		private string _queueName;
		private int _maxBatchCount;
		private int _maxIdleMs;
		private Action<K, V[]> _dequeueAction;
		private int _defendWorkerThreadCounts;
		#endregion

		#region Diagnostics Fields
		private ParallelQueuePerfCounters _perfCounters;
		private ITracing _tracing;
		#endregion

		#region Constructors
		public ParallelQueue(string queueName, int maxBatchCount, int maxIdleMs, Action<K, V[]> dequeueAction)
		{
			_queueName = queueName;
			_maxBatchCount = maxBatchCount;
			_maxIdleMs = maxIdleMs;
			_dequeueAction = dequeueAction;

			_tracing = TracingManager.GetTracing("ParalelQueue." + queueName);
			_perfCounters = IICPerformanceCounterFactory.GetCounters<ParallelQueuePerfCounters>(queueName);

			_thread = new Thread(ThreadProc);
			_thread.IsBackground = true;
			_thread.Start();

			int max;
			int foo;
			ThreadPool.GetMaxThreads(out max, out foo);
			_defendWorkerThreadCounts = max / 8; 
		}
		#endregion

		#region Public Fields
		public void Enqueue(K queueKey, V value)
		{
			lock (_syncRoot) {
				ParallelQueueUnit queue;
				if (!_queues.TryGetValue(queueKey, out queue)) {
					queue = new ParallelQueueUnit();
					queue.LastTick = Environment.TickCount;
					_queues.Add(queueKey, queue);
				}
				if (queue.Queue.Count >= MaxQueueCount) {
					//
					// Discard Queue保护自己
					_perfCounters.DiscardTotal.Increment();
				}
				queue.Queue.Enqueue(value);
			}

			_perfCounters.EnqueuePerSecond.Increment();
			_perfCounters.EnqueueTotal.Increment();
			_perfCounters.QueueLength.Increment();
		}
		
		public void FlushCache()
		{
			lock (_syncRoot) {
				foreach (KeyValuePair<K, ParallelQueueUnit> p in _queues) {
					ParallelQueueUnit queue = p.Value;
					while (queue.Queue.Count > 0) {
						int queueCount = queue.Queue.Count;
						int dequeueCount = queueCount > _maxBatchCount ? _maxBatchCount : queueCount;
						Dequeue(p.Key, queue, dequeueCount);
					}
				}
			}
		}

        /// <summary>
        /// flush 特定key 的 queue
        /// </summary>
        /// <param name="key"></param>
        public void FlushCacheByKey(K key)
        {
            lock (_syncRoot)
            {
                ParallelQueueUnit queueUnit = null;
                if (_queues.TryGetValue(key, out queueUnit))
                {
                    while (queueUnit.Queue.Count > 0)
                    {
                        int queueCount = queueUnit.Queue.Count;
                        int dequeueCount = queueCount > _maxBatchCount ? _maxBatchCount : queueCount;
                        Dequeue(key, queueUnit, dequeueCount);
                    }
                }
            }
        }
		#endregion

		#region Inner Procs
		private void ThreadProc()
		{
			while (true) {
				try {
					int nowTick = Environment.TickCount;
					bool idle = true;
					int workerThreads;
					int iocpThreads;
					ThreadPool.GetAvailableThreads(out workerThreads, out iocpThreads);

					//
					// 线程池忙时会进入保护状态
					if (workerThreads < _defendWorkerThreadCounts) {
						idle = true;
					} else {
						lock (_syncRoot) {
							foreach (KeyValuePair<K, ParallelQueueUnit> p in _queues) {
								ParallelQueueUnit queue = p.Value;
								int count = GetDequeueCount(nowTick, queue);
								if (count > 0) {
									Dequeue(p.Key, queue, count);
									_perfCounters.QueueLength.IncrementBy(-count);
									idle = false;
								}
							}
						}
					}
					if (idle) {
						Thread.Sleep(1);
					}
				} catch (ThreadAbortException) {
					Thread.ResetAbort();
					FlushCache();
					return;
				} catch (Exception ex) {
					SystemLog.Error(LogEventID.ServiceFailed, ex, "Parallel<{0}> Raise Exception", _queueName);
					Thread.Sleep(1000);
				}
			}
		}

		private int GetDequeueCount(int nowTick, ParallelQueueUnit q)
		{
			int queueCount = q.Queue.Count;
			if (queueCount >= _maxBatchCount) {
				return _maxBatchCount;
			} else {
				int passedTick = nowTick - q.LastTick;
				if (((passedTick > _maxIdleMs) || (passedTick < - _maxIdleMs)) && queueCount > 0) {
					q.LastTick = nowTick;
					return queueCount;
				} else {
					return 0;
				}
			}
		}

		private void Dequeue(K key, ParallelQueueUnit q, int count)
		{
			V[] vals = new V[count];
			for (int i = 0; i < count; i++) {
				V item = q.Queue.Dequeue();
				vals[i] = item;
			}

			_perfCounters.DequeueTotal.Increment();
			_perfCounters.DequeuePerSecond.Increment();
			_perfCounters.DequeueConcurrent.Increment();

			ThreadPool.QueueUserWorkItem(new WaitCallback(
				delegate {
					try {
						_dequeueAction(key, vals);
					} catch (Exception ex) {
						_tracing.Error(ex, "ParallelQueue DequeueAction Failed");
						_perfCounters.DequeueFailed.Increment();
					} finally {
						_perfCounters.DequeueConcurrent.Decrement();
					}
				}
				)
			);
		}
		#endregion

		#region PerformanceCounters
		[IICPerformanceCounters("Imps:ParallelQueue")]
		private class ParallelQueuePerfCounters
		{
			[IICPerformanceCounter("Enqueue /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter EnqueuePerSecond = null;

			[IICPerformanceCounter("Enqueue Total.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter EnqueueTotal = null;

			[IICPerformanceCounter("Dequeue /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
			public IICPerformanceCounter DequeuePerSecond = null;

			[IICPerformanceCounter("Dequeue Total.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter DequeueTotal = null;

			[IICPerformanceCounter("Discard Total.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter DiscardTotal = null;

			[IICPerformanceCounter("Dequeue Councurrent.", PerformanceCounterType.NumberOfItems32)]
			public IICPerformanceCounter DequeueConcurrent = null;

			[IICPerformanceCounter("Dequeue Failed.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter DequeueFailed = null;

			[IICPerformanceCounter("Queue Length.", PerformanceCounterType.NumberOfItems64)]
			public IICPerformanceCounter QueueLength = null;
		}
		#endregion
	}
}
