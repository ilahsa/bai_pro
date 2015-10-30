using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	[IICPerformanceCounters("Imps:IITracing")]
	public class TracingPerfCounters
	{
		[IICPerformanceCounter("Info /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter InfoPerSec;

		[IICPerformanceCounter("Warn /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter WarnPerSec;

		[IICPerformanceCounter("Error /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter ErrorPerSec;

		[IICPerformanceCounter("Info Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter InfoTotal;

		[IICPerformanceCounter("Warn Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter WarnTotal;

		[IICPerformanceCounter("Error Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ErrorTotal;

		[IICPerformanceCounter("Queue Length.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter QueueLength;

		[IICPerformanceCounter("Appender Failed.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter AppenderFailed;
	}
}
