/*
 * 一个高效率的对象缓冲池，用于缓冲快速产生，快速释放的对象，
 * 相对于Dictionary来说，有无锁和高效的有点
 * 
 * 在乐观情况下， 添加和删除的算法优先级均是 O(C)，
 * 悲观情况下，存在大量连续的长生存期对象，添加的算法优先级是O(N)
 * 
 * Create by 高磊 2010-06-22, 
 *		感谢陈鹏的修改建议
 */ 
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class SessionPool<T>
	{
		private int _head;
		private int _capacity;
		private int _capacityMask;
		private int _count;

		private object[] _pools;

		public SessionPool(int capacity)
		{
			//
			// 为了保证效率，不将_pools分配满，仅保留少于一半的对象作为空槽使用
			IICAssert.IsTrue(capacity < 1024 * 1024);
			_capacity = capacity;
			_capacityMask = NumberUtils.NextPower2(capacity) * 2;
			
			_pools = new object[_capacityMask];
			_capacityMask = _capacityMask - 1;
		}

		public int Add(object value)
		{
			//
			// 先增加对象计数，避免容器满
			IICAssert.IsNotNull(value);

			int count = Interlocked.Increment(ref _count);
			if (count >= _capacity) {
				Interlocked.Decrement(ref _count);
				return -1;
			}

			//
			// 在head后寻找第一个空槽，然后插入对象
			int index = -1;
			do {
				index = Interlocked.Increment(ref _head) & _capacityMask;
			} while (Interlocked.CompareExchange(ref _pools[index], value, null) != null);
			return index;
		}
		
		public int Count
		{
			get { return _count; }
		}

		public int Capacity
		{
			get { return _capacity; }
		}

		public T this[int index]
		{
			get 
			{
				return (T)_pools[index];
			}
		}

		public void Remove(int index)
		{
			object obj = _pools[index];

			if (obj == null)
				return;

			//
			// 实际Remove掉了，就变更容器数量
			if (Interlocked.Exchange(ref _pools[index], null) != null) {
				Interlocked.Decrement(ref _count);
			}
		}

		public IEnumerable<KeyValuePair<int, T>> Items
		{
			get
            { /*原先的_capacity 循环明显是bug*/
                for (int i = 0; i < _capacityMask; i++)
                {
					var a = _pools[i];
					if (a != null)
						yield return new KeyValuePair<int, T>(i, (T)a);
				}
			}
		}

		// 
		// Clear很难做到线程安全，有需求的话，还是New个新的吧
		//public void Clear()
		//{
		//}
	}
}
