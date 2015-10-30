using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	public class DebugTraceListener: TraceListener
	{
		private ITracing _tracing = TracingManager.GetTracing("Trace");

		public override void Write(string message)
		{
			_tracing.Info(message);
		}

		public override void WriteLine(string message)
		{
			_tracing.Info(message);
		}

		public override void Fail(string message)
		{
			_tracing.Error(message);
		}
	}
}
