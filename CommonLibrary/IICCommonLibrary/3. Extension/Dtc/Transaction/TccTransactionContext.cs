using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public class TccTransactionContext<TContext> where TContext: ITccContext
	{
		private int _hasReturned;
		private TccTransaction<TContext> _trans;
		private Action<TccTransactionContext<TContext>> _callback;
		private TccTransactionException<TContext> _exception = null;

		internal TccTransactionContext(TccTransaction<TContext> trans, Action<TccTransactionContext<TContext>> callback)
		{
			_trans = trans;
			_callback = callback;
			_hasReturned = 0;
		}

		internal void Return()
		{
			if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) == 0) {
				_callback(this);
			} else {
				// TODO: 报错
			}
		}

		internal void ThrowException(string message, Exception ex)
		{
			//
			// Interlocked.CompareExchange 防止重复回调
			if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) == 0) {
				_exception = new TccTransactionException<TContext>(_trans, message, ex);
				_callback(this);
			} else {
				// TODO: 报错
			}
		}

		public TContext ContextValue
		{
			get { return _trans.Context; }
		}

		public TContext EndInvoke()
		{
			if (_exception != null)
				throw _exception;

			return _trans.Context;
		}
	}
}
