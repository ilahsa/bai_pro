using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		调用Rpc接口的客户端代理，不允许重用s，每次从RpcProxyFactory.GetProxy中创建
	/// </summary>
	public sealed class RpcClientProxy
	{
		#region Private Static Fields
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcClientProxy));
		private static RpcClientPerfCounter _perfCounters = IICPerformanceCounterFactory.GetCounters<RpcClientPerfCounter>();
		#endregion

		#region Protected Fields
		private int _timeout = -1;
        private RpcConnection _conn;
		private ResolvableUri _contextUri;
		private RpcClientInterface _serviceInterface;
		#endregion

		#region Public Properties
		/// <summary>
		///		服务名称
		/// </summary>
		public string ServiceName
		{
			get { return _serviceInterface.ServiceName; }
		}

		/// <summary>
		///		调用超时, 毫秒
		/// </summary>
		/// <remarks>不设置的话，会采用Channel上的默认值</remarks>
		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}
		#endregion

		#region Constructor
		internal RpcClientProxy(RpcConnection conn, RpcClientInterface intf, ResolvableUri toUri)
		{
			_conn = conn;
			_contextUri = toUri;
			_serviceInterface = intf;
		}
		#endregion

		#region Public Methods
		/// <summary>
		///		无参数的异步的调用
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <param name="callback">接受回调的delegate</param>
		public void BeginInvoke(string methodName, Action<RpcClientContext> callback)
		{
			BeginInvoke<RpcNull>(methodName, null, callback);
		}

		/// <summary>
		///		带参数的异步的调用
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <typeparam name="TArgs">调用的参数类型</typeparam>
		/// <param name="args">调用的参数</param>
		/// <param name="callback">接受回调的delegate</param>
		public void BeginInvoke<TArgs>(string methodName, TArgs args, Action<RpcClientContext> callback)
		{
			IICAssert.IsNotNull(callback);

			//
			// 如果Interface声明为强类型检查（默认），则需要判断是否为合法的MethodName和类型
			bool isBatch = false;
			RpcClientMethodSensor method = null;
			
			if (_serviceInterface.ClientCheck) {
				method = _serviceInterface.GetMethod(methodName);
				if (method == null) {
					string msg = string.Format("RpcService {0} not exists methods: {1}, please check your code", _serviceInterface.ServiceName, methodName);
					throw new NotSupportedException(msg);
				}

				bool typeChecked = (method.ArgsType == null || method.ArgsType == typeof(RpcNull)) && (typeof(TArgs) == typeof(RpcNull)) 
					|| method.ArgsType == typeof(TArgs);
				
				if (!typeChecked) {
					string msg = string.Format("RpcMethod {0}.{1} Expired type<{2}>, not <{3}>", 
						_serviceInterface.ServiceName, methodName, 
						method.ArgsType == null ? "NULL" : ObjectHelper.GetTypeName(method.ArgsType, false),
						ObjectHelper.GetTypeName(typeof(TArgs), false));
					//
					// TODO: 暂时只记录错误Trace，不抛出异常
					// throw new NotSupportedException(msg);
					_tracing.Error(msg);
				} 

				if (method.BatchManager != null) {
					isBatch = true;
				}
			} 

			RpcRequest request = new RpcRequest(_serviceInterface.ServiceName, methodName, _contextUri);
			if (typeof(TArgs) == typeof(RpcNull) || args == null) {
				request.BodyBuffer = null;
			} else {
				request.BodyBuffer = new RpcBodyBuffer<TArgs>(args);
			}

			RpcClientTransaction trans;
			if (isBatch) {
				_perfCounters.BatchPerSec.Increment();
				_perfCounters.BatchTotal.Increment();
				_perfCounters.BatchConcurrent.Increment();

                trans = method.BatchManager.CreateTransaction(_conn.RemoteUri, request);
			} else {
				_perfCounters.InvokeTotal.Increment();
				_perfCounters.InvokePerSec.Increment();
				_perfCounters.ConcurrentContext.Increment();

                trans = _conn.CreateTransaction(request);
			}

			RpcClientContext ctx = new RpcClientContext(trans);
			ctx.SendRequest(
				delegate(long ticks, RpcClientContext cx2, bool successed) {
					if (isBatch) {
						_perfCounters.BatchConcurrent.Decrement();
					} else {
						_perfCounters.ConcurrentContext.Decrement();
					}
					_perfCounters.AvgInvokeElapseMs.IncrementBy(ticks / FrequencyHelper.TicksPerMs);

					if (!successed)
						_perfCounters.InvokeFailed.Increment();

					try {
						callback(cx2);
					} catch (Exception ex) {
						_tracing.Error(ex, "BeginInvoke.Calblack Failed");
					}
				},
				_timeout
			);
		}

		/// <summary>
		///		不关心返回结果的调用, 但错误会被Trace系统记录
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <typeparam name="TArgs">调用的参数类型</typeparam>
		/// <param name="args">调用的参数</param>
		/// <remarks></remarks>
		public void InvokeOneway<TArgs>(string methodName, TArgs args)
		{
			BeginInvoke<TArgs>(methodName, args, 
				delegate(RpcClientContext ctx) {
					try {
						ctx.EndInvoke();
					} catch (Exception ex) {
						_tracing.ErrorFmt(ex, "InvokeOneway Failed");
					}
				}
			);
		}

		/// <summary>
		///		同步调用Rpc接口, 仅用于希望同步操作的场合
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <typeparam name="TArgs">调用的参数类型</typeparam>
		/// <param name="args">调用的参数</param>
		/// <typeparam name="TResults">返回参数类型</typeparam>
		/// <returns>Rpc调用返回值</returns>
		/// <remarks>一般来说同步Rpc的允许的场合包括:
		///		1. 读取配置
		///		2. 在固定线程中的, 从队列中取出数据的并通过Rpc处理, 避免对服务的压力并保持顺序的一致性
		///		3. 单元测试
		/// </remarks>
		public TResults Invoke<TArgs, TResults>(string methodName, TArgs args)
		{
			return Invoke<TArgs, TResults>(methodName, args, _timeout);	// use Channel Default Timeout
		}

		/// <summary>
		///		同步调用Rpc接口, 仅用于希望同步操作的场合
		/// </summary>
		/// <param name="methodName">方法名</param>
		/// <typeparam name="TArgs">调用的参数类型</typeparam>
		/// <param name="args">调用的参数</param>
		/// <typeparam name="TResults">返回参数类型</typeparam>
		/// <param name="timeoutMs">超时时间, 毫秒ms</param>
		/// <returns>Rpc调用返回值</returns>
		/// <remarks>一般来说同步Rpc的允许的场合包括:
		///		1. 读取配置
		///		2. 在固定线程中的, 从队列中取出数据的并通过Rpc处理, 避免对服务的压力并保持顺序的一致性
		///		3. 单元测试
		/// </remarks>
		public TResults Invoke<TArgs, TResults>(string methodName, TArgs args, int timeoutMs)
		{
			var a = new SyncInvoker<RpcClientContext>();
			BeginInvoke<TArgs>(methodName, args, a.Callback);

			if (a.Wait(timeoutMs)) {
				return a.Context.EndInvoke<TResults>();
			} else {
				throw new RpcException(RpcErrorCode.TransactionTimeout, "", "TransactionTimeout", null);
			}
		}
		#endregion
	}
}