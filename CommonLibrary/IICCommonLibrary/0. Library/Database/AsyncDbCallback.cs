using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data.Common;
using System.Data.SqlClient;
using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4
{
	public enum CallbackType : int
	{
		Unknown = 0,
		DataReader = 1,
		NonQuery = 2,
	}

	public class AsyncDbCallback<T>
	{
		public DbConnection SqlConnection = null;
		public DbCommand SqlCommand = null;
		public IAsyncResult ar = null;
		public IICDbType DbType = IICDbType.SqlServer2005;
		public CallbackType CallbackType = CallbackType.Unknown;

		public T EndInvoke()
		{
			T result = default(T);
			try {
				switch (CallbackType) {
					case CallbackType.DataReader:
						if (DbType == IICDbType.SqlServer2005) {
							SqlCommand cmd = SqlCommand as SqlCommand;
							try {
								SqlDataReader retValue = cmd.EndExecuteReader(ar);
								result = (T)Convert.ChangeType(new DataReader(retValue, cmd, SqlConnection), typeof(T));
							} catch (Exception ex) {
								SqlConnection.Close();
								throw ex;
							}
						} else if (DbType == IICDbType.Mysql) {
							MySqlCommand cmd = SqlCommand as MySqlCommand;
							try {
								MySqlDataReader retValue = cmd.EndExecuteReader(ar);
								result = (T)Convert.ChangeType(new DataReader(retValue, cmd, SqlConnection), typeof(T));
							} catch (Exception ex) {
								SqlConnection.Close();
								throw ex;
							}
						}
						break;

					case CallbackType.NonQuery:
						if (DbType == IICDbType.SqlServer2005) {
							SqlCommand cmd = SqlCommand as SqlCommand;
							try {
								int retValue = cmd.EndExecuteNonQuery(ar);
								result = (T)Convert.ChangeType(retValue, typeof(T));
							} catch (Exception ex) {
								SqlConnection.Close();
								throw ex;
							}
						} else if (DbType == IICDbType.Mysql) {
							MySqlCommand cmd = SqlCommand as MySqlCommand;
							try {
								int retValue = cmd.EndExecuteNonQuery(ar);
								result = (T)Convert.ChangeType(retValue, typeof(T));
							} catch (Exception ex) {
								SqlConnection.Close();
								throw ex;
							}
						}
						break;

				}
				return result;
			} catch (System.Exception ex) {
				throw ex;
			}
		}
	}
}
