using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public enum TccTransactionState
	{
		New				= 0,
		LockTrying		= 1,
		Trying			= 2,
		LockConfirming	= 3,
		Confirming		= 4,
		Confirmed		= 5,
		ConfirmFailed	= 6,
		LockCanceling	= 7,
		Cancelling		= 8,
		Cancelled		= 9,
		CancelFailed	= 10,
	}
}
