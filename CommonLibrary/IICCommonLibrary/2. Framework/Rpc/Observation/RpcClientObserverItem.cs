using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcClientObserverItem: PerformanceObserverItem
	{
		[ObserverField]
		public string ServerUri;

		[ObserverField]
		public string Service;

		[ObserverField]
		public string Method;

		[ObserverField]
		public string ServiceRole;

		public ITracing RequestTracer;

		public ITracing ResponseTracer;

		public RpcClientObserverItem(string serverUri, string service, string method, string serviceRole)
		{
			ServerUri = serverUri;
			Service = service;
			Method = method;
			ServiceRole = serviceRole;

			RequestTracer = TracingManager.GetTracing(string.Format("RpcClient.{0}.{1}.Request", service, method));
			ResponseTracer = TracingManager.GetTracing(string.Format("RpcClient.{0}.{1}.Response", service, method));
		}
	}
}
