using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Imps.Services.CommonV4.DbAccess
{
	public interface IDatabaseOperation
	{
		/// <summary>Count in Seconds</summary>
		int CommandTimeout { get; set; }

		int SpExecuteNonQuery(string spName, string[] paramters, params object[] values);

        /// <summary>
        /// SpExecuteNonQuery的异步方法。。。
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <param name="values"></param>
        void SpBeginExecuteNonQuery(string spName, Action<AsyncDbCallback<int>> callback, string[] parameters, params object[] values);
		T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values);
        DataReader SpExecuteReader(string spName, string[] paramters, params object[] values);

        /// <summary>
        /// SpExecuteReader的异步方法。。。
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="spName"></param>
        /// <param name="paramters"></param>
        /// <param name="values"></param>
        void SpBeginExecuteReader(string spName, Action<AsyncDbCallback<DataReader>> callback, string[] paramters, params object[] values);
		DataTable SpExecuteTable(string spName, string[] parameters, params object[] values);
		DataSet SpExecuteDataSet(string spName, string[] paramters, params object[] values);
		int BulkInsert<T>(string tableName, IEnumerable<T> values);
		int BulkInsert(string tableName, DataTable table);
        int BulkInsert(string tableName, DataTable table, string[] colNames);

		int ExecuteNonQuery(string sql, params object[] parameters);
		T ExecuteScalar<T>(string sql, params object[] parameters);
        DataReader ExecuteReader(string sql, params object[] parameters);
		DataTable ExecuteTable(string sql, params object[] parameters);

        string GetDatabaseName();
		string FormatSql(string spName, string[] parameters, params object[] values);
    }
}
