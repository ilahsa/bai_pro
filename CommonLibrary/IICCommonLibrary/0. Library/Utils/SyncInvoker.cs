/*
 * 用于消除异步转同步时冗余代码的辅助类
 * 
 * Create by Gao Lei
 *		2010-08-04
 *		
 *	开发的核心问题就是将复杂的问题简单化，并排除冗余
 *	
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		用于消除异步转同步时冗余代码的辅助类, 基类
	/// </summary>
	public class SyncInvokerBase
	{
		#region Private
		protected ManualResetEvent _evt;

		protected void SetEvent()
		{
			_evt.Set();
		}
		#endregion

		#region Public
		/// <summary>
		///		创建一个AsyncInvoker调用器
		/// </summary>
		protected SyncInvokerBase()
		{
			_evt = new ManualResetEvent(false);
		}

		/// <summary>
		///		等待
		/// </summary>
		/// <param name="timeoutMs">等待超时，-1为无限等待</param>
		/// <returns>在时限内返回：true</returns>
		public bool Wait(int timeoutMs)
		{
			return _evt.WaitOne(timeoutMs);
		}
		#endregion
	}

	/// <summary>
	///		用于消除异步转同步时冗余代码的辅助类，适用于无参数的异步回调
	/// </summary>
	public class SyncInvoker: SyncInvokerBase
	{
		private Action _callback;

		public SyncInvoker()
			: base()
		{
			_callback = delegate {
				_evt.Set();
			};
		}

		/// <summary>
		///		提供给异步回调使用的Callback函数
		/// </summary>
		public Action Callback
		{
			get { return _callback; }
		}
	}

	/// <summary>
	///		用于消除异步转同步时冗余代码的辅助类，适用于一个参数的异步回调
	/// </summary>
	/// <typeparam name="TContext">异步回调参数类型</typeparam>
	public class SyncInvoker<TContext>: SyncInvokerBase
	{
		private TContext _ctx;
		private Action<TContext> _callback;

		/// <summary>
		///		创建一个AsyncInvoker调用器
		/// </summary>
		public SyncInvoker()
			: base()
		{
			_callback = delegate(TContext ctx) {
				_ctx = ctx;
				_evt.Set();
			};
		}

		/// <summary>
		///		提供给异步回调使用的Callback函数
		/// </summary>
		public Action<TContext> Callback
		{
			get { return _callback; }
		}

		/// <summary>
		///		异步回调的上下文参数
		/// </summary>
		public TContext Context
		{
			get { return _ctx; }
		}
	}

	/// <summary>
	///		用于消除异步转同步时冗余代码的辅助类，适用于一个参数的异步回调
	/// </summary>
	/// <typeparam name="C1">异步回调参数1类型</typeparam>
	/// <typeparam name="C2">异步回调参数2类型</typeparam>
	public class SyncInvoker<C1, C2>: SyncInvokerBase
	{
		private C1 _c1;
		private C2 _c2;
		private Action<C1, C2> _callback;

		/// <summary>
		///		创建一个AsyncInvoker调用器
		/// </summary>
		public SyncInvoker()
			: base()
		{
			_callback = delegate(C1 c1, C2 c2) {
				_evt.Set();
				_c1 = c1;
				_c2 = c2;
			};
		}

		/// <summary>
		///		提供给异步回调使用的Callback函数
		/// </summary>
		public Action<C1, C2> Callback
		{
			get { return _callback; }
		}

		/// <summary>
		///		异步回调的参数1
		/// </summary>
		public C1 Param1
		{
			get { return _c1; }
		}

		/// <summary>
		///		异步回调的参数2
		/// </summary>
		public C2 Param2
		{
			get { return _c2; }
		}
	}
}
