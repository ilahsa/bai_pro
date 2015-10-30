using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Tracing;

namespace UnitTest
{
	class TracingSnifferService: ITracingSniffer
	{
		public ManualResetEvent WaitEvent = new ManualResetEvent(false);
		
		public void AppendTrace(RpcServerContext context)
		{
			RpcList<TracingEvent> evts = context.GetArgs<RpcList<TracingEvent>>();
			foreach (TracingEvent evt in evts.Value) {
				Console.WriteLine(evt.ToString());
			}
			WaitEvent.Set();
			context.Return();
		}

		public void AppendSystemLog(RpcServerContext context)
		{
			RpcList<SystemLogEvent> evts = context.GetArgs<RpcList<SystemLogEvent>>();
			foreach (SystemLogEvent evt in evts.Value) {
				Console.WriteLine(evt.ToString());
			}
			WaitEvent.Set();
			context.Return();			
		}

		public void AppendConsole(RpcServerContext context)
		{
			throw new NotImplementedException();
		}
	}
}
