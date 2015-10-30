using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingDatabaseAppender: IAppender
	{
		private Database _db;
		private RetryProtector _protector;

		public RetryProtector Protector
		{
			get { return _protector; }
		}

		public AppenderType Type
		{
			get { return AppenderType.Database; }
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

		public TracingDatabaseAppender(string connStr, bool enabled)
		{
			Enabled = enabled;
			_db = DatabaseManager.GetDatabase(IICDbType.SqlServer2005, connStr);
			_protector = new RetryProtector("TracingDatabaseAppender", new int[] { 0, 10, 30, 300 });
		}

		public void AppendTracing(IEnumerable<TracingEvent> events)
		{
			if (Enabled) {
				_db.BulkInsert("CMN_ServerTrace", events);
			}
		}

		public void AppendSystemLog(IEnumerable<SystemLogEvent> events)
		{
			if (Enabled) {
				_db.BulkInsert("CMN_SystemLog", events);
			}
		}

	}
}
