using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Observation
{
	public class ObserverDelegate: IObserver
	{
		private string _name;
		private DateTime _clearTime;
		private Func<List<ObserverItem>> _obsereProc;
		private Action _clearProc;

		public ObserverDelegate(string name, Func<List<ObserverItem>> obsereProc, Action clearProc)
		{
			IICAssert.IsTrue(obsereProc != null);
			IICAssert.IsTrue(clearProc != null);

			_name = name;
			_obsereProc = obsereProc;
			_clearProc = clearProc;
			_clearTime = DateTime.Now;
		}

		public string ObserverName
		{
			get { return _name; }
		}

		public DateTime ClearTime
		{
			get { return _clearTime; }
		}

		public List<ObserverItem> Observe()
		{
			return _obsereProc();
		}

		public void ClearObserver()
		{
			_clearProc();
			_clearTime = DateTime.Now;
		}
	}
}
