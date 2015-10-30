using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Tracing;

namespace Imps.Services.CommonV4
{
	public enum LogEventID
	{
		ServiceStart				= 1001,
		ServiceStop					= 1002,
		PerformanceCounterFailed	= 2001,
		HAMasterMonitorWarning		= 2100,
		ServiceFailed				= 3001,
		ConfigFailed				= 3002,
		CommonFailed				= 3003,
		NetworkFailed				= 3004,
		RpcFailed					= 3005,
		DatabaseFailed				= 3006,
		TracingFailed				= 3007,
		ServerInnerFailed			= 3008,
		SecuirtyQuotaService		= 3101,
		ConcurrentTaskFailed		= 3102,
		QualityMonitorService		= 3103,
		HACenterFailed				= 3200,
		HAWorkerMonitorFailed		= 3201,
		Unexcepted					= 3999,
	}

	public static class SystemLog
	{
		public static void Info(LogEventID eventId, string message)
		{
			WriteEventLog(EventLogEntryType.Information, message, eventId);
		}

		public static void Info(LogEventID eventId, string format, params object[] args)
		{
			string message = TracingHelper.FormatMessage(format, args);
			WriteEventLog(EventLogEntryType.Information, message, eventId);
		}

		public static void Warn(LogEventID eventId, string message)
		{
			WriteEventLog(EventLogEntryType.Warning, message, eventId);
		}

		public static void Warn(LogEventID eventId, string format, params object[] args)
		{
			string message = TracingHelper.FormatMessage(format, args);
			WriteEventLog(EventLogEntryType.Warning, message, eventId);
		}

		public static void Warn(LogEventID eventId, Exception ex, string message)
		{
			string message2 = message + "\r\n" + ex.ToString();
			WriteEventLog(EventLogEntryType.Warning, message2, eventId);
		}

		public static void Warn(LogEventID eventId, Exception ex, string format, params object[] args)
		{
			string message = TracingHelper.FormatMessage(format, args) + "\r\n" + ex.ToString();
			WriteEventLog(EventLogEntryType.Warning, message, eventId);
		}

		public static void Error(LogEventID eventId, string message)
		{
			WriteEventLog(EventLogEntryType.Error, message, eventId);
		}

		public static void Error(LogEventID eventId, string format, params object[] args)
		{
			string message = TracingHelper.FormatMessage(format, args);
			WriteEventLog(EventLogEntryType.Error, message, eventId);
		}

		public static void Error(LogEventID eventId, Exception ex, string message)
		{
			string message2 = message + "\r\n" + ex.ToString();
			WriteEventLog(EventLogEntryType.Error, message2, eventId);
		}

		public static void Error(LogEventID eventId, Exception ex, string format, params object[] args)
		{
			string message = TracingHelper.FormatMessage(format, args) + "\r\n" + ex.ToString();
			WriteEventLog(EventLogEntryType.Error, message, eventId);
		}

		public static void Unexcepted(Exception ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		private static void WriteEventLog(EventLogEntryType entryType, string message, LogEventID eventId)
		{
			try {
				if (_eventSource == null) {
					lock (_syncRoot) {
						if (_eventSource == null) {
							try {
								_eventSource = ServiceEnvironment.ServiceName + "-Service";
							} catch (Exception) {
								_eventSource = null;
							}
							if (string.IsNullOrEmpty(_eventSource))
								_eventSource = Process.GetCurrentProcess().ProcessName;

							EnsureEventSource(_eventSource);
						}
					}
				}

				WriteEventLogInner(entryType, message, eventId, 1);
				//
				// NextVersion
				//int repeat;
				//if (TracingManager.AntiRepeat) {
				//    if (!AntiRepeater.IsRepeated(out repeat))
				//		WriteEventLogInner(entryType, message, eventId, repeat);
				//} else {
				//	WriteEventLogInner(entryType, message, eventId, 1);
				//}
			} catch (Exception ex) {
				Unexcepted(ex);
			}
		}

		private static void WriteEventLogInner(EventLogEntryType entryType, string message, LogEventID eventId, int repeat)
		{
			// NextVersion
			//if (repeat != 1) {
			//    message = string.Format("!!!Repeat ({0}) times within last 5 second.\r\n", repeat, message);
			//}
			EventLog.WriteEntry(_eventSource, message, entryType, (int)eventId);

			SystemLogEvent evt = new SystemLogEvent();
			evt.ComputerName = ServiceEnvironment.ComputerName;
			evt.ServiceName = ServiceEnvironment.ServiceName;
			evt.Time = DateTime.Now;
			evt.Message = message;
			evt.Level = GetTracingLevel(entryType);
			evt.EventId = eventId;
			evt.Repeat = repeat;

			TracingManager.Enqueue(evt);
		}

		private static TracingLevel GetTracingLevel(EventLogEntryType entryType)
		{
			switch (entryType) {
				case EventLogEntryType.Error: return TracingLevel.Error;
				case EventLogEntryType.Warning: return TracingLevel.Warn;
				case EventLogEntryType.Information: return TracingLevel.Info;
				default: return TracingLevel.Error;
			}
		}

		public static void EnsureEventSource(string eventSource)
		{
			if (!EventLog.SourceExists(eventSource)) {
				EventLog.CreateEventSource(eventSource, "Application");
			} else {
				if (EventLog.LogNameFromSourceName(eventSource, ".") != "Application") {
					EventLog.DeleteEventSource(eventSource);
					EventLog.CreateEventSource(eventSource, "Application");
				}
			}
		}

		public static void DeleteEventSource(string eventSource)
		{
			if (!EventLog.SourceExists(eventSource)) {
				EventLog.CreateEventSource(eventSource, "Application");
			} else {
				if (EventLog.LogNameFromSourceName(eventSource, ".") != "Application") {
					EventLog.DeleteEventSource(eventSource);
					EventLog.CreateEventSource(eventSource, "Application");
				}
			}
		}

		private static string _eventSource = null;
		private static object _syncRoot = new object();
	}
}