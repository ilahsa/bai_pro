using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public abstract class IICCodeTableBase
	{
		public IICCodeTableBase(string name, DateTime version)
		{
			_tableName = name;
			_version = version;
		}

		public string TableName
		{
			get { return _tableName; }
		}

		public DateTime Version
		{
			get { return _version; }
		}

		private string _tableName;
		private DateTime _version;
	}
}
