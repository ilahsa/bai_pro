using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace Imps.Services.CommonV4.DbAccess
{
	public class MysqlBatchInsert: IDatabaseOperation
	{
		#region Common & Constructor
		private string _connStr;

		public int CommandTimeout
		{
			get { return -1; }
			set { throw new NotSupportedException("MysqlBatchInsert not support command timeout"); }
		}

		public MysqlBatchInsert(string connStr)
		{
			_connStr = connStr;
		}
		#endregion

		#region BatchInsert
		private const int BatchCount = 100;
		private object _syncRoot = new object();
		private Dictionary<ComboClass<Type, int>, ComboClass<string, string[], List<TableFieldAttribute>>> _bulkCaches =
			new Dictionary<ComboClass<Type, int>, ComboClass<string, string[], List<TableFieldAttribute>>>();

		public int BulkInsert<T>(string tableName, IEnumerable<T> values)
		{
			int ret = 0;
			MySqlConnection cnn = null;
			MySqlTransaction trans = null;

			try {
				cnn = GetConnection();
				trans = cnn.BeginTransaction(IsolationLevel.RepeatableRead);

				int n = 0;
				T[] batchData = new T[BatchCount];
				foreach (T value in values) {
					batchData[n] = value;
					n++;
					if (n == BatchCount) {
						ret += BatchInsert(tableName, batchData, n, cnn, trans);
						n = 0;
					}
				}
				if (n > 0) {
					ret += BatchInsert(tableName, batchData, n, cnn, trans);
				}

				trans.Commit();
			} catch (Exception ex) {
				if (trans != null)
					trans.Rollback();
				throw ex;
			} finally {
				if (cnn != null)
					cnn.Close();
			}
			return ret;
		}

		private int BatchInsert<T>(string tableName, T[] batchData, int count, MySqlConnection cnn, MySqlTransaction trans)
		{
			Type type = typeof(T);
			ComboClass<string, string[], List<TableFieldAttribute>> cache;
			ComboClass<Type, int> key = new ComboClass<Type, int>(typeof(T), count);
			lock (_syncRoot) {
				if (!_bulkCaches.TryGetValue(key, out cache)) {
					cache = BuildBulkCache(tableName, type, count);
					_bulkCaches.Add(key, cache);
				}
			}

			string sql = cache.V1;
			string[] parameters = cache.V2;
			List<TableFieldAttribute> attrs = cache.V3;

			object[] objs = new object[attrs.Count * count];
			for (int i = 0; i < count; i++) {
				for (int j = 0; j < attrs.Count; j++) {
					objs[i * attrs.Count + j] = attrs[j].Field.GetValue(batchData[i]);
				}
			}

			MySqlCommand cmd = new MySqlCommand(sql, cnn, trans);
			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;
			SpFillParameters(cmd, parameters, objs);
			return cmd.ExecuteNonQuery();			
		}


		public int BulkInsert(string tableName, DataTable table)
		{
			throw new NotImplementedException();
		}

		private ComboClass<string, string[], List<TableFieldAttribute>> BuildBulkCache(string tableName, Type t, int batchCount)
		{
			List<string> param = new List<string>();

			FieldInfo[] fields = t.GetFields();

			List<TableFieldAttribute> attrs = new List<TableFieldAttribute>();
			foreach (FieldInfo field in fields) {
				TableFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<TableFieldAttribute>(field);
				if (fieldAttr != null) {
					attrs.Add(fieldAttr);
					fieldAttr.Field = field;
				}
			}

			StringBuilder sqlAll = new StringBuilder();

			for (int i = 0; i < batchCount; i++) {
				StringBuilder sqlBefore = new StringBuilder();
				StringBuilder sqlAfter = new StringBuilder();
				StringBuilder sqlUpdate = new StringBuilder();

				sqlBefore.AppendFormat("INSERT INTO {0} (", tableName);
				for (int j = 0; j < attrs.Count; j++) {
					var attr = attrs[j];
					sqlBefore.AppendFormat("{0}", attr.ColumnName);

					int paramOrder = i * attrs.Count + j;
					sqlAfter.AppendFormat("@v_p{0}", paramOrder);
					sqlUpdate.AppendFormat("{0}=@v_p{1}", attr.ColumnName, paramOrder);
					param.Add("@p" + paramOrder);

					if (j != attrs.Count - 1) {
						sqlBefore.Append(",");
						sqlAfter.Append(",");
						sqlUpdate.Append(",");
					}
				}
				sqlAll.Append(sqlBefore.ToString());
				sqlAll.Append(") VALUES (");
				sqlAll.Append(sqlAfter.ToString());
				sqlAll.Append(") ON DUPLICATE KEY UPDATE ");
				sqlAll.Append(sqlUpdate.ToString());
				sqlAll.Append(";\r\n");
			}

			var cache = new ComboClass<string, string[], List<TableFieldAttribute>>();
			cache.V1 = sqlAll.ToString();
			cache.V2 = param.ToArray();
			cache.V3 = attrs;

			return cache;
		}

		private string FormatData(object obj, Type fieldType)
		{
			if (fieldType == typeof(byte[])) {
				return "0x" + StrUtils.ToHexString((byte[])obj);
			} else {
				return obj.ToString();
			}
		}
		#endregion

		#region Private Methods
		private MySqlConnection GetConnection()
		{
			MySqlConnection cnn = new MySqlConnection(_connStr);
			try {
				cnn.Open();
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return cnn;
		}

		private void SpFillParameters(MySqlCommand command, string[] parameters, object[] values)
		{
			if (values == null || values.Length == 0)
				return;

			for (int i = 0; i < parameters.Length; i++) {
				string parameter = parameters[i];
				object value = values[i];
				if (value != null)
					command.Parameters.AddWithValue(parameter.Insert(1, "v_"), value);
				else
					command.Parameters.AddWithValue(parameter.Insert(1, "v_"), DBNull.Value);
			}
		}
		#endregion

		#region IDatabaseOperation Methods (NotSupported)

        public IAsyncResult SpBeginExecuteNonQuery(string spName, AsyncCallback callback, object stateOjbect, string[] paramters, params object[] values) {
            throw new NotSupportedException();
        }

        public int SpEndExecuteNonQuery(IDbCommand cmd, IAsyncResult ar) {
            throw new NotSupportedException();
        }

        public void SpBeginExecuteNonQuery(string spName,  Action<AsyncDbCallback<int>> callback, string[] paramters, params object[] values) {
            throw new NotSupportedException();
        }

		public int SpExecuteNonQuery(string spName, string[] paramters, params object[] values)
		{
			throw new NotSupportedException();
		}

        public IAsyncResult SpBeginExecuteReader(string spName, AsyncCallback callback, object stateOjbect, string[] parameters, params object[] values) {
            throw new NotSupportedException();
        }

        public IDataReader SpEndExecuteReader(IDbCommand cmd, IAsyncResult ar) {
            throw new NotSupportedException();
        }

        public void SpBeginExecuteReader(string spName, Action<AsyncDbCallback<DataReader>> callback, string[] paramters, params object[] values)
        {
            throw new NotSupportedException();
        }


		public DataReader SpExecuteReader(string spName, string[] paramters, params object[] values)
		{
			throw new NotSupportedException();
		}

		public T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
		{
			throw new NotSupportedException();
		}

		public DataTable SpExecuteTable(string spName, string[] paramters, params object[] values)
		{
			throw new NotSupportedException();
		}

		public DataSet SpExecuteDataSet(string spName, string[] paramters, params object[] values)
		{
			throw new NotSupportedException();
		}

		public string FormatSql(string spName, string[] parameters, params object[] values)
		{
			throw new NotSupportedException();
		}

		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public T ExecuteScalar<T>(string sql, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public DataReader ExecuteReader(string sql, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteTable(string sql, params object[] parameters)
		{
			throw new NotImplementedException();
		}
		#endregion

        #region IDatabaseOperation Members


        public int BulkInsert(string tableName, DataTable table, string[] colNames)
        {
            throw new NotImplementedException();
        }

        public string GetDatabaseName()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}