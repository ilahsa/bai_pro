using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public class RpcServiceDecorator<T>: RpcServiceBase
	{
		private object _serviceObj;
		private HybridDictionary<string, RpcServiceMethod> _methods;

		public RpcServiceDecorator(T serviceObj)
			: this(serviceObj, null)
		{
		}

		public RpcServiceDecorator(T serviceObj, string serviceName): base(string.Empty)
		{
			Type intf = typeof(T);
			if (!intf.IsInterface)
				throw new NotSupportedException();

			RpcServiceAttribute serviceAttr = AttributeHelper.GetAttribute<RpcServiceAttribute>(intf);
			if (!string.IsNullOrEmpty(serviceName)) {
				p_serviceName = serviceName;
			} else {
				p_serviceName = serviceAttr.ServiceName;
			}

			_serviceObj = serviceObj;
			_methods = new HybridDictionary<string, RpcServiceMethod>();

			bool enableCounter = serviceAttr.EnableCounters == RpcPerformanceCounterMode.Both || serviceAttr.EnableCounters == RpcPerformanceCounterMode.Server;

			IICPerformanceCounterCategory category = new IICPerformanceCounterCategory("rpc:" + p_serviceName, PerformanceCounterCategoryType.MultiInstance);

			foreach (MethodInfo method in intf.GetMethods()) {
				string methodName = method.Name;
				RpcServiceMethod m;

				RpcServiceBatchMethodAttribute battr = AttributeHelper.TryGetAttribute<RpcServiceBatchMethodAttribute>(method);
				if (battr != null) {
					if (!string.IsNullOrEmpty(battr.MethodName))
						methodName = battr.MethodName;

					m = new RpcServiceBatchMethod(serviceObj, category, methodName, method, enableCounter);
				} else {
					RpcServiceMethodAttribute attr = AttributeHelper.GetAttribute<RpcServiceMethodAttribute>(method);
					if (!string.IsNullOrEmpty(attr.MethodName))
						methodName = attr.MethodName;

					m = new RpcServiceMethod(serviceObj, category, methodName, method, enableCounter);
				}

				_methods.Add(methodName, m);
			}
			IICPerformanceCounterFactory.GetCounters(category);
		}

		public override void OnTransactionStart(RpcServerContext ctx)
		{
			RpcServiceMethod method;
			if (!_methods.TryGetValue(ctx.MethodName, out method)) {
				ctx.ReturnError(RpcErrorCode.MethodNotFound, null);
				return;
			} else {
				method.Call(ctx);
			}
		}
	}
}
