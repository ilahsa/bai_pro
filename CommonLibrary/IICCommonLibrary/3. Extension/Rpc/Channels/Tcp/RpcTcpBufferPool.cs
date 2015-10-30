/*
 * 一个高效率的对象缓冲池，用于缓冲可复用的对象
 * 有无锁和高效的有点
 * 
 * 在乐观情况下， 获取和放回的算法优先级均是O(C)，
 * 悲观情况下，存在大量连续的长非可用对象，添加的算法优先级是O(N)
 * 
 * Create by 高磊 2010-06-22, 
 *		感谢陈鹏的修改建议
 */
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public interface IObjectPoolCacheable
	{
		int CachedIndex { get; set; }
		void FetchPrepare();
		void Release();
	}

	/// <summary>
	///		高效的对象缓冲池
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class RpcTcpBufferPool<T> where T: IObjectPoolCacheable
	{
		private int _head;
		private int _capacity;
		private int _capacityMask;
		private int _count;

		private CachedNode[] _nodes;

		private class CachedNode
		{
			public int Index;
			public int Fetched;
			public T Value;
		}

		public RpcTcpBufferPool(int capacity, Func<T> allocator)
		{
			//
			// 所有的capacity都是2的幂
			IICAssert.IsTrue(capacity < 1024 * 1024);

			_capacity = NumberUtils.NextPower2(capacity);
			_nodes = new CachedNode[_capacity];
			_capacityMask = _capacity - 1;
			_count = _capacity;

			//
			// 填充满所有对象
			for (int i = 0; i < _capacity; i++) {
				var node = new CachedNode() {
					Value = allocator(),
					Fetched = 0,
					Index = i,
				};

				node.Value.CachedIndex = i;
				_nodes[i] = node;
			}
		}

		public T Fetch()
		{
			//
			// 先减小对象数，避免容器空
			int count = Interlocked.Decrement(ref _count);
			if (count < 0) {
				Interlocked.Increment(ref _count);
				return default(T);
			}

			//
			// 在head后寻找第一个可用，然后获取值
			int index = -1;
			CachedNode node = null;
			do {
				index = Interlocked.Increment(ref _head) & _capacityMask;
				node = _nodes[index];
			} while (Interlocked.CompareExchange(ref node.Fetched, 1, 0) != 0);
			return node.Value;
		}

		public void Release(T value)
		{
			CachedNode node = _nodes[value.CachedIndex];
			//
			// 实际Release掉了, 就增加可用数量
            //原先FetchPrepare在CompareExchange里面，会导致node马上被另一个线程fetch使用后，却被FetchPrepare（）清了所有内容
            //造成callback不能回调，依附sokcket sendpending一直增加，这个asynarg 不能会导池内
            node.Value.FetchPrepare();
			if (Interlocked.CompareExchange(ref node.Fetched, 0, 1) == 1) {

				Interlocked.Increment(ref _count);
			}
		}

		public int Count
		{
			get { return _count; }
		}

		public int Capacity
		{
			get { return _capacity; }
		}

		public void Dispose()
		{
			foreach (var n in _nodes) {
				if (n != null) {
					n.Value.Release();
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
