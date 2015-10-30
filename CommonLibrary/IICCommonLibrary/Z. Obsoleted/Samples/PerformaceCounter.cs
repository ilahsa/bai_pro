using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Samples
{
	[IICPerformanceCounters("Imps:SampleCategory", CategoryType = PerformanceCounterCategoryType.MultiInstance)]
	class SamplePerformanceCounter
	{
		[IICPerformanceCounter("SampleCounter", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter SampleCounter = null;
	}

	class PerformaceCounterSample
	{
		public void Sample1() // 旧有模式
		{
			SamplePerformanceCounter samplePerf = IICPerformanceCounterFactory.GetCounters<SamplePerformanceCounter>();
			samplePerf.SampleCounter.Increment();
		}

		public void Sample2() // V4 新增模式
		{
			IICPerformanceCounterCategory category = new IICPerformanceCounterCategory("Imps:SampleCategory", PerformanceCounterCategoryType.MultiInstance);
			Dictionary<int, IICPerformanceCounter> counters = new Dictionary<int, IICPerformanceCounter>();
			for (int i = 0; i < 5; i++) {
				counters.Add(i, category.CreateCounter("SampleCounter-" + i, PerformanceCounterType.RateOfCountsPerSecond32));
			}
			IICPerformanceCounterFactory.GetCounters(category);

			for (int i = 0; i < 5; i++) {
				counters[i].Increment();
			}
		}
	}
}
