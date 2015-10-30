/*
 * Asynchronous Call Helper
 * 
 * Gaolei 2007-07-18
 * useful anonymous delegate
 */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public delegate void AnonymousDelegate();
	public delegate void EndInvokeAnonymousDelegate(IAsyncResult result);
	/// <summary>
	///		异步调用的Helper class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AsyncCallHelper<T>
	{
		private T _delegate;
		private AsyncCallback _asyncCallBack;
		private EndInvokeAnonymousDelegate _endInvokeDelegate;
        private int _retryCount = 0;

		public T Delegate
		{
			get { return _delegate; }
		}

		public AsyncCallback Callback 
		{
			get { return _asyncCallBack; }
		}

		public AsyncCallHelper(T methodDelegate)
		{
			_delegate = methodDelegate;
			_asyncCallBack = new AsyncCallback(CallbackProc);
		}

        public AsyncCallHelper(T methodDelegate, int retryCount) {
            _delegate = methodDelegate;
            _asyncCallBack = new AsyncCallback(CallbackProc);
            _retryCount = retryCount;
        }
	
		private void CallbackProc(IAsyncResult result)
		{
			try {
				_endInvokeDelegate.Invoke(result);
			} catch (Exception ex) {
				Trace.WriteLine(string.Format("AsyncCallHelper<{0}>.CallbackProc Failed:\r\n{1}", typeof(T).Name, ex.ToString()));
			}
		}

		public void Invoke(AnonymousDelegate begin, EndInvokeAnonymousDelegate end)
		{
			_endInvokeDelegate = end;
			begin();
		}
	}
}
