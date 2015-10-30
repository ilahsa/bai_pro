/*
 * 线程安全的循环计数器, 用于随机分发
 * 
 * 
 * Gao lei
 * 2010-08-11
 */ 
using System;
using System.Threading;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		严格保证线程安全的循环计数器
	/// </summary>
	/// <remarks>为保证性能，循环计数器的平均率，会有万分之一以下的误差</remarks>
	public sealed class LoopCounter
	{
		private int _n;
		private int _mask;
		private int _i;

		/// <summary>
		///		构造函数, 创建一个线程安全，顺序返回0..n-1的循环计数器
		/// </summary>
		/// <param name="n">上限</param>
		public LoopCounter(int n)
		{
			if (n > 10000) {
				throw new NotSupportedException("Only support n <= 10000");
			}
			if (n == 0) {
				throw new NotSupportedException("Bad parameter n == 0");
			}

			//
			// 
			_n = n;
			_mask = NumberUtils.NextPower2(n * 40000) - 1;
			_i = 0;
		}

		public int Count
		{
			get { return _n; }
		}
		
		/// <summary>
		///		重置为零
		/// </summary>
		public void Reset()
		{
			_i = 0;
		}

		/// <summary>
		///		获取下一个值，会在0..n-1之间顺序循环
		/// </summary>
		/// <returns>0..n-1，顺序循环的值</returns>
		public int Next()
		{
			int i = Interlocked.Increment(ref _i) & _mask;
			return i % _n;
		}
	}
}
