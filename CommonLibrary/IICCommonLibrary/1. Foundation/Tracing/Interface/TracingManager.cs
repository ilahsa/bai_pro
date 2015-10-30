using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Imps.Services.CommonV4.Tracing;
using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		TracingManager
	/// </summary>
	public static class TracingManager
	{
		#region Private Fields
		private static object _syncRoot = new object();
		private static TracingLevel _level;
		private static LazyQueue<TracingEvent> _queueTracing;
		private static LazyQueue<SystemLogEvent> _queueLog;
		private static TracingConfigSection _configSection = null;
		private static Dictionary<string, TracingImpl> _loggers = new Dictionary<string,TracingImpl>();
		private static IAppender _backupAppender = null;
		private static List<IAppender> _appenders = new List<IAppender>();
		private static List<TracingSniffer> _sniffers = new List<TracingSniffer>();
		private static TracingPerfCounters _counters = IICPerformanceCounterFactory.GetCounters<TracingPerfCounters>();
		#endregion

		#region Static Constructor
		static TracingManager()
		{
			_queueTracing = new LazyQueue<TracingEvent>("TracingManager.QueueTracing", 32, 50, TracingDequeueAction);
			_queueLog = new LazyQueue<SystemLogEvent>("TracingManager.QueueLog", 32, 50, SystemLogDequeueAction);
			_configSection = IICConfigSection.CreateDefault<TracingConfigSection>();
			_level = TracingLevel.Off;
			ReloadConfiguration();

			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
			//Trace.Listeners.Add(new DebugTraceListener());

			ObserverManager.RegisterObserver("Tracing", ObserveProc, ClearObserver);
		}
		#endregion

		#region Public Methods
		public static void Initialize()
		{
		}

		public static void UpdateConfig(TracingConfigSection section)
		{
			_configSection = section;
			ReloadConfiguration();
		}

		public static void UpdateConfig()
		{
			_configSection = IICConfigurationManager.Configurator.GetConfigSecion<TracingConfigSection>(
				"IICTracing",
				delegate(TracingConfigSection section) {
					_configSection = section;
					ReloadConfiguration();
				}
			);
		}

		public static TracingLevel Level
		{
			get { return _configSection.Level; }
			set { _configSection.Level = value; }
		}

		public static ITracing GetTracing(Type type)
		{
			string name = ObjectHelper.GetTypeName(type, true);
			return GetTracing(name);
		}

		public static ITracing GetTracing(string loggerName)
		{
			TracingImpl impl;
			lock (_syncRoot) {
				if (!_loggers.TryGetValue(loggerName, out impl)) {
					impl = new TracingImpl(loggerName);
					_loggers.Add(loggerName, impl);
				}
			}
			return impl;
		}

		public static void AddSniffer(string url, string[] loggerPrefix, TracingLevel level, string from, string to)
		{
			RemoveSniffer(url);
			if (_sniffers.Count >= TracingSniffer.MaxSniffer) {
				throw new NotSupportedException("Over Max Sniffer");
			}

			TracingSniffer newSniffer = new TracingSniffer(url, level, from, to);
			_sniffers.Add(newSniffer);

			lock (_syncRoot) {
				foreach (var k in _loggers) {
					foreach (string pre in loggerPrefix) {
						if (pre == "*" || k.Value.LoggerName.StartsWith(pre)) {
							k.Value.AddSniffer(newSniffer);
						}
					}
				}
			}
		}

		public static void RemoveSniffer(string url)
		{
			lock (_syncRoot) {
				TracingSniffer delSniffer = null;
				foreach (TracingSniffer sniffer in _sniffers) {
					if (sniffer.Url == url) {
						delSniffer = sniffer;
						break;
					}
				}

				if (delSniffer != null) {
					foreach (var k in _loggers) {
						k.Value.RemoveSniffer(delSniffer);
					}
					_sniffers.Remove(delSniffer);
				}
			}
		}

		public static void FlushCache()
		{
			_queueTracing.FlushCache();
		}
		#endregion

		#region Private or Internal Methods
		internal static void Enqueue(TracingEvent evt)
		{
			_queueTracing.Enqueue(evt);
		}

		internal static void Enqueue(SystemLogEvent evt)
		{
			_queueLog.Enqueue(evt);
		}

		private static void TracingDequeueAction(TracingEvent[] evts)
		{
			bool writebak = true;
			foreach (IAppender appender in _appenders) {
				try {
					if (appender == _backupAppender)
						writebak = false;
					
					if (!appender.Protector.Failing) {
						appender.AppendTracing(evts);
						writebak = false;
					}
				} catch (Exception ex) {
					appender.Protector.OnException(ex);
					SystemLog.Error(LogEventID.TracingFailed, ex, 
						"Tracing Appender<{0}> Write Tracing Failed!", appender.Type);
				}
			}

			if (writebak && _backupAppender != null) {
				try {
					_backupAppender.AppendTracing(evts);
				} catch (Exception ex) {
					SystemLog.Error(LogEventID.TracingFailed, ex, "Tracing Appender<{0}> Write Tracing Failed!", _backupAppender.Type);
				}
			}
		}

		private static void SystemLogDequeueAction(SystemLogEvent[] evts)
		{
			foreach (IAppender appender in _appenders) {
				try {
					if (!appender.Protector.Failing) {
						appender.AppendSystemLog(evts);
					}
				} catch (Exception ex) {
					appender.Protector.OnException(ex);
					_backupAppender.AppendSystemLog(evts);
					SystemLog.Error(LogEventID.TracingFailed, ex,
						"Tracing Appender<{0}> Write SystemLog Failed!", appender.Type);
				}
			}
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try {
				SystemLog.Error(LogEventID.Unexcepted, "Unexcepted Exception \r\n{0}", e.ExceptionObject);
			} catch (Exception ex) {
				SystemLog.Unexcepted(ex);
			}
		}

		private static void ReloadConfiguration()
		{
			_level = _configSection.Level;
			List<IAppender> appenders = new List<IAppender>();

			foreach (TracingConfigAppenderItem item in _configSection.Appenders.Values) {
				switch (item.Type) {
					case AppenderType.Console:
						appenders.Add(new TracingConsoleAppender(item.Enabled));
						break;
					case AppenderType.Database:
						appenders.Add(new TracingDatabaseAppender(item.Path, item.Enabled));
						break;
					case AppenderType.Text:
						var a = new TracingTextAppender(item.Path, item.Enabled);
						appenders.Add(a);
						_backupAppender = a;
						break;
				}
			}
			_appenders = appenders;
		}

		private static List<ObserverItem> ObserveProc()
		{
			List<ObserverItem> ret = new List<ObserverItem>();
			lock (_syncRoot) {
				foreach (var k in _loggers) {
					var i = k.Value.ObserverItem;
					if (i.Started) {
						ret.Add(i);
					}
				}
			}
			return ret;
		}

		private static void ClearObserver()
		{
			lock (_syncRoot) {
				foreach (var k in _loggers) {
					k.Value.ObserverItem.Clear();
				}
			}
		}
		#endregion

		#region Callback log Action
		public static void Info(Action action)
		{
			bool sniffered;
			if (CanLog(TracingLevel.Info, out sniffered))
				Do(action);
		}

		public static void Warn(Action action)
		{
			bool sniffered;
			if (CanLog(TracingLevel.Warn, out sniffered))
				Do(action);
		}

		public static void Error(Action action)
		{
			bool sniffered;
			if (CanLog(TracingLevel.Error, out sniffered))
				Do(action);
		}

		private static void Do(Action action)
		{
			try {
				action();
			} catch (Exception ex) {
				SystemLog.Unexcepted(ex);
			}
		}

		private static bool CanLog(TracingLevel level, out bool sniffered) 
		{
			sniffered = false;
			return level >= Level;
		}
		#endregion

		#region NextVersion
		// 
		// Next Version
		//public static bool AntiRepeat
		//{
		//    get { return _configSection.AntiRepeat; }
		//    set { _configSection.AntiRepeat = value; }
		//}
		#endregion
	}
}