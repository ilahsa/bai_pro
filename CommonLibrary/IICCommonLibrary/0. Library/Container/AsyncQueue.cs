using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace Imps.Services.CommonV4
{
    /// <summary>
    /// 用于两个线程间通讯的异步队列
    /// 线程安全
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncQueue<T> where T : class
    {
        private Queue<T> _queue;
        private object _syncRoot;
        public AsyncQueue()
        {
            _queue = new Queue<T>(32);
            _syncRoot = new object();
        }

        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        public Queue<T> Queue
        {
            get
            {
                return _queue;
            }
        }

        public int Count
        {
            get
            {
                int count;
                lock (_syncRoot)
                {
                    count = _queue.Count;
                }
                return count;
            }
        }

        public void Enqueue(T value)
        {
            lock (_syncRoot)
            {
                _queue.Enqueue(value);
                //Pulse()被调用时将恢复等待锁的线程队列的第一个线程
                Monitor.Pulse(_syncRoot);
            }
        }

        public T DequeueUnBlock()
        {
            T value = null;
            lock (_syncRoot)
            {
                if (_queue.Count > 0)
                {
                    value = _queue.Dequeue();
                }
            }
            return value;
        }

        public T Dequeue(TimeSpan ts)
        {
            return Dequeue((int)ts.TotalMilliseconds);
        }

        public T Dequeue()
        {
            return Dequeue(-1);
        }

        public T Dequeue(int ms)
        {
            T value = null;
            lock (_syncRoot)
            {
                while (_queue.Count == 0)
                {
                    //wait 释放锁，线程休眠
                    if (!Monitor.Wait(_syncRoot, ms))
                        break;
                }
                if (_queue.Count > 0)
                {
                    value = _queue.Dequeue();
                }
            }
            return value;
        }
    }
}
