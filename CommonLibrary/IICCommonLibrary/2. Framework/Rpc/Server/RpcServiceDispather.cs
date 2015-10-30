/*
 * RpcServiceDispather:
 *	Rpc服务调度器: RpcServiceManager的扩充，增加了专用计数器，以及对Service Busy的保护
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcServiceDispather
	{
		// private int _concurrentThreads;
		private bool _started;
		private object _syncRoot = new object();
		private Dictionary<string, RpcServiceBase> _services;
		private RpcServerPerfCounter _perfCounter = IICPerformanceCounterFactory.GetCounters<RpcServerPerfCounter>();

		public RpcServiceDispather(string name)
		{
			_started = false;
			_services = new Dictionary<string, RpcServiceBase>();
		}

		public void Start()
		{
			lock (_syncRoot) {
				_started = true;
			}
		}

		public void RegisterService(RpcServiceBase service)
		{
			if (_started)
				throw new NotSupportedException("you can't register service after start");
			lock (_syncRoot) {
				_services.Add(service.ServiceName, service);
			}
		}

		public void ProcessTransaction(RpcServerTransaction tx)
		{
			RpcServerContext context = null;
			try {
				context = new RpcServerContext(tx);

				RpcServiceBase serviceBase;
				_perfCounter.InvokePerSec.Increment();
				_perfCounter.InvokeTotal.Increment();
				_perfCounter.ConcurrentThreads.Increment();
				_perfCounter.ConcurrentContext.Increment();

				if (_services.TryGetValue(context.ServiceName, out serviceBase)) {
					serviceBase.OnTransactionStart(context);
				} else {
					context.ReturnError(RpcErrorCode.ServiceNotFound, new Exception(context.ServiceName + " NotFound"));
				}
			} catch (RpcException ex) {
				context.ReturnError(ex.RpcCode, ex);
			} catch (Exception ex) {
				context.ReturnError(RpcErrorCode.ServerError, ex);
			} finally {
				_perfCounter.ConcurrentThreads.Decrement();
			}
		}
	}
}
