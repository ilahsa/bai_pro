/*
 * class:	RpcClientContext
 *	管理Transaction和回调之间的桥梁，
 *	发出RpcRequest，并接受RpcResponse
 * 
 * GaoLei
 * 2010-06-11 变更RpcFramework的基础类
 */
using System;
using System.Diagnostics;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		管理Rpc客户端调用的异步上下文
	/// </summary>
	public sealed class RpcClientContext
	{
		private RpcClientTransaction _trans;
		private RpcClientObserverItem _observer;
		private Action<long, RpcClientContext, bool> _callback;

		public RpcClientContext(RpcClientTransaction trans)
		{
			_trans = trans;
		}

		public void SendRequest(Action<long, RpcClientContext, bool> callback, int timeout)
		{
			RpcRequest request = _trans.Request;
			Stopwatch watch = new Stopwatch();

			_callback = callback;
			_observer = RpcObserverManager.GetClientItem(_trans.ServerUri.ToString(), request.Service, request.Method, _trans.ServiceRole); 

			watch.Start();

			try {
				_trans.SendRequest(
					delegate {
						var response = _trans.Response;
						long elapseTicks = watch.ElapsedTicks;
						bool successed = response.ErrorCode == RpcErrorCode.OK;
						_observer.Track(successed, response.Error, elapseTicks);
						_callback(elapseTicks, this, successed);
					},
					timeout
				);

				TracingManager.Info(
					delegate() {
						_observer.RequestTracer.InfoFmt2(
							request.ServiceAtComputer,
							request.ContextUri,
							"Args = {0}",
							ObjectHelper.DumpObject(request.BodyValue)
						);
					}
				);
			} catch (Exception ex) {
				_observer.RequestTracer.ErrorFmt2(
					ex,
					request.FromService,
					request.ContextUri.ToString(),
					"Args = {0}",
					ObjectHelper.DumpObject(request.BodyValue)
				);

				var response = RpcResponse.Create(RpcErrorCode.SendFailed, ex);
				long elapseTicks = watch.ElapsedTicks;
				_observer.Track(false, response.Error, elapseTicks);
				_callback(elapseTicks, this, false);
			}
		}

		public void EndInvoke()
		{
			EndInvoke<RpcNull>();
		}

		public T EndInvoke<T>()
		{
			T retValue;
			var request = _trans.Request;
			var response = _trans.Response;

			if (response.ErrorCode != RpcErrorCode.OK) {
				throw new RpcException(response.ErrorCode, _trans.ServiceUrl, "RpcResponse Failed", response.Error);
			}

			if (typeof(T) == typeof(RpcNull) || _trans.Response.BodyBuffer == null) {
				retValue = default(T);
			} else {
				try {
					retValue = _trans.Response.BodyBuffer.GetValue<T>();
				} catch (Exception ex) {
					byte[] buffer = _trans.Response.BodyBuffer.GetByteArray();

                    _observer.RequestTracer.WarnFmt2(
						ex,
						request.ServiceAtComputer,
						request.ContextUri,
						"ResponseBuffer byte[{0}] = {1}",
						buffer.Length,
						StrUtils.ToHexString(buffer, 500)
					);
					throw new RpcException(RpcErrorCode.InvaildResponseArgs, _trans.ServiceUrl, "RpcClientContext.EndInvoke<T>, Failed", ex);
				}
			}

			TracingManager.Info(
				delegate() {
					_observer.ResponseTracer.InfoFmt2(
						request.FromService,
						request.ContextUri,
						"Args={0}\r\nResults={1}",
						ObjectHelper.DumpObject(request.BodyValue),
						ObjectHelper.DumpObject(response.BodyValue)
					);
				}
			);

			return retValue;
		}
	}
}

