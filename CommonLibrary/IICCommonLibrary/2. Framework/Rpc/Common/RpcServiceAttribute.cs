using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public enum RpcPerformanceCounterMode
	{
		None,
		Client,
		Server,
		Both
	}

	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
	public class RpcServiceAttribute: Attribute
	{
		private string _serviceName;
		private RpcPerformanceCounterMode _enableCounters = RpcPerformanceCounterMode.None;
		private bool _typeChecking = true;

		public string ServiceName
		{
			get { return _serviceName; }
		}

		public RpcServiceAttribute(string serviceName)
		{
			_serviceName = serviceName;
		}

		/// <summary>
		///		是否在客户端进行强制命名检查
		/// </summary>
		public bool ClientChecking
		{
			get { return _typeChecking; }
			set { _typeChecking = value; }
		}

		/// <summary>
		///		
		/// </summary>
		public RpcPerformanceCounterMode EnableCounters
		{
			get { return _enableCounters; }
			set { _enableCounters = value; }
		}
	}
}
