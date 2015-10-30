using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	[RpcService("TracingSniffer")]
	public interface ITracingSniffer
	{
		[RpcServiceMethod(ArgsType = typeof(TracingEvent[]), ResultType = null)]
		void AppendTrace(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(SystemLogEvent[]), ResultType = null)]
		void AppendSystemLog(RpcServerContext context);
	}
}
