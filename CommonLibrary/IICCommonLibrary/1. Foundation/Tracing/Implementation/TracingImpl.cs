using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingImpl: ITracing
	{
		#region Static Private Fields
		private static TracingPerfCounters _counters = IICPerformanceCounterFactory.GetCounters<TracingPerfCounters>();
		#endregion

		#region Private Fields
		private string _loggerName;
		private TracingObserverItem _observerItem;
		private TracingSniffer[] _sniffers;
		#endregion

		#region Public Methods & Constructor
		public TracingImpl(string loggerName)
		{
			_loggerName = loggerName;
			_observerItem = new TracingObserverItem(loggerName);
		}

		public void AddSniffer(TracingSniffer sniffer)
		{
			if (_sniffers == null) {
				_sniffers = new TracingSniffer[TracingSniffer.MaxSniffer];
			}
			for (int i = 0; i < TracingSniffer.MaxSniffer; i++) {
				if (_sniffers[i] == null) {
					_sniffers[i] = sniffer;
					break;
				}
			}
		}

		public void RemoveSniffer(TracingSniffer sniffer)
		{
			if (_sniffers == null)
				return;

			for (int i = 0; i < TracingSniffer.MaxSniffer; i++) {
				if (_sniffers[i] == sniffer) {
					_sniffers[i] = null;
					break;
				}
			}
		}

		public string LoggerName
		{
			get { return _loggerName; }
		}

		public TracingObserverItem ObserverItem
		{
			get { return _observerItem; }
		}
		#endregion

		#region ITracing Members
		public void Info(string message)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Info, null, string.Empty, string.Empty, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void Info(string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Info, null, from, to, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void Info(Exception exception, string message)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Info, exception, string.Empty, string.Empty, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void Info(Exception exception, string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Info, exception, from, to, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void InfoFmt(string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Info, null, string.Empty, string.Empty, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void InfoFmt(Exception exception, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Info, exception, string.Empty, string.Empty, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void InfoFmt2(string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, from, to, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Info, null, from, to, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}

		public void InfoFmt2(Exception exception, string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.InfoCount);
			int sniffMask;
			if (CanLog(TracingLevel.Info, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Info, exception, from, to, message);
				_counters.InfoPerSec.Increment();
				_counters.InfoTotal.Increment();
			}
		}


		public void Warn(string message)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Warn, null, string.Empty, string.Empty, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void Warn(string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Warn, null, from, to, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void Warn(Exception exception, string message)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Warn, exception, string.Empty, string.Empty, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void Warn(Exception exception, string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Warn, exception, from, to, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void WarnFmt(string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Warn, null, string.Empty, string.Empty, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void WarnFmt(Exception exception, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Warn, exception, string.Empty, string.Empty, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void WarnFmt2(string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, from, to, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Warn, null, from, to, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void WarnFmt2(Exception exception, string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.WarnCount);
			int sniffMask;
			if (CanLog(TracingLevel.Warn, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Warn, exception, from, to, message);
				_counters.WarnPerSec.Increment();
				_counters.WarnTotal.Increment();
			}
		}

		public void Error(string message)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Error, null, string.Empty, string.Empty, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void Error(string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Error, null, from, to, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void Error(Exception exception, string message)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Error, exception, string.Empty, string.Empty, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void Error(Exception exception, string from, string to, string message)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, from, to, out sniffMask)) {
				WriteLog(sniffMask, TracingLevel.Error, exception, from, to, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void ErrorFmt(string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Error, null, string.Empty, string.Empty, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void ErrorFmt(Exception exception, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Error, exception, string.Empty, string.Empty, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void ErrorFmt2(string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, from, to, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Error, null, from, to, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}

		public void ErrorFmt2(Exception exception, string from, string to, string format, params object[] args)
		{
			Interlocked.Increment(ref _observerItem.ErrorCount);
			int sniffMask;
			if (CanLog(TracingLevel.Error, out sniffMask)) {
				string message = TracingHelper.FormatMessage(format, args);
				WriteLog(sniffMask, TracingLevel.Error, exception, from, to, message);
				_counters.ErrorPerSec.Increment();
				_counters.ErrorTotal.Increment();
			}
		}
		#endregion

		#region Callback Log Methods
 		public void Info(Action action)
		{
			int sniffered;
			if (CanLog(TracingLevel.Info, out sniffered))
				Do(action);
		}

		public void Warn(Action action)
		{
			int sniffered;
			if (CanLog(TracingLevel.Warn, out sniffered))
				Do(action);
		}

		public void Error(Action action)
		{
			int sniffered;
			if (CanLog(TracingLevel.Error, out sniffered))
				Do(action);
		}

		private void Do(Action action)
		{
			try {
				action();
			} catch (Exception ex) {
				SystemLog.Unexcepted(ex);
			}
		}
		#endregion

		#region Private Fields
		private bool CanLog(TracingLevel level, out int sniffMask)
		{
			return CanLog(level, "", "", out sniffMask);
		}

		private bool CanLog(TracingLevel level, string from, string to, out int sniffMask)
		{
			sniffMask = 0;
			if (_sniffers != null) {
				try {
					for (int i = 0; i < TracingSniffer.MaxSniffer; i++) {
						TracingSniffer sniffer = _sniffers[i];
						if (sniffer != null) {
							if (sniffer.CanSniff(level, from, to))
								sniffMask |= (1 << i);
						}
					}
				} catch (Exception ex) {
					SystemLog.Unexcepted(ex);
				}
			}

			return (level >= TracingManager.Level) || (sniffMask > 0);
		}

		private void WriteLog(int sniffMask, TracingLevel level, Exception ex, string from, string to, string message)
		{
			// NextVersion
			//int repeat;
			//if (TracingManager.AntiRepeat && !sniffed) {
			//    if (AntiRepeater.IsRepeated(out repeat)) {
			//        return;
			//    }
			//} else {
			//    repeat = 1;
			//}

			//if (repeat > 1) {
			//    message = string.Format("!!!Repeat {0} times in last 5 seconds\r\n{1}", repeat, message);
			//}

			TracingEvent evt = new TracingEvent();
			evt.Level = level;
			evt.LoggerName = _loggerName;
			evt.Time = DateTime.Now;
			evt.Message = message;
			evt.ProcessInfo = ServiceEnvironment.ProcessInfo;
			evt.ServiceName = ServiceEnvironment.ServiceName;
			evt.ComputerName = ServiceEnvironment.ComputerName;
			evt.ThreadInfo = TracingHelper.FormatThreadInfo(Thread.CurrentThread);
			evt.From = from ?? "";
			evt.To = to ?? "";
			evt.Error = ex == null ? "" : ex.ToString();
			evt.Repeat = 1; // repeat;

			if (sniffMask > 0) {
				for (int i = 0; i < TracingSniffer.MaxSniffer; i++) {
					if ((sniffMask & (1 << i)) > 0) {
						TracingSniffer sniffer = _sniffers[i];
						if (sniffer != null)
							sniffer.Enqueue(evt);
					}
				}
				
			}
			if (evt.Level == TracingLevel.Error) {
				_observerItem.LastError = evt.Message;
				if (ex != null)
					_observerItem.LastException = evt.Error;

			}
			TracingManager.Enqueue(evt);
		}
		#endregion
	}
}
