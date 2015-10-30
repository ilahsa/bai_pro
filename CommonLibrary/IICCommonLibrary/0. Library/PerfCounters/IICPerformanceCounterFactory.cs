using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ComponentModel;

namespace Imps.Services.CommonV4
{
	public static class IICPerformanceCounterFactory
	{
		private static object _syncRoot = new object();
		private static Dictionary<ComboClass<Type, string>, object> _cachedCounters = new Dictionary<ComboClass<Type, string>, object>();

		public static T GetCounters<T>()
		{
			return GetCounters<T>(string.Empty);
		}

		public static T GetCounters<T>(string instance)
		{
			ComboClass<Type, string> key = new ComboClass<Type,string>(typeof(T), instance);
			object ret;
			lock (_syncRoot) {
				if (!_cachedCounters.TryGetValue(key, out ret)) {
					ret = CreateCounters<T>(instance);
					_cachedCounters.Add(key, ret);
				}
				return (T)ret;
			}
		}

		public static void GetCounters(IICPerformanceCounterCategory category)
		{
			GetCounters(category, string.Empty);
		}

		public static void GetCounters(IICPerformanceCounterCategory category, string instanceName)
		{
			IICPerformanceCounterMananger.CreateCounters(instanceName, category._categoryAttribute, category._counters);
		}

		private static T CreateCounters<T>(string instance) 
		{
			T result = Activator.CreateInstance<T>();

			try {
				IICPerformanceCountersAttribute categoryAttr = AttributeHelper.
					GetAttribute<IICPerformanceCountersAttribute>(typeof(T));

				List<IICPerformanceCounter> counters = new List<IICPerformanceCounter>();

				FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

				foreach (FieldInfo fieldInfo in fields) {
					IICPerformanceCounterAttribute counterAttr = AttributeHelper.
						GetAttribute<IICPerformanceCounterAttribute>(fieldInfo);

					IICPerformanceCounter perfCounter = new IICPerformanceCounter();

					perfCounter._rawAttr = counterAttr;
					counters.Add(perfCounter);

					fieldInfo.SetValue(result, perfCounter);
				}

				IICPerformanceCounterMananger.CreateCounters(instance, categoryAttr, counters);
			} catch (Exception ex) {
				SystemLog.Warn(LogEventID.PerformanceCounterFailed, ex, "PerformanceCounter<{0}>({1}) Create Failed.", typeof(T).Name, instance); 
			}
			return result;
		}
	}
}
