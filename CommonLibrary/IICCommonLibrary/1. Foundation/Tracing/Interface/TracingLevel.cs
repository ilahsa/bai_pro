using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public enum TracingLevel
	{
		All		= 10000,
		Info	= 30000,
		Warn	= 50000,
		Error	= 80000,
		Off		= 99999,
	}
}
