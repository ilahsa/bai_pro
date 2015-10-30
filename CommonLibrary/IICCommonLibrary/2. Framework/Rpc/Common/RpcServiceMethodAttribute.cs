using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class RpcServiceMethodAttribute: Attribute
	{
		public RpcServiceMethodAttribute()
		{
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
	}
}
