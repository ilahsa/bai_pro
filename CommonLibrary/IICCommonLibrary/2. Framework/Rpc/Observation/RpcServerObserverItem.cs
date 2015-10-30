using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcServerObserverItem: PerformanceObserverItem
	{
		[ObserverField(ObserverColumnType.String)]
		public string Service = string.Empty;

		[ObserverField(ObserverColumnType.String)]
		public string Method = string.Empty;

		[ObserverField(ObserverColumnType.String)]
		public string FromService = string.Empty;

		[ObserverField(ObserverColumnType.String)]
		public string FromComputer = string.Empty;

		public ITracing RequestTracer;

		public ITracing ResponseTracer;

		public RpcServerObserverItem(string service, string method, string fromService, string fromComputer)
		{
			Service = service;
			Method = method;
			FromService = fromService;
			FromComputer = fromComputer;

			RequestTracer = TracingManager.GetTracing(string.Format("RpcServer.{0}.{1}.Request", service, method));
			ResponseTracer = TracingManager.GetTracing(string.Format("RpcServer.{0}.{1}.Response", service, method));
		}
	}
}
