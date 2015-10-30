using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Dtc
{
	[IICPerformanceCounters("Imps:TccTransaction")]
	class TccTransactionPerfCounter
	{
		[IICPerformanceCounter("Transaction Open /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter TransOpenPerSec = null;

		[IICPerformanceCounter("Transaction Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter TransactionTotal = null;

		[IICPerformanceCounter("Transaction Failed.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter TransactionFailed = null;

		[IICPerformanceCounter("Active Transaction.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ActiveTransaction = null;

		[IICPerformanceCounter("Avg Elapse Ms.", PerformanceCounterType.AverageCount64)]
		public IICPerformanceCounter AvgTransactionElapseMs = null;
	}
}
