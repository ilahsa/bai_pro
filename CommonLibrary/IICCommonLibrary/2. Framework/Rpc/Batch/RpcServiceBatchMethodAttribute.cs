using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class RpcServiceBatchMethodAttribute: Attribute
	{
		public RpcServiceBatchMethodAttribute(int batchCount, int idleMs)
		{
			BatchCount = batchCount;
			IdleMs = idleMs;
		}

		public string MethodName
		{
			get;
			set;
		}

		public Type ArgsType
		{
			get;
			set;
		}

		public Type ResultType
		{
			get;
			set;
		}

		public int BatchCount
		{
			get;
			set;
		}

		public int IdleMs
		{
			get;
			set;
		}
	}
}
