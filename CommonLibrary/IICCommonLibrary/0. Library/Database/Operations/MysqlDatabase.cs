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
	public class MysqlDatabase: IDatabaseOperation
	{
		const string TabSplitter = "\t";
		const string LineSplitter = "\r";

		#region Common & Constructor
		private int _commandTimeout;
		private string _connStr;
        private string _databaseName;

		public int CommandTimeout
		{
			get { return _commandTimeout; }
			set { _commandTimeout = value; }
		}

		public MysqlDatabase(string connStr)
			: this(connStr, -1)
		{
		}

		public MysqlDatabase(string connStr, int commandTimeout)
		{
			_connStr = connStr;
			_commandTimeout = commandTimeout;
		}
		#endregion

		#region DatabaseOperation Methods
        //public IAsyncResult SpBeginExecuteNonQuery(string spName, AsyncCallback callback, object stateObject, string[] parameters, params object[] values) {
        //    throw new NotImplementedException();
        //}

        //public int SpEndExecuteNonQuery(IDbCommand cmd, IAsyncResult ar) {
        //    throw new NotImplementedException();
        //}

        public void SpBeginExecuteNonQuery(string spName, Action<AsyncDbCallback<int>> callback, string[] parameters, params object[] values) {
            if (callback == null)
                throw new ArgumentNullException(string.Format("Exec {0} Error! Callback action can not be null!!", spName));

            MySqlConnection cnn = GetConnection();
            MySqlCommand cmd = new MySqlCommand(spName, cnn);

            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandType = CommandType.StoredProcedure;

            try {
                SpFillParameters(cmd, parameters, values);

                AsyncDbCallback<int> context = new AsyncDbCallback<int>();
                context.CallbackType = CallbackType.NonQuery;
                context.DbType = IICDbType.Mysql;
                context.SqlConnection = cnn;
                context.SqlCommand = cmd;

                cmd.BeginExecuteNonQuery(new AsyncCallback(delegate(IAsyncResult ar) {
                    context.ar = ar;
                    callback(context);
                }), context);
            } catch (System.Exception ex) {
                cnn.Close();
                throw ex;
            }
        }

		public int SpExecuteNonQuery(string spName, string[] parameters, params object[] values)
		{
			int result = 0;
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(spName, cnn);

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

        public IAsyncResult SpBeginExecuteReader(string spName, AsyncCallback callback, object stateObject, string[] parameters, params object[] values) {
            throw new NotImplementedException();
        }

        public IDataReader SpEndExecuteReader(IDbCommand cmd, IAsyncResult ar) {
            throw new NotImplementedException();
        }

        public void SpBeginExecuteReader(string spName, Action<AsyncDbCallback<DataReader>> callback, string[] parameters, params object[] values)
        {
            //if (callback == null)
            //    throw new ArgumentNullException(string.Format("Exec {0} Error! Callback action can not be null!!", spName));

            //MySqlConnection cnn = GetConnection();
            //MySqlCommand cmd = new MySqlCommand(spName, cnn);

            //cmd.CommandTimeout = CommandTimeout;
            //cmd.CommandType = CommandType.StoredProcedure;

            //try {
            //    SpFillParameters(cmd, parameters, values);
            //    AsyncDbCallback<DataReader> context = new AsyncDbCallback<DataReader>();
            //    context.SqlConnection = cnn;
            //    context.SqlCommand = cmd;
            //    context.CallbackType = CallbackType.DataReader;
            //    context.DbType = IICDbType.Mysql;

            //    cmd.BeginExecuteReader(
            //        new AsyncCallback(delegate(IAsyncResult ar) {
            //            context.ar = ar;
            //            callback(context);
            //        }), 
            //        context, 
            //        CommandBehavior.CloseConnection);
            //} catch (System.Exception ex) {
            //    cnn.Close();
            //    throw ex;
            //}
        }

		public DataReader SpExecuteReader(string spName, string[] parameters, params object[] values)
		{
			DataReader result = null;
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			try {
                SpFillParameters(cmd, parameters, values);
				IDataReader reader = (IDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader == null)
					throw new ApplicationException("mysql query interrupted");

				result = new DataReader(reader, cmd, cnn);
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return result;
		}

		public T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
		{
			T result = default(T);

			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			try {
                SpFillParameters(cmd, parameters, values);

				result = (T)Convert.ChangeType(cmd.ExecuteScalar(), typeof(T));
			} finally {
				cnn.Close();
			}
			return result;
		}

		public DataTable SpExecuteTable(string spName, string[] parameters, params object[] values)
		{
			DataTable result = new DataTable();
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			
			try {
                SpFillParameters(cmd, parameters, values);
				MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
				adp.FillError += new FillErrorEventHandler(
					delegate(object sender, FillErrorEventArgs e) {
						e.Continue = true;
						e.DataTable.Rows.Add(e.Values);
					}
				);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}

			return result;
		}

		public DataSet SpExecuteDataSet(string spName, string[] parameters, params object[] values)
		{
			DataSet result = new DataSet();
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			
			try {
                SpFillParameters(cmd, parameters, values);

				MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}
			return result;
		}

		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			int result = 0;
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(sql, cnn);

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

		public DataReader ExecuteReader(string sql, params object[] parameters)
		{
			DataReader result = null;
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;

			SqlFillParameters(cmd, parameters);
			try {
				IDataReader reader = (IDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader == null)
					throw new ApplicationException("mysql query interrupted");

				result = new DataReader(reader, cmd, cnn);
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return result;
		}

		public T ExecuteScalar<T>(string sql, params object[] parameters)
		{
			T result = default(T);

			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;

			SqlFillParameters(cmd, parameters);
			try {
				object obj = cmd.ExecuteScalar();
				if (obj is DBNull)
					return default(T);
				else
					result = (T)Convert.ChangeType(obj, typeof(T));
			} finally {
				cnn.Close();
			}
			return result;
		}

		public DataTable ExecuteTable(string sql, params object[] parameters)
		{
			DataTable result = new DataTable();
			MySqlConnection cnn = GetConnection();
			MySqlCommand cmd = new MySqlCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;

			SqlFillParameters(cmd, parameters);
			try {
				MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
				adp.FillError += new FillErrorEventHandler(
					delegate(object sender, FillErrorEventArgs e) {
						e.Continue = true;
						e.DataTable.Rows.Add(e.Values);
					}
				);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}

			return result;
		}

		public int BulkInsert<T>(string tableName, IEnumerable<T> values)
		{
			Type type = typeof(T);
			List<TableFieldAttribute> attrs;
			if (!_bulkCaches.TryGetValue(type, out attrs)) {
				attrs = BuildBulkCache(type);
				_bulkCaches[type] = attrs;
			}

			string seed = Guid.NewGuid().ToString("N");
			string path = Environment.CurrentDirectory + "\\" + seed + ".txt";

			using (StreamWriter writer = new StreamWriter(path, false)) {
				foreach (T value in values) {
					int n = attrs.Count;
					for (int i = 0; i < n; i++) {
						var attr = attrs[i];
						object obj = attr.Field.GetValue(value);
						writer.Write(FormatData(obj, attr.Field.FieldType));
						if (i < n - 1)
							writer.Write(TabSplitter);
					}
					writer.Write(LineSplitter);
				}
			}

			int ret;
			MySqlConnection cnn = null;

			try {
				cnn = GetConnection();
				MySqlBulkLoader bulkLoader = new MySqlBulkLoader(cnn);

				bulkLoader.TableName = tableName;
				bulkLoader.FileName = path;
				bulkLoader.ConflictOption = MySqlBulkLoaderConflictOption.Replace;
				bulkLoader.FieldTerminator = TabSplitter;
				bulkLoader.LineTerminator = LineSplitter;
				bulkLoader.EscapeCharacter = '\\';
				bulkLoader.Priority = MySqlBulkLoaderPriority.Concurrent;

				//
				// UTF-8会导致第一行数据丢失, 暂时没有解决办法
				// bulkLoader.CharacterSet = "UTF8";
				
				ret = bulkLoader.Load();
			} catch (Exception) {
				throw;
			} finally {
				if (cnn != null)
					cnn.Close();

				try {
					if (File.Exists(path)) {
						File.Delete(path);
					}
				} catch (Exception ex) {
					Trace.Write(ex.ToString());
				}
			}
			return ret;
		}

		public int BulkInsert(string tableName, DataTable table)
		{
			string seed = Guid.NewGuid().ToString("N");
			string path = Environment.CurrentDirectory + "\\" + seed + ".txt";

			using (StreamWriter writer = new StreamWriter(path, false)) {
				foreach (DataRow row in table.Rows) {
					int n = table.Columns.Count;
					for (int i = 0; i < n; i++) {
						object obj = row[i];
						writer.Write(FormatData(obj, obj.GetType()));
						if (i < n - 1)
							writer.Write(TabSplitter);
					}
					writer.Write(LineSplitter);
				}
			}

			int ret;
			MySqlConnection cnn = null;

			try {
				cnn = GetConnection();
				MySqlBulkLoader bulkLoader = new MySqlBulkLoader(cnn);

				bulkLoader.TableName = tableName;
				bulkLoader.FileName = path;
				bulkLoader.ConflictOption = MySqlBulkLoaderConflictOption.Replace;
				bulkLoader.FieldTerminator = TabSplitter;
				bulkLoader.LineTerminator = LineSplitter;
				bulkLoader.EscapeCharacter = '\\';
				bulkLoader.Priority = MySqlBulkLoaderPriority.Concurrent;

				//
				// UTF-8会导致第一行数据丢失, 暂时没有解决办法
				// bulkLoader.CharacterSet = "UTF8";

				ret = bulkLoader.Load();
			} catch (Exception) {
				throw;
			} finally {
				if (cnn != null)
					cnn.Close();

				try {
					if (File.Exists(path)) {
						File.Delete(path);
					}
				} catch (Exception ex) {
					Trace.Write(ex.ToString());
				}
			}
			return ret;			
		}
        /// <summary>
        /// 根据指定的字段顺序插入表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        /// <param name="colNames">字段顺序列</param>
        /// <returns></returns>
        public int BulkInsert(string tableName, DataTable table, string[] colNames)
        {
            //比对表字段数否一致
            if (table.Columns.Count != colNames.Length)
                throw new Exception(string.Format("Columns count is not equal,源:{0};目的:{1}",table.TableName,tableName));
            for (int i = 0; i < colNames.Length; i++)
            {
                if (!table.Columns.Contains(colNames[i]))
                    throw new Exception(string.Format("DataTable:{0} does not countains Colume:{1}",table.TableName,colNames[i]));
            }
            
            string seed = Guid.NewGuid().ToString("N");
            string path = Environment.CurrentDirectory + "\\" + seed + ".txt";

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                foreach (DataRow row in table.Rows)
                {
                    int n = table.Columns.Count;
                    for (int i = 0; i < n; i++)
                    {
                        string colName = colNames[i];
                        object obj = row[colName];
                        writer.Write(FormatData(obj, obj.GetType()));
                        if (i < n - 1)
                            writer.Write(TabSplitter);
                    }
                    writer.Write(LineSplitter);
                }
            }

            int ret;
            MySqlConnection cnn = null;

            try
            {
                cnn = GetConnection();
                MySqlBulkLoader bulkLoader = new MySqlBulkLoader(cnn);

                bulkLoader.TableName = tableName;
                bulkLoader.FileName = path;
                bulkLoader.ConflictOption = MySqlBulkLoaderConflictOption.Replace;
                bulkLoader.FieldTerminator = TabSplitter;
                bulkLoader.LineTerminator = LineSplitter;
                bulkLoader.EscapeCharacter = '\\';
                bulkLoader.Priority = MySqlBulkLoaderPriority.Concurrent;

                //
                // UTF-8会导致第一行数据丢失, 暂时没有解决办法
                // bulkLoader.CharacterSet = "UTF8";

                ret = bulkLoader.Load();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cnn != null)
                    cnn.Close();

                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write(ex.ToString());
                }
            }
            return ret;
        }

        public string GetDatabaseName()
        {
            if (string.IsNullOrEmpty(_databaseName))
            {
                MySqlConnection cnn = null;
                try
                {
                    cnn = GetConnection();
                    _databaseName = cnn.Database;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (cnn != null)
                        cnn.Close();
                }
                
            }
            return _databaseName;
        }

        public string FormatSql(string spName, string[] parameters, params object[] values)
		{
			try {
				StringBuilder str = new StringBuilder();
				str.Append("CALL ");
				str.Append(spName);
				if (values != null && values.Length != 0) {
					str.Append("(");
					for (int i = 0; i < values.Length; i++) {
						str.Append(SqlUtils.FormatSql(values[i]));
						if (i != values.Length - 1)
							str.Append(", ");
					}
					str.Append(")");
				}
				return str.ToString();
			} catch (Exception ex) {
				return "Format Failed:" + ex.ToString();
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

		private List<TableFieldAttribute> BuildBulkCache(Type t)
		{
			FieldInfo[] fields = t.GetFields();
			List<TableFieldAttribute> attrs = new List<TableFieldAttribute>();

			foreach (FieldInfo field in fields) {
				TableFieldAttribute fieldAttr = AttributeHelper.TryGetAttribute<TableFieldAttribute>(field);
				if (fieldAttr != null) {
					fieldAttr.Field = field;
					attrs.Add(fieldAttr);
				}
			}

			return attrs;
		}

		private string FormatData(object obj, Type fieldType)
		{
			if (obj == null) {
				return "";
			} else if (fieldType.IsEnum) {
				return Convert.ToInt64(obj).ToString();
			} else if (fieldType == typeof(DateTime)) {
				return  ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss.fff");
			} else if (fieldType == typeof(bool)) {
				return (bool)obj ? "1" : "0";
			} else if (fieldType == typeof(byte[])) {
				throw new NotSupportedException("Can't use byte[] in MysqlDatabase.Insert()");
			} else {
				string s = obj.ToString();
				s = XmlHelper.MaskInvalidCharacters(s);
				s = s.Replace("\\", "\\\\");
				s = s.Replace(TabSplitter, string.Empty);
				s = s.Replace(LineSplitter, string.Empty);
				return s;
			}
		}

		private void SpFillParameters(MySqlCommand command, string[] parameters, object[] values)
		{
			if (parameters == null || parameters.Length == 0)
				return;

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

		private void SqlFillParameters(MySqlCommand command, object[] values)
		{
			if (values == null || values.Length == 0)
				return;

			// command.CommandText = command.CommandText.Replace("@", "v_");
			for (int i = 0; i < values.Length; i++) {
				string parameter = string.Format("@p{0}",i);
				object value = values[i];
				command.Parameters.AddWithValue(parameter, value ?? DBNull.Value);
			}
		}
		#endregion

		private object _syncRoot = new object();
		private Dictionary<Type, List<TableFieldAttribute>> _bulkCaches = new Dictionary<Type, List<TableFieldAttribute>>();

		#region IDatabaseOperationEx Members

		#endregion
	}
}