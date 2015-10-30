using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Observation
{
	public static class ObserverManager
	{
		private static object _syncRoot = new object();
		private static Dictionary<string, IObserver> _observers = new Dictionary<string, IObserver>();

		public static void RegisterObserver(IObserver observer)
		{
			lock (_syncRoot) {
				_observers.Add(observer.ObserverName, observer);
			}
		}

		public static void RegisterObserver(string name, Func<List<ObserverItem>> proc, Action clearProc)
		{
			lock (_syncRoot) {
				_observers.Add(name, new ObserverDelegate(name, proc, clearProc));
			}
		}

		public static List<string> EnumObserverName()
		{
			List<string> list = new List<string>();
			lock (_syncRoot) {
				foreach (var k in _observers) {
					list.Add(k.Value.ObserverName);
				}
			}
			return list;
		}

		public static ObserverDataTable Observe(string key)
		{
			IObserver observer = null;
			lock (_syncRoot) {
				if (!_observers.TryGetValue(key, out observer)) {
					throw new KeyNotFoundException("Observer Not Found:" + key);
				}
			}

			List<ObserverItem> list = observer.Observe();
			ObserverDataTable table = new ObserverDataTable();

			if (list.Count == 0) {
				table.Columns = new string[0];
				table.ColumnTypes = new ObserverColumnType[0];
				table.Rows = new List<RpcClass<string[]>>();
				table.ClearTime = DateTime.Parse("2000-01-01");
				return table;
			}

			var formatter = list[0].GetFormatter();
			table.ClearTime = observer.ClearTime;
			table.Columns = formatter.Columns;
			table.ColumnTypes = formatter.ColumnTypes;
			table.Rows = new List<RpcClass<string[]>>();

			foreach (var i in list) {
				string[] ss = formatter.GetRow(i);
				table.Rows.Add(new RpcClass<string[]>(ss));
			}
			return table;
		}

		public static void Clear(List<string> keys)
		{
			foreach (var s in keys) {
				IObserver observer;
				if (_observers.TryGetValue(s, out observer)) {
					observer.ClearObserver();
				}
			}
		}
	}
}
