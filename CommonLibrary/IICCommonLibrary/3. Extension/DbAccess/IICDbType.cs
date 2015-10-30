using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		DbAccess支持的数据库类型
	/// </summary>
	public enum IICDbType
	{
		/// <summary>SqlServer2005</summary>
		SqlServer2005,

		/// <summary>Mysql: 其中BulkInsert方法不支持binary数据</summary>
		Mysql,

		/// <summary>Mysql：仅用于binary数据的BulkInsert方法</summary>
		MysqlBatchInsert,

		/// <summary>Oracle</summary>
		Oracle,
	}
}
