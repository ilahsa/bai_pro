using System;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4
{
	public static class DatabaseManager
	{
		private static object _syncRoot = new object();
		private static Dictionary<string, Database> _allDatabses = new Dictionary<string, Database>();

		static DatabaseManager()
		{
			ObserverManager.RegisterObserver("DbAccess", Observe, ClearObserver);
		}

		public static Database GetDatabase(IICDbType dbType, string connString)
        {
			return new Database(dbType, connString);
	    }

		public static Database GetDatabase(ResolvableUri uri)
		{
			return GetDatabase(uri.Resolve(RouteMethod.Database));
		}

		public static Database GetDatabase(ServerUri uri)
		{
			throw new NotImplementedException();
		}

        public static Database GetDatabase(string dbName)
        {
            return GetDatabase(dbName, 0);
        }

        public static Database GetDatabase(string dbName, int physicalPool)
        {
            string key = dbName + (physicalPool == 0 ? "" : "." + physicalPool.ToString());

			Database ret;
			lock (_syncRoot) {
				if (!_allDatabses.TryGetValue(key, out ret)) {
					ret = new Database(key);
					_allDatabses.Add(key, ret);
				}
			}
            return ret;
		}

		#region Observation
		private static void ClearObserver()
		{
			List<Database> dbs = new List<Database>();

			lock (_syncRoot) {
				foreach (var db in _allDatabses.Values) {
					dbs.Add(db);
				}
			}

			foreach (var db in dbs) {
				db.ClearObserver();
			}
		}

		private static List<ObserverItem> Observe()
		{
			List<Database> dbs = new List<Database>();

			lock (_syncRoot) {
				foreach (var db in _allDatabses.Values) {
					dbs.Add(db);
				}
			}

			List<ObserverItem> ret = new List<ObserverItem>();
			foreach (var db in dbs) {
				foreach (var a in db.Observe()) {
					ret.Add(a);
				}
			}

			return ret;
		}
		#endregion
	}
}
