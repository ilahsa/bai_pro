using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

using Google.ProtoBuf;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		在Server端管理Rpc方法
	/// </summary>
	public sealed class RpcServerContext
	{
		#region Private Members
		private Stopwatch _watch;
		private RpcRequest _request;
		private RpcServerTransaction _trans;
		private RpcServerObserverItem _observer;
		private ResolvableUri _contextUri;
		private int _hasReturned;
        private int _returnIfCount = 0;
		#endregion

		#region Public Properties
		public RpcRequest Request
		{
			get { return _request; }
		}
		/// <summary>
		///		来源 "服务@机器名"
		/// </summary>
		public string From
		{
			get { return _request.ServiceAtComputer; }
		}

		/// <summary>
		///		来源服务
		/// </summary>
		public string FromService
		{
			get { return _request.FromService; }
		}

		/// <summary>
		///		来源机器名
		/// </summary>
		public string FromComputer
		{
			get { return _request.FromComputer; }
		}

		/// <summary>
		///		访问Rpc服务名
		/// </summary>
		public string ServiceName
		{
			get { return _request.Service; }
		}

		/// <summary>
		///		访问Rpc方法名
		/// </summary>
		public string MethodName
		{
			get { return _request.Method; }
		}

		/// <summary>
		///		关联的Rpc连接
		/// </summary>
		/// <remarks>无连接模式下为null</remarks>
		public RpcConnection Connection
		{
			get { return _trans.Connection; }
		}

		/// <summary>
		///		关联的通道
		/// </summary>
		public RpcServerChannel Channel
		{
			get { return _trans.Channel; }
		}

		/// <summary>
		///		是否已经调用过Return方法
		/// </summary>
		/// <remarks>调用Return方法，或在方法中抛出异常，均视为返回到客户端，结果仅返回一次</remarks>
		public bool HasReturned
		{
			get { return _hasReturned != 0; }
		}

		/// <summary>
		///		上下文相关的Uri
		/// </summary>
		/// <remarks>可能为IdUri，GroupUri等，由客户端送上来的用户关联Uri</remarks>
		public ResolvableUri ContextUri
		{
			get
			{
				if (_contextUri != null) {
					return _contextUri;
				} else {
					if (!string.IsNullOrEmpty(_request.ContextUri)) {
						_contextUri = ResolvableUri.Parse(_request.ContextUri);
						return _contextUri;
					} else {
						return null;
					}
				}
			}
		}

		/// <summary>
		///		获取一个特定类型的上下文Uri
		/// </summary>
		/// <typeparam name="T">Uri类型</typeparam>
		/// <returns>T类型的Uri</returns>
		public T GetContextUri<T>() where T: ResolvableUri
		{
			return (T)ContextUri;
		}
		
		internal RpcServerContext(RpcServerTransaction trans)
		{
			_request = trans.Request;
			_trans = trans;
			_hasReturned = 0;

			_observer = RpcObserverManager.GetServerItem(_request.Service, _request.Method, _request.FromService, _request.FromComputer);
			_watch = new Stopwatch();
			_watch.Start();
		}

		/// <summary>
		///		获取请求参数
		/// </summary>
		/// <typeparam name="T">请求参数类型</typeparam>
		/// <returns>请求参数</returns>
		public T GetArgs<T>()
		{
			try {
				T ret;
				if (_request.BodyBuffer == null) {
					object a = null;
					ret = (T)a;
				} else {
					ret = _trans.Request.BodyBuffer.GetValue<T>();

					TracingManager.Info(
						delegate() {
							_observer.RequestTracer.InfoFmt2(
								_request.ServiceAtComputer,
								_request.ContextUri,
								"Args={0}",
								ObjectHelper.DumpObject(_trans.Request.BodyValue)
							);
						}
					);
				}
				return ret;
			} catch (Exception ex) {
				byte[] buffer = _trans.Request.BodyBuffer.GetByteArray();
				
				_observer.RequestTracer.ErrorFmt2(
					ex,
					_request.ServiceAtComputer,
					_request.ContextUri,
					"RequestBuffer byte[{0}] = {1}",
					buffer.Length,
					StrUtils.ToHexString(buffer, 500)
				);
				throw new RpcException(RpcErrorCode.InvaildRequestArgs, _trans.ServiceUrl, "RpcServerContext.GetArgs(), Failed", ex);
			}
		}

		/// <summary>
		///		无参数成功返回
		/// </summary>
		public void Return()
		{
			RpcResponse response = RpcResponse.Create(RpcErrorCode.OK);
			SendResponse(response);
		}

        /// <summary>
        /// 有条件的返回
        /// </summary>
        /// <typeparam name="TResults"></typeparam>
        /// <param name="results"></param>
        /// <param name="ifProc"></param>
        public void ReturnIf<TResults>(TResults results, Func<int, bool> ifProc)
        {
            lock (_trans)
            {
                _returnIfCount++;
                if (ifProc(_returnIfCount))
                {
                    Return<TResults>(results);
                }
            }
        }

		/// <summary>
		///		带参数成功返回
		/// </summary>
		/// <typeparam name="T">参数类型</typeparam>
		/// <param name="results">返回参数</param>
		/// <remarks>返回null等同于无参数返回</remarks>
		public void Return<T>(T results)
		{	
			RpcResponse response = RpcResponse.Create<T>(results);
			SendResponse(response);
		}

		/// <summary>
		///		抛出异常到客户端
		/// </summary>
		/// <param name="ex">异常</param>
		/// <remarks>等同于在方法中直接抛出异常，但可以在方法外异步抛出</remarks>
		public void ThrowException(Exception ex)
		{
			RpcResponse response = RpcResponse.Create(RpcErrorCode.ServerError, ex);
			SendResponse(response);
		}

		/// <summary>
		///		返回错误到客户端
		/// </summary>
		/// <param name="errCode">错误码</param>
		/// <param name="ex">异常</param>
		public void ReturnError(RpcErrorCode errCode, Exception ex)
		{
			RpcResponse response = RpcResponse.Create(errCode, ex);
			SendResponse(response);
		}

		public RpcClientProxy GetCallbackProxy<T>()
		{
			return new RpcClientProxy(_trans.Connection, RpcClientInterfaceFactory<T>.GetOne(), null);
		}

		public void SendResponse(RpcResponse response)
		{
			if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) != 0) {
				string msg = string.Format("Return more than once <{0}.{1}>", this.ServiceName, this.MethodName);
				throw new NotSupportedException(msg);
			}

			try {
				if (response.ErrorCode == RpcErrorCode.OK) {
					TracingManager.Info(
						delegate() {
							_observer.ResponseTracer.InfoFmt2(
								_request.FromService + "@" + _request.FromComputer,
								_request.ContextUri,
								"Args={0}\r\nResults={1}",
								ObjectHelper.DumpObject(_request.BodyValue),
								ObjectHelper.DumpObject(response.BodyValue)
							);
						}
					);
					_observer.Track(true, response.Error, (int)(_watch.ElapsedMilliseconds));
				} else {
					_observer.ResponseTracer.ErrorFmt2(
						response.Error,
						_request.FromService + "@" + _request.FromComputer,
						_request.ContextUri,
						"Args={0}",
						ObjectHelper.DumpObject(_request.BodyValue)
					);
					_observer.Track(false, response.Error, _watch.ElapsedMilliseconds);
					_perfCounters.InvokeFailed.Increment();
				}
				_trans.SendResponse(response);
			} catch (Exception innerEx) {
				SystemLog.Error(LogEventID.RpcFailed, innerEx, "RpcServerContext.ReturnError Failed");
			} finally {
				_perfCounters.ConcurrentContext.Decrement();
			}
		}
		#endregion

		#region Private static
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcServerContext));
		private static RpcServerPerfCounter _perfCounters = IICPerformanceCounterFactory.GetCounters<RpcServerPerfCounter>();
		#endregion
	}
}
