using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Observation
{
	public interface IObserver
	{
		string ObserverName { get; }

		DateTime ClearTime { get; }

		List<ObserverItem> Observe();

		void ClearObserver();
	}
}
