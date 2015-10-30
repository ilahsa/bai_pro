using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4.DbAccess
{
	class DatabaseObserverItem: PerformanceObserverItem
	{
		[ObserverField(ObserverColumnType.String)]
		public string Database;

		[ObserverField(ObserverColumnType.String)]
		public string ProcName;
	}
}
