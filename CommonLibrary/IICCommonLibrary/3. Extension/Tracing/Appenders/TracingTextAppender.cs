using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingTextAppender: IAppender
	{
		private string _path;
		private RetryProtector _protector;

		public RetryProtector Protector
		{
			get { return _protector; }
		}

		public AppenderType Type
		{
			get { return AppenderType.Text; }
		}

		public bool IsBackup
		{
			get { return true; }
		}

		public bool Enabled
		{
			get;
			set;
		}

		public TracingTextAppender(string path, bool enabled)
		{
			Enabled = enabled;
			if (!string.IsNullOrEmpty(path)) {
				_path = path.TrimEnd('\\') + "\\" + ServiceEnvironment.ServiceName;
			} else {
				path = AppDomain.CurrentDomain.BaseDirectory;
				_path = path.TrimEnd('\\') + "\\LOG";
			}

			try {
				if (!Directory.Exists(_path)) {
					Directory.CreateDirectory(_path);
				}
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.ServiceFailed, ex, "TracingTextAppeder Failed: Appender Disabled." + _path);
				Enabled = false;
			}

			_protector = new RetryProtector("TracingTextAppender", new int[] { 0, 10, 30, 300 });
		}

		public void AppendSystemLog(IEnumerable<SystemLogEvent> events)
		{
			if (Enabled) {
				DoAppendSystemLogs(0, events);
			}
		}

		public void AppendTracing(IEnumerable<TracingEvent> events)
		{
			if (Enabled) {
				DoAppendTracings(0, events);
			}
		}

		private void DoAppendTracings(int retryCount, IEnumerable<TracingEvent> events)
		{
			try {
				string path = string.Format("{0}\\LOG_{1}{2}.log",
					_path,
					DateTime.Now.ToString("yyyy-MM-dd_HH"),
					retryCount > 0 ? "(" + retryCount.ToString() + ")" : "");

				using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) {
					foreach (TracingEvent evt in events) {
						writer.WriteLine(evt.ToString());
					}
				}
			} catch (IOException) {
				if (retryCount < 3) {
					DoAppendTracings(retryCount + 1, events);
				}
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.TracingFailed, ex, "TextAppender.AppendLogs");
			}
		}

		private void DoAppendSystemLogs(int retryCount, IEnumerable<SystemLogEvent> events)
		{
			try {
				string path = string.Format("{0}\\SYSTEMLOG_{1}{2}.log",
					_path,
					DateTime.Now.ToString("yyyy-MM-dd_HH"),
					retryCount > 0 ? "(" + retryCount.ToString() + ")" : "");

				using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) {
					foreach (SystemLogEvent evt in events) {
						writer.WriteLine(evt.ToString());
					}
				}
			} catch (IOException) {
				if (retryCount < 3) {
					DoAppendSystemLogs(retryCount + 1, events);
				}
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.TracingFailed, ex, "TextAppender.AppendSystemLogs");
			}
		}
	}
}
