using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
	public class SqlServerDatabase: IDatabaseOperation
	{
		#region Private Fields
		private int _commandTimeout;
		private string _connStr;
		#endregion

		#region
		public int CommandTimeout
		{
			get { return _commandTimeout; }
			set { _commandTimeout = value; }
		}
		#endregion

		#region Constructors
		public SqlServerDatabase(string connStr): this(connStr, -1)
		{
		}

		public SqlServerDatabase(string connStr, int commandTimeout)
		{
			_connStr = connStr;
			_commandTimeout = commandTimeout;
		}
		#endregion

		#region DatabaseOperation Methods
        public void SpBeginExecuteNonQuery(string spName, Action<AsyncDbCallback<int>> callback, string[] parameters, params object[] values) {
            if (callback == null)
                throw new ArgumentNullException(string.Format("Exec {0} Error! Callback action can not be null!!", spName));

            SqlConnection cnn = GetConnection();
            SqlCommand cmd = new SqlCommand(spName, cnn);

            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandType = CommandType.StoredProcedure;

            try {
                SpFillParameters(cmd, parameters, values);

                AsyncDbCallback<int> context = new AsyncDbCallback<int>();
                context.SqlCommand = cmd;
                context.SqlConnection = cnn;
                context.CallbackType = CallbackType.NonQuery;
                context.DbType = IICDbType.SqlServer2005;
                IAsyncResult a = null;
                a = cmd.BeginExecuteNonQuery(
                    new AsyncCallback(delegate(IAsyncResult ar) {
                        AsyncDbCallback<int> ctx = ar.AsyncState as AsyncDbCallback<int>;
                        ctx.ar = a;
                        callback(ctx);
                    }), 
                    context);
            } catch (System.Exception ex)  {
                cnn.Close();
                throw ex;
            }
        }

		public int SpExecuteNonQuery(string spName, string[] parameters, params object[] values)
		{
			int result = 0;
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			try {
                SpFillParameters(cmd, parameters, values);

				result = cmd.ExecuteNonQuery();
			} finally {
				cnn.Close();
			}
			return result;
		}

        public void SpBeginExecuteReader(string spName, Action<AsyncDbCallback<DataReader>> callback, string[] parameters, params object[] values) {
            if (callback == null)
                throw new ArgumentNullException(string.Format("Exec {0} Error! Callback action can not be null!!", spName));

            SqlConnection cnn = GetConnection();
            SqlCommand cmd = new SqlCommand(spName, cnn);
            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandType = CommandType.StoredProcedure;

            try {
                SpFillParameters(cmd, parameters, values);

                AsyncDbCallback<DataReader> context = new AsyncDbCallback<DataReader>();
                context.SqlCommand = cmd;
                context.SqlConnection = cnn;
                context.CallbackType = CallbackType.DataReader;
                context.DbType = IICDbType.SqlServer2005;
                cmd.BeginExecuteReader(
                    new AsyncCallback(delegate(IAsyncResult ar) {
                        AsyncDbCallback<DataReader> c = ar.AsyncState as AsyncDbCallback<DataReader>;
                        c.ar = ar;
                        callback(context);
                    } ), 
                    context, 
                    CommandBehavior.CloseConnection);
            } catch (System.Exception ex) {
                cnn.Close();
                throw ex;
            }
        }

		public DataReader SpExecuteReader(string spName, string[] parameters, params object[] values)
		{
            DataReader result = null;
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			try {
                SpFillParameters(cmd, parameters, values);
                result = new DataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd, cnn);
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return result;
		}

		public T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
		{
			T result = default(T);

			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			try {
                SpFillParameters(cmd, parameters, values);

                Stopwatch watch = Stopwatch.StartNew();

				result = (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
			} finally {
				cnn.Close();
			}
			return result;
		}

		public DataTable SpExecuteTable(string spName, string[] parameters, params object[] values)
		{
			DataTable result = new DataTable();
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			try {
                SpFillParameters(cmd, parameters, values);

				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}

			return result;
		}

		public DataSet SpExecuteDataSet(string spName, string[] parameters, params object[] values)
		{
			DataSet result = new DataSet();
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			try {
                SpFillParameters(cmd, parameters, values);
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}

			return result;
		}

		public int BulkInsert<T>(string tableName, IEnumerable<T> values)
		{
			Type type = typeof(T);
			ComboClass<DataTable, List<TableFieldAttribute>> cache;
			if (!_bulkCaches.TryGetValue(type, out cache)) {
				cache = BuildBulkCache(type);
				_bulkCaches.Add(type, cache);
			}

            SqlBulkCopy bulkCopy = new SqlBulkCopy(_connStr, SqlBulkCopyOptions.KeepIdentity);
			DataTable table = cache.V1.Clone();
			foreach (TableFieldAttribute attr in cache.V2) {
				bulkCopy.ColumnMappings.Add((SqlBulkCopyColumnMapping)attr.Context);
			}

			foreach (T value in values) {
				DataRow row = table.NewRow();
				foreach (TableFieldAttribute attr in cache.V2) {
					object obj = attr.Field.GetValue(value);
					if (attr.FieldType != null)
						obj = Convert.ChangeType(obj, attr.FieldType);

					DataColumn column = table.Columns[attr.ColumnName];

					row[column] = obj;
				}
				table.Rows.Add(row);
			}

			try {
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.WriteToServer(table);
			} finally {
				bulkCopy.Close();
			}
			return table.Rows.Count;
		}

		public int BulkInsert(string tableName, DataTable table)
		{
			if (table == null || table.Rows.Count == 0)
				throw new InvalidOperationException("NULL or Empty Datas");

			SqlConnection sqlConn = null;
			SqlBulkCopy bulkCopy = null;

			try {
				sqlConn = GetConnection();
				bulkCopy = new SqlBulkCopy(sqlConn);
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.BatchSize = table.Rows.Count;

                //DataRow[] rows = new DataRow[table.Rows.Count];
                //table.Rows.CopyTo(rows, 0);

                //bulkCopy.WriteToServer(rows);
                foreach(DataColumn dc in table.Columns)
                {
                    bulkCopy.ColumnMappings.Add(dc.ColumnName,dc.ColumnName);
                }
                bulkCopy.WriteToServer(table);
                

				return table.Rows.Count;
			} finally {
				if (bulkCopy != null)
					bulkCopy.Close();

				if (sqlConn != null)
					sqlConn.Close();
			}			
		}

		public string FormatSql(string spName, string[] parameters, params object[] values)
		{
			try {
				StringBuilder str = new StringBuilder();
				str.Append("EXEC ");
				str.Append(spName);
				if (values != null) {
					str.Append(" ");
					for (int i = 0; i < values.Length; i++) {
						str.Append(parameters[i]);
						str.Append(" = ");
						str.Append(SqlUtils.FormatSql(values[i]));
						if (i != values.Length - 1)
							str.Append(", ");
					}
				}
				return str.ToString();
			} catch (Exception ex) {
				return "Format Failed:" + ex.ToString();
			}
		}
		#endregion

		#region Private Methods
		private SqlConnection GetConnection()
		{
			SqlConnection cnn = new SqlConnection(_connStr);

			try {
				cnn.Open();
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return cnn;
		}

		private void SpFillParameters(SqlCommand command, string[] parameters, object[] values)
		{
			if (parameters == null || parameters.Length == 0) {
				return;
			}

			if (values == null || values.Length == 0)
				return;

			for (int i = 0; i < parameters.Length; i++) {
				string parameter = parameters[i];
				object value = values[i];
				if (value != null)
					command.Parameters.AddWithValue(parameter, value);
				else
					command.Parameters.AddWithValue(parameter, DBNull.Value);
			}
		}

		private void SqlFillParameters(SqlCommand command, object[] values)
		{
			if (values == null || values.Length == 0)
				return;

			for (int i = 0; i < values.Length; i++) {
				string parameter = string.Format("@p{0}", i);
				object value = values[i];

				command.Parameters.AddWithValue(parameter, value ?? DBNull.Value);
			}
		}

		private ComboClass<DataTable, List<TableFieldAttribute>> BuildBulkCache(Type t)
		{
			DataTable table = new DataTable();
			FieldInfo[] fields = t.GetFields();

			List<TableFieldAttribute> attrs = new List<TableFieldAttribute>();

			foreach (FieldInfo field in fields) {
				TableFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<TableFieldAttribute>(field);
				if (fieldAttr != null) {
					DataColumn column = new DataColumn();
					column.ColumnName = fieldAttr.ColumnName;
					column.DataType = fieldAttr.FieldType ?? field.FieldType;
					table.Columns.Add(column);

					fieldAttr.Field = field;
					fieldAttr.Column = column;
					fieldAttr.Context = new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName);
					attrs.Add(fieldAttr);
				}
			}

			ComboClass<DataTable, List<TableFieldAttribute>> cache = new ComboClass<DataTable, List<TableFieldAttribute>>();
			cache.V1 = table;
			cache.V2 = attrs;

			return cache;
		}
		#endregion

		private SafeDictionary<Type, ComboClass<DataTable, List<TableFieldAttribute>>> _bulkCaches = new SafeDictionary<Type, ComboClass<DataTable, List<TableFieldAttribute>>>();

		#region IDatabaseOperationEx Members
		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			int result = 0;
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;
			SqlFillParameters(cmd, parameters);

			try {
				result = cmd.ExecuteNonQuery();
			} finally {
				cnn.Close();
			}
			return result;
		}

		public T ExecuteScalar<T>(string sql, params object[] parameters)
		{
			T result = default(T);

			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;
			SqlFillParameters(cmd, parameters);

			try {
				result = (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
			} finally {
				cnn.Close();
			}
			return result;
		}

		public DataReader ExecuteReader(string sql, params object[] parameters)
		{
            DataReader result = null;
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;
			SqlFillParameters(cmd, parameters);

			try {
				result = new DataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection), cmd, cnn);
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return result;
		}

		public DataTable ExecuteTable(string sql, params object[] parameters)
		{
			DataTable result = new DataTable();
			SqlConnection cnn = GetConnection();
			SqlCommand cmd = new SqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;
			SqlFillParameters(cmd, parameters);

			try {
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}
			return result;
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
