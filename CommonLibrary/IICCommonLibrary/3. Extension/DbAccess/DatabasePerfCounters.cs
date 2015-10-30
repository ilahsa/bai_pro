using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
	[IICPerformanceCounters("Imps:DbAccess", PerformanceCounterCategoryType.MultiInstance)]
    class DatabasePerfCounters
    {
		[IICPerformanceCounter("Commands Executed /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter CommandExecutedPerSec = null;

		[IICPerformanceCounter("Commands Executed Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter CommandExecutedTotal = null;

		[IICPerformanceCounter("Commands Failed Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter CommandFailedTotal = null;

        [IICPerformanceCounter("Avg Command Execution Ms.", PerformanceCounterType.AverageCount64)]
        public IICPerformanceCounter AvgExecuteMs = null;

		[IICPerformanceCounter("BulkInsert Rows /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter BulkInsertRowsPerSec = null;

		[IICPerformanceCounter("BulkInsert Elapse Ms.", PerformanceCounterType.AverageCount64)]
		public IICPerformanceCounter BulkInsertAvgElapseMs = null;
    }
}