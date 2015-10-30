using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Observation
{
	public class ObserverItem
	{
		public ObserverItemFormatter GetFormatter()
		{
			return ObserverItemFormatter.GetFormatter(this.GetType());
		}

		public virtual void BeforeFetchData() { }
	}
}
