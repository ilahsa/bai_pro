using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public enum TccAction
	{
		None = 0,
		Try,
		Confirm,
		Cancel,
	}

	public enum TccWorkState
	{
		None			= 0,
		Trying			= 1,
		Tryed			= 2,
		TryFailed		= 3,
		Confirming		= 4,
		Confirmed		= 5,
		ConfirmFailed	= 6,
		Cancelling		= 7,
		Cancelled		= 8,
		CancelFailed	= 9,
	}
}
