using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	public enum AppenderType
	{
		Console,
		Text,
		Database,
	}

	public interface IAppender
	{
		AppenderType Type { get; }

		RetryProtector Protector { get; }

		bool Enabled { get; set; }

		bool IsBackup { get; }

		void AppendTracing(IEnumerable<TracingEvent> events);

		void AppendSystemLog(IEnumerable<SystemLogEvent> events);
	}
}
