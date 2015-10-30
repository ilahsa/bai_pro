using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[IICPerformanceCounters("rpc:Client", PerformanceCounterCategoryType.MultiInstance)]
	class RpcClientPerfCounter
	{
		[IICPerformanceCounter("Invoke /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter InvokePerSec = null;

		[IICPerformanceCounter("Invoke Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter InvokeTotal = null;

		[IICPerformanceCounter("Invoke Failed,", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter InvokeFailed = null;

		[IICPerformanceCounter("Batch /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter BatchPerSec = null;

		[IICPerformanceCounter("Batch Concurrent.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter BatchConcurrent = null;

		[IICPerformanceCounter("Batch Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter BatchTotal = null;
		
		[IICPerformanceCounter("Concurrent Context.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ConcurrentContext = null;

		[IICPerformanceCounter("Invoke Elapse Avg Ms.", PerformanceCounterType.AverageCount64)]
		public IICPerformanceCounter AvgInvokeElapseMs = null;
	}
}
