using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Class)]
	public class HAServiceAttribute: Attribute
	{
		public HAServiceAttribute(string serviceName)
		{
			_serviceName = serviceName;
		}

		public string ServiceName
		{
			get { return _serviceName; }
		}

		private string _serviceName;
	}
}
