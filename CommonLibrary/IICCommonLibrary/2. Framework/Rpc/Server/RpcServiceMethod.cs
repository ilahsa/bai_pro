using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcServiceMethod
	{
		public string MethodName;

		protected IICPerformanceCounter RatePerSecond;
		protected IICPerformanceCounter Concurrent;
		protected MethodInfo Method;
		protected object Service;

		public RpcServiceMethod(object service, IICPerformanceCounterCategory category, string methodName, MethodInfo method, bool enableCounters)
		{
			MethodName = methodName;
			Method = method;
			Service = service;

			if (enableCounters) {
				RatePerSecond = category.CreateCounter(methodName + " /sec.", PerformanceCounterType.RateOfCountsPerSecond32);
				Concurrent = category.CreateCounter(methodName + " Concurrent.", PerformanceCounterType.NumberOfItems32);
			} else {
				RatePerSecond = null;
				Concurrent = null;
			}
		}

		public virtual void Call(RpcServerContext ctx)
		{
			if (RatePerSecond != null) {
				try {
					RatePerSecond.Increment();
					Concurrent.Increment();
					Method.Invoke(Service, new object[] { ctx });
				} catch (Exception ex) {
					ctx.ReturnError(RpcErrorCode.ServerError, ex);
				} finally {
					Concurrent.Decrement();
				}	
			} else {
				try {
					Method.Invoke(Service, new object[] { ctx });
				} catch (Exception ex) {
					ctx.ReturnError(RpcErrorCode.ServerError, ex);
				}
			}
		}
	}
}
