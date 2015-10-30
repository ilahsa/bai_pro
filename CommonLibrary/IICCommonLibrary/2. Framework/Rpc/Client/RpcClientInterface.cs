/*
 * Rpc实现interface的一个包装类，负责Methods的强名称检查，强调用类型检查，以及批量调用的信息获取
 * 
 * 
 */
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcClientInterface
	{
		public string ServiceName;

		public bool ClientCheck;

		public RpcPerformanceCounterMode EnableCounters;

		private HybridDictionary<string, RpcClientMethodSensor> _methods;

		public RpcClientInterface(Type intf)
		{
			if (!intf.IsInterface)
				throw new NotSupportedException();

			RpcServiceAttribute serviceAttr = AttributeHelper.GetAttribute<RpcServiceAttribute>(intf);
			ServiceName = serviceAttr.ServiceName;
			ClientCheck = serviceAttr.ClientChecking;
			EnableCounters = serviceAttr.EnableCounters;
			_methods = new HybridDictionary<string, RpcClientMethodSensor>();

			foreach (MethodInfo method in intf.GetMethods()) {
				RpcClientMethodSensor m;
				string methodName = method.Name;

				RpcServiceBatchMethodAttribute battr = AttributeHelper.TryGetAttribute<RpcServiceBatchMethodAttribute>(method);
				if (battr != null) {
					if (!string.IsNullOrEmpty(battr.MethodName))
						methodName = battr.MethodName;

					m = new RpcClientMethodSensor() {
						ArgsType = battr.ArgsType,
						BatchManager = new RpcClientBatchManager(battr.BatchCount, battr.IdleMs)
					};
				} else {
					RpcServiceMethodAttribute attr = AttributeHelper.GetAttribute<RpcServiceMethodAttribute>(method);

					if (!string.IsNullOrEmpty(attr.MethodName))
						methodName = battr.MethodName;

					m = new RpcClientMethodSensor() {
						ArgsType = attr.ArgsType,
						BatchManager = null
					};
				}
				_methods.Add(methodName, m);
			}
		}

		public RpcClientMethodSensor GetMethod(string methodName)
		{
			RpcClientMethodSensor sensor;
			if (_methods.TryGetValue(methodName, out sensor)) {
				return sensor;
			} else {
				return null;
			}
		}
	}
}
