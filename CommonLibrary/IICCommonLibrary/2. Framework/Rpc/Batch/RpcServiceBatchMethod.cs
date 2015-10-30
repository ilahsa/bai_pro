using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcServiceBatchMethod: RpcServiceMethod
	{
		public RpcServiceBatchMethod(object service, IICPerformanceCounterCategory category, string methodName, MethodInfo method, bool enableCounters)
			: base(service, category, methodName, method, enableCounters)
		{
		}

		public override void Call(RpcServerContext ctx)
		{
			if (RatePerSecond != null) {
				try {
					if (RatePerSecond != null) {
						RatePerSecond.Increment();
						Concurrent.Increment();
					}
					Method.Invoke(Service, new object[] { new RpcBatchServerContext(ctx) });
				} catch (Exception ex) {
					ctx.ReturnError(RpcErrorCode.ServerError, ex);
				} finally {
					Concurrent.Decrement();
				}	
			} else {
				try {
					Method.Invoke(Service, new object[] { new RpcBatchServerContext(ctx) });
				} catch (Exception ex) {
					ctx.ReturnError(RpcErrorCode.ServerError, ex);
				}
			}
		}
	}
}
