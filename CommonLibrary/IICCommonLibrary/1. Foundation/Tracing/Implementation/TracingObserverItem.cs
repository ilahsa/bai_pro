using System;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingObserverItem: ObserverItem
	{
		[ObserverField(ObserverColumnType.String)]
		public string LoggerName;

		[ObserverField(ObserverColumnType.Interger)]
		public int InfoCount = 0;

		[ObserverField(ObserverColumnType.Interger)]
		public int WarnCount = 0;

		[ObserverField(ObserverColumnType.Interger)]
		public int ErrorCount = 0;

		[ObserverField(ObserverColumnType.String)]
		public string LastError = string.Empty;

		[ObserverField(ObserverColumnType.String)]
		public string LastException = string.Empty;

		public TracingObserverItem(string loggerName)
		{
			LoggerName = loggerName;
		}

		public bool Started
		{
			get
			{
				return InfoCount > 0 ||
					WarnCount > 0 ||
					ErrorCount > 0;
			}
		}

		public void Clear()
		{
			InfoCount = 0;
			WarnCount = 0;
			ErrorCount = 0;
			LastError = string.Empty;
			LastException = string.Empty;
		}
	}
}
