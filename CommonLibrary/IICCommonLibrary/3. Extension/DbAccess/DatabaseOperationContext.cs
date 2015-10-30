using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
	internal class DatabaseOperationContext
	{
		public string Info;
		public string SpName;
		public string[] Parameters;
		public object[] Values;

		public Exception Ex;
		public Stopwatch Watch;
		public IDatabaseOperation Operation;
		internal DatabaseObserverItem Observer;
	}
}
