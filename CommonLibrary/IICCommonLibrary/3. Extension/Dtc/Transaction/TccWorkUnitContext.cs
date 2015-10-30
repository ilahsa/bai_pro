using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public class TccWorkUnitContext<TContext>
	{
		private TContext _value;
		private Action<Exception> _callback;

		public TccWorkUnitContext(TContext value, Action<Exception> callback)
		{
			_value = value;
			_callback = callback;
		}

		public TContext Value
		{
			get { return _value; }
		}

		public void ThrowException(Exception ex)
		{
			_callback(ex);
		}

		public void Return()
		{
			_callback(null);
		}
	}
}
