using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	static class IICPerformanceCounterMananger
	{
		public static void CreateCounters(string instance, IICPerformanceCountersAttribute categoryAttr, IEnumerable<IICPerformanceCounter> counters)
		{
			try {
				foreach (IICPerformanceCounter counter in counters) {
					EvoluteCounter(counter);
				}

				EnsureCategory(categoryAttr, counters);
				DoCreateCounters(instance, categoryAttr, counters);

				foreach (IICPerformanceCounter counter in counters) {
					counter.Reset();
				}
			} catch (Exception ex) {
				SystemLog.Warn(LogEventID.PerformanceCounterFailed, ex, "PerformanceCounter<{0}> Create Failed", categoryAttr.CategoryName);
			}
		}

		public static void EnsureCategory(IICPerformanceCountersAttribute categoryAttr, IEnumerable<IICPerformanceCounter> counters)
		{
			bool needCreate;
			if (!PerformanceCounterCategory.Exists(categoryAttr.CategoryName)) {
				needCreate = true;
			} else {
				needCreate = false;
				foreach (IICPerformanceCounter counter in counters) {
					if (!PerformanceCounterCategory.CounterExists(counter._rawAttr.CounterName, categoryAttr.CategoryName)) {
						needCreate = true;
						break;
					}
				}

				if (needCreate)
					PerformanceCounterCategory.Delete(categoryAttr.CategoryName);
			}

			if (needCreate)
				CreateCategory(categoryAttr, counters);
		}

		public static void CreateCategory(IICPerformanceCountersAttribute categoryAttr, IEnumerable<IICPerformanceCounter> counters)
		{
			CounterCreationDataCollection ccdc = new CounterCreationDataCollection();

			foreach (IICPerformanceCounter counter in counters) {
				CounterCreationData ccd = new CounterCreationData();
				ccd.CounterType = counter._rawAttr.CounterType;
				ccd.CounterName = counter._rawAttr.CounterName;
				ccd.CounterHelp = counter._rawAttr.CounterHelp;
				ccdc.Add(ccd);

				if (counter._baseAttr != null) {
					CounterCreationData baseCcd = new CounterCreationData();
					baseCcd.CounterType = counter._baseAttr.CounterType;
					baseCcd.CounterName = counter._baseAttr.CounterName;
					baseCcd.CounterHelp = counter._baseAttr.CounterHelp;
					ccdc.Add(baseCcd);
				}
			}

			PerformanceCounterCategory.Create(categoryAttr.CategoryName, categoryAttr.CategoryHelp, categoryAttr.CategoryType, ccdc);
		}

		public static void EvoluteCounter(IICPerformanceCounter counter)
		{
			bool setBase = true;
			PerformanceCounterType baseCounterType = PerformanceCounterType.RawBase;

			switch (counter._rawAttr.CounterType) {
				case PerformanceCounterType.AverageCount64:
				case PerformanceCounterType.AverageTimer32:
					baseCounterType = PerformanceCounterType.AverageBase;
					break;
				case PerformanceCounterType.CounterMultiTimer:
				case PerformanceCounterType.CounterMultiTimer100Ns:
				case PerformanceCounterType.CounterMultiTimer100NsInverse:
				case PerformanceCounterType.CounterMultiTimerInverse:
					baseCounterType = PerformanceCounterType.CounterMultiBase;
					break;
				case PerformanceCounterType.RawFraction:
					baseCounterType = PerformanceCounterType.RawBase;
					break;
				case PerformanceCounterType.SampleCounter:
				case PerformanceCounterType.SampleFraction:
					baseCounterType = PerformanceCounterType.SampleBase;
					break;
				default:
					setBase = false;
					break;
			}

			if (setBase) {
				counter._baseAttr = new IICPerformanceCounterAttribute(
					counter._rawAttr.CounterName + " Base",
					baseCounterType,
					counter._rawAttr.CounterHelp + " Base");
			}
		}

		public static void DoCreateCounters(string instance, IICPerformanceCountersAttribute categoryAttr, IEnumerable<IICPerformanceCounter> counters)
		{
			switch (categoryAttr.CategoryType) {
				case PerformanceCounterCategoryType.SingleInstance:
					instance = string.Empty;
					break;
				case PerformanceCounterCategoryType.MultiInstance:
					if (string.IsNullOrEmpty(instance))
						instance = Process.GetCurrentProcess().ProcessName;
					break;
				default:
					break;
			}

			foreach (IICPerformanceCounter counter in counters) {
				if (string.IsNullOrEmpty(instance)) {
					counter._counter = new PerformanceCounter(categoryAttr.CategoryName, counter._rawAttr.CounterName, false);

					if (counter._baseAttr != null)
						counter._baseCounter = new PerformanceCounter(categoryAttr.CategoryName, counter._baseAttr.CounterName, false);
				} else {
					counter._counter = new PerformanceCounter(categoryAttr.CategoryName, counter._rawAttr.CounterName, instance, false);

					if (counter._baseAttr != null)
						counter._baseCounter = new PerformanceCounter(categoryAttr.CategoryName, counter._baseAttr.CounterName, instance, false);
				}
				counter._rawAttr = null;
				counter._baseAttr = null;
			}
		}

		public static void Close()
		{
			try {
				foreach (IICPerformanceCounterCategory category in _categorys) {
					foreach (IICPerformanceCounter counter in category._counters) {
						counter.Close();
					}
				}
				PerformanceCounter.CloseSharedResources();
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.CommonFailed, ex, "PerformanceCounter Close Failed");
			}
		}

		private static List<IICPerformanceCounterCategory> _categorys = new List<IICPerformanceCounterCategory>();
	}
}
