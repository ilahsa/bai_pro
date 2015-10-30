using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
	[IICConfigItem("Database", KeyField="Name")]
	public sealed class IICDbConfigItem: IICConfigItem
	{
		[IICConfigField("Name")]
		public string Name;

		[IICConfigField("DbType")]
		public IICDbType DbType;

		[IICConfigField("ConnectionString")]
		public string ConnectionString;

		[IICConfigField("CommandTimeout", DefaultValue = 120)]
		public int CommandTimeout;
	}
}
 