using System;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;

using Imps.Services.CommonV4.Observation;
using Imps.Services.CommonV4.DbAccess;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		用于访问数据库的代理类
	/// </summary>
	public sealed class Database
	{
		#region Const Fields
		private const int SlowSqlTimeout = 5000;
		#endregion

		#region Private Fields
		private string _configKey;
		private object _syncRoot = new object();
		private object _observerSyncRoot = new object();

		private IICDbConfigItem _configItem = null;
		private IDatabaseOperation _operation = null;
		private Dictionary<string, DatabaseObserverItem> _observers = new Dictionary<string, DatabaseObserverItem>();

		private DatabasePerfCounters _perfCounters = null;
		private ITracing _tracing;
		#endregion

		#region Database Interface
		public void SpBeginExecuteNonQuery(string spName, Action<AsyncDbContext<int>> callback, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpBeginExecuteNonQuery", spName, parameters, values);

			try {
				AsyncDbContext<int> context = new AsyncDbContext<int>();
				context.Database = this;
				context.OperationContext = ctx;

				ctx.Operation.SpBeginExecuteNonQuery(
					spName,
					delegate(AsyncDbCallback<int> dbCallback) {
						context.dbCallBack = dbCallback;
						callback(context);
					}, parameters, values);
			} catch (System.Exception ex) {
				throw ex;
			}
		}

		public int SpExecuteNonQuery(string spName, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpExecuteNonQuery", spName, parameters, values);

			try {
				return ctx.Operation.SpExecuteNonQuery(spName, parameters, values);
			} catch (Exception ex) {
				OnException(ctx, ex);
				throw;
			} finally {
				OnFinally(ctx);
			}
		}

		public T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
		{
			string info = string.Format("SpExecuteScalar<0>", ObjectHelper.GetTypeName(typeof(T), false));
			var ctx = Prepare(info, spName, parameters, values);

			try {
				return ctx.Operation.SpExecuteScalar<T>(spName, parameters, values);
			} catch (Exception ex) {
				OnException(ctx, ex);
				throw;
			} finally {
				OnFinally(ctx);
			}
		}


		public void SpBeginExecuteReader(string spName, Action<AsyncDbContext<DataReader>> callback, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpBeginExecuteReader", spName, parameters, values);

			try {
				AsyncDbContext<DataReader> context = new AsyncDbContext<DataReader>();
				context.Database = this;
				context.OperationContext = ctx;
				GetInnerOperation().SpBeginExecuteReader(
					spName,
					delegate(AsyncDbCallback<DataReader> dbCallback) {
						context.dbCallBack = dbCallback;
						callback(context);
					},
					parameters,
					values);
			} catch (System.Exception ex) {
				throw ex;
			}
		}

		public DataReader SpExecuteReader(string spName, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpExecuteReader", spName, parameters, values);
			try {
				return GetInnerOperation().SpExecuteReader(spName, parameters, values);
			} catch (Exception ex) {
				OnException(ctx, ex);
				throw;
			} finally {
				OnFinally(ctx);
			}
		}

		public DataTable SpExecuteTable(string spName, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpExecuteTable", spName, parameters, values);
			try {
				return GetInnerOperation().SpExecuteTable(spName, parameters, values);
			} catch (Exception ex) {
				OnException(ctx, ex);
				throw;
			} finally {
				OnFinally(ctx);
			}
		}

		public DataSet SpExecuteDataSet(string spName, string[] parameters, params object[] values)
		{
			var ctx = Prepare("SpExecuteDataSet", spName, parameters, values);

			try {
				return GetInnerOperation().SpExecuteDataSet(spName, parameters, values);
			} catch (Exception ex) {
				OnException(ctx, ex);
				throw;
			} finally {
				OnFinally(ctx);
			}
		}

		public int BulkInsert<T>(string destTableName, IEnumerable<T> values)
		{
			Stopwatch watch = Stopwatch.StartNew();

			try {
				_perfCounters.BulkInsertRowsPerSec.Increment();
				return GetInnerOperation().BulkInsert<T>(destTableName, values);
			} catch (Exception) {
				throw;
			} finally {
				_perfCounters.BulkInsertAvgElapseMs.IncrementBy(watch.ElapsedMilliseconds);
			}
		}

		public int BulkInsert(string destTableName, DataTable table)
		{
			Stopwatch watch = Stopwatch.StartNew();

			try {
				_perfCounters.BulkInsertRowsPerSec.Increment();
				return GetInnerOperation().BulkInsert(destTableName, table);
			} catch (Exception) {
				throw;
			} finally {
				_perfCounters.BulkInsertAvgElapseMs.IncrementBy(watch.ElapsedMilliseconds);
			}
		}

		public int BulkInsert(string destTableName, DataTable table, string[] colNames)
		{
			Stopwatch watch = Stopwatch.StartNew();

			try {
				_perfCounters.BulkInsertRowsPerSec.Increment();
				return GetInnerOperation().BulkInsert(destTableName, table, colNames);
			} catch (Exception) {
				throw;
			} finally {
				_perfCounters.BulkInsertAvgElapseMs.IncrementBy(watch.ElapsedMilliseconds);
			}
		}

		public string GetDatabaseName()
		{
			Stopwatch watch = Stopwatch.StartNew();
			try {
				return GetInnerOperation().GetDatabaseName();
			} catch (Exception) {
				throw;
			}
		}

		public IDatabaseOperation OperationOnlyForUnitTest_NOT_FOR_NORMAL_SERVICE
		{
			get { return GetInnerOperation(); }
		}
		#endregion

		#region Private Methods


		private DatabaseOperationContext Prepare(string info, string spName, string[] parameters, object[] values)
		{
			DatabaseOperationContext ctx = new DatabaseOperationContext();
			ctx.Info = info;
			ctx.SpName = spName;
			ctx.Parameters = parameters;
			ctx.Values = values;
			ctx.Ex = null;
			ctx.Watch = Stopwatch.StartNew();

			if (parameters != null && values != null && parameters.Length != values.Length) {
				string msg = string.Format("{0} Parameters({1}) != Values({2})", spName, parameters.Length, values.Length);
				throw new InvalidOperationException(msg);
			}

			ctx.Operation = GetInnerOperation();
			TracingManager.Info(
				delegate() {
					_tracing.Info(info + ":\r\n" + ctx.Operation.FormatSql(spName, parameters, values));
				}
			);

			ctx.Observer = GetObserverItem(spName);

			_perfCounters.CommandExecutedTotal.Increment();
			_perfCounters.CommandExecutedPerSec.Increment();
			return ctx;
		}

		internal void OnException(DatabaseOperationContext ctx, Exception ex)
		{
			ctx.Ex = ex;
			_perfCounters.CommandFailedTotal.Increment();

			_tracing.ErrorFmt(ex, "{0} Failed in {1} \r\n{2}",
				ctx.Info,
				_configItem.Name,
				ctx.Operation.FormatSql(ctx.SpName, ctx.Parameters, ctx.Values));
		}

		internal void OnFinally(DatabaseOperationContext ctx)
		{
			int elapseMs = Convert.ToInt32(ctx.Watch.ElapsedMilliseconds);

			ctx.Observer.Track(ctx.Ex == null, ctx.Ex, ctx.Watch.ElapsedTicks);
			if (elapseMs > SlowSqlTimeout) {
				_tracing.ErrorFmt("{0} SlowSql({1}) in {2} cost {3}ms",
					ctx.Info,
					ctx.Operation.FormatSql(ctx.SpName, ctx.Parameters, ctx.Values),
					_configItem.Name,
					elapseMs);
			}
			_perfCounters.AvgExecuteMs.IncrementBy(elapseMs);
		}
		#endregion

		#region Internal Methods
		internal Database(string configKey)
		{
			_configKey = configKey;
			_tracing = TracingManager.GetTracing("Database." + configKey);
			_perfCounters = IICPerformanceCounterFactory.GetCounters<DatabasePerfCounters>(configKey);

			try {
				GetInnerOperation();
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.DatabaseFailed, ex, "Database GetFailed <{0}>", configKey);
			}
		}

		internal Database(IICDbType dbType, string connectionString)
		{
			_configItem = new IICDbConfigItem();
			_configItem.DbType = dbType;
			_configItem.ConnectionString = connectionString;

			_tracing = TracingManager.GetTracing("Database.[ConnStr=" + connectionString + "]");
			_perfCounters = IICPerformanceCounterFactory.GetCounters<DatabasePerfCounters>(".");

			switch (_configItem.DbType) {
				case IICDbType.SqlServer2005:
					_operation = new SqlServerDatabase(_configItem.ConnectionString, 120);
					break;
				case IICDbType.Mysql:
					_operation = new MysqlDatabase(_configItem.ConnectionString, 120);
					break;
				case IICDbType.MysqlBatchInsert:
					_operation = new MysqlBatchInsert(_configItem.ConnectionString);
					break;
				default:
					throw new NotSupportedException(string.Format("Not Support DbType:{0} {1}", _configItem.DbType, _configItem.Name));
			}
		}

		private IDatabaseOperation GetInnerOperation()
		{
			if (_configItem == null) {
				lock (_syncRoot) {
					if (_configItem == null) {
						_configItem = IICConfigurationManager.Configurator.GetConfigItem<IICDbConfigItem>(
							"Database",
							_configKey,
							delegate(IICDbConfigItem item) {
								_configItem = item;
							}
						);
						AssignInnerDb();
					}
				}
			}
			return _operation;
		}

		private void AssignInnerDb()
		{
			switch (_configItem.DbType) {
				case IICDbType.SqlServer2005:
					_operation = new SqlServerDatabase(_configItem.ConnectionString, _configItem.CommandTimeout);
					break;
				case IICDbType.Mysql:
					_operation = new MysqlDatabase(_configItem.ConnectionString, _configItem.CommandTimeout);
					break;
				case IICDbType.MysqlBatchInsert:
					_operation = new MysqlBatchInsert(_configItem.ConnectionString);
					break;
				default:
					throw new NotSupportedException(string.Format("Not Support DbType:{0} {1}", _configItem.DbType, _configItem.Name));
			}
		}
		#endregion

		#region Observation
		private DatabaseObserverItem GetObserverItem(string spName)
		{
			DatabaseObserverItem item = null;
			lock (_observerSyncRoot) {
				if (!_observers.TryGetValue(spName, out item)) {
					item = new DatabaseObserverItem();
					item.Database = _configKey;
					item.ProcName = spName;
					_observers.Add(spName, item);
				}
			}
			return item;
		}

		internal List<ObserverItem> Observe()
		{
			List<ObserverItem> ret = new List<ObserverItem>();
			lock (_observerSyncRoot) {
				foreach (var k in _observers) {
					ret.Add(k.Value);
				}
			}
			return ret;
		}

		internal void ClearObserver()
		{
			lock (_observerSyncRoot) {
				_observers.Clear();
			}
		}
		#endregion
	}
}