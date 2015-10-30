using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingConsoleAppender: IAppender, ITracingConsole
	{
		private object _syncRoot = new object();
		private RetryProtector _protector;

		public RetryProtector Protector
		{
			get { return _protector; }
		}

		public AppenderType Type
		{
			get { return AppenderType.Console; } 
		}

		public bool Enabled
		{
			get;
			set;
		}

		public bool IsBackup
		{
			get { return false; }
		}

		public TracingConsoleAppender(bool enabled)
		{
			Enabled = enabled;
			_protector = new RetryProtector("TracingConsoleAppender", new int[] {0, 10, 30, 300});
		}

		public void AppendTracing(IEnumerable<TracingEvent> events)
		{
			if (Enabled) {
				lock (_syncRoot) {
					foreach (TracingEvent evt in events) {
						evt.WriteToConsole(this);
					}
				}
			}
		}

		public void AppendSystemLog(IEnumerable<SystemLogEvent> events)
		{
			if (Enabled) {
				lock (_syncRoot) {
					foreach (SystemLogEvent evt in events) {
						evt.WriteToConsole(this);
					}
				}
			}
		}

		public void AppendText(string text)
		{
			Console.Write(text);
		}

		public void ResetColor()
		{
			Console.ResetColor();
		}

		public ConsoleColor TextColor
		{
			get { return Console.ForegroundColor; }
			set { Console.ForegroundColor = value; }
		}

		public ConsoleColor BackgroundColor
		{
			get { return Console.BackgroundColor; }
			set { Console.BackgroundColor = value; }
		}
	}
}
