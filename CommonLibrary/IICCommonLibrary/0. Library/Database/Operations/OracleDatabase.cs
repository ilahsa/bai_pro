using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
    class OracleDatabase : IDatabaseOperation
    {
        const string TabSplitter = "\t";
		const string LineSplitter = "\r";
        const string ReturnParameter = "Re_Value";

		#region Common & Constructor
		private int _commandTimeout;
		private string _connStr;
        private string _databaseName;

		public int CommandTimeout
		{
			get { return _commandTimeout; }
			set { _commandTimeout = value; }
		}

		public OracleDatabase(string connStr)
			: this(connStr, -1)
		{
		}

        public OracleDatabase(string connStr, int commandTimeout)
		{
			_connStr = connStr;
			_commandTimeout = commandTimeout;
		}
		#endregion

		#region DatabaseOperation Methods
		public int SpExecuteNonQuery(string spName, string[] parameters, params object[] values)
		{
			int result = 0;
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			SpFillParameters(cmd, parameters, values);
			try {
				result = cmd.ExecuteNonQuery();
			} finally {
				cnn.Close();
			}
			return result;
		}

		public DataReader SpExecuteReader(string spName, string[] parameters, params object[] values)
		{
            //re construct the spname para pkg name yzy 2010-7-9 14:07:21
            string spNewName = spName.Insert(0, spName + ".");
			DataReader result = null;
			OracleConnection cnn = GetConnection();
            OracleCommand cmd = new OracleCommand(spNewName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			SpFillParameters(cmd, parameters, values);
            //to add special para cursor add by yzy 2010-7-9 10:41:06
            OracleParameter OracleParam=cmd.Parameters.Add(new OracleParameter(ReturnParameter, OracleType.Cursor));
            OracleParam.Direction = ParameterDirection.Output;

			try {
				IDataReader reader = (IDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection);
				if (reader == null)
					throw new ApplicationException("oracle query interrupted");

				result = new DataReader(reader, cmd, cnn);
			} catch (Exception) {
				cnn.Close();
				throw;
			}
			return result;
		}

        public DataReader SpExecuteReader_Bak(string spName, string[] parameters, params object[] values)
        {
            DataReader result = null;
            OracleConnection cnn = GetConnection();
            OracleCommand cmd = new OracleCommand(spName, cnn);

            cmd.CommandTimeout = CommandTimeout;
            cmd.CommandType = CommandType.StoredProcedure;

            SpFillParameters(cmd, parameters, values);
            try
            {
                IDataReader reader = (IDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader == null)
                    throw new ApplicationException("oracle query interrupted");

                result = new DataReader(reader, cmd, cnn);
            }
            catch (Exception)
            {
                cnn.Close();
                throw;
            }
            return result;
        }

		public T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values)
		{
			T result = default(T);

			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;

			SpFillParameters(cmd, parameters, values);
            //add the out para
            OracleParameter OracleParam;
            OracleParam = cmd.Parameters.Add(new OracleParameter(ReturnParameter, OracleType.VarChar, 30));
            OracleParam.Direction = ParameterDirection.Output;
            try 
            {
                cmd.ExecuteNonQuery();
                string sReturn = cmd.Parameters[ReturnParameter].Value.ToString();

                result = (T)Convert.ChangeType(sReturn, typeof(T));

			
			} 
            finally 
            {
				cnn.Close();
			}
			return result;
		}

		public DataTable SpExecuteTable(string spName, string[] parameters, params object[] values)
		{
			DataTable result = new DataTable();
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			
			SpFillParameters(cmd, parameters, values);
			try {
				OracleDataAdapter adp = new OracleDataAdapter(cmd);
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
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(spName, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.StoredProcedure;
			
			SpFillParameters(cmd, parameters, values);
			try {
				OracleDataAdapter adp = new OracleDataAdapter(cmd);
				adp.Fill(result);
			} finally {
				cnn.Close();
			}
			return result;
		}

		public int ExecuteNonQuery(string sql, params object[] parameters)
		{
			int result = 0;
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(sql, cnn);

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
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(sql, cnn);

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

			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(sql, cnn);

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
			OracleConnection cnn = GetConnection();
			OracleCommand cmd = new OracleCommand(sql, cnn);

			cmd.CommandTimeout = CommandTimeout;
			cmd.CommandType = CommandType.Text;

			SqlFillParameters(cmd, parameters);
			try {
				OracleDataAdapter adp = new OracleDataAdapter(cmd);
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
            throw new NotSupportedException();

		}

		public int BulkInsert(string tableName, DataTable table)
		{
            throw new NotSupportedException();

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
            throw new NotSupportedException();

        }

        public string GetDatabaseName()
        {
            if (string.IsNullOrEmpty(_databaseName))
            {
                OracleConnection cnn = null;
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
		private OracleConnection GetConnection()
		{
            OracleConnection cnn = new OracleConnection(_connStr);
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

		private void SpFillParameters(OracleCommand command, string[] parameters, object[] values)
		{
			if (parameters == null || parameters.Length == 0)
				return;

			if (values == null || values.Length == 0)
				return;

			for (int i = 0; i < parameters.Length; i++) {
				string parameter = parameters[i];
				object value = values[i];
                //if (values[i].GetType() == typeof(System.Boolean))
                //{
                //    value = (long)((bool)values[i] == true ? 1 : 0);

                //}
                if (value != null)
                {
                    if (value.ToString().Length >= 4000)
                    {
                        OracleParameter a = new OracleParameter(parameter.Replace("@", "in_"), OracleType.Clob, value.ToString().Length);
                        a.Value = value;
                        command.Parameters.Add(a);
                    }   
                    else
                    {
                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            value = " ";
                        }
                        if (value.GetType().ToString() == "System.DateTime")
                        {
                            //command.Parameters.AddWithValue(parameter.Replace("@", "in_"),value);
                            //command.Parameters.Add(new OracleParameter(parameter.Replace("@", "in_"),OracleType.DateTime,value));
                            OracleParameter p = new OracleParameter();
                            p.ParameterName = parameter.Replace("@", "in_");
                            p.OracleType = OracleType.DateTime;
                            p.Value = value;
                            command.Parameters.Add(p);

                           // OracleParam.Direction = ParameterDirection.Input;

                        }
                        else
                            command.Parameters.AddWithValue(parameter.Replace("@", "in_"), value);
                   }
                }
                else
                    command.Parameters.AddWithValue(parameter.Replace("@", "in_"), DBNull.Value);
			}
		}

		private void SqlFillParameters(OracleCommand command, object[] values)
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

        #region IDatabaseOperation Members


        public void SpBeginExecuteNonQuery(string spName, Action<AsyncDbCallback<int>> callback, string[] parameters, params object[] values)
        {
            throw new NotImplementedException();
        }

        public void SpBeginExecuteReader(string spName, Action<AsyncDbCallback<DataReader>> callback, string[] paramters, params object[] values)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
