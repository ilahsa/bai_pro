/*
 * A Wrapper of ReaderWriterLock
 * 
 * Author:	Lei Gao
 * Created:	2008-03-06
 */
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class IICReaderWriterLock
	{
		public const int DefaultTimeout = -1;

		private ReaderWriterLock _innerLock = new ReaderWriterLock();

		public IICReaderWriterLock()
		{
			_innerLock = new ReaderWriterLock();
		}

		public IICLockRegion LockForRead(int millisecondTimeout)
		{
			IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.ReaderLock, millisecondTimeout);
			return region;
		}

		public IICLockRegion LockForRead()
		{
			return LockForRead(DefaultTimeout);
		}

		public IICLockRegion LockForUpgradeableRead(int millisecondTimeout)
		{
			IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.UpgradeableReadLock, millisecondTimeout);
			return region;
		}

		public IICLockRegion LockForUpgradeableRead()
		{
			return LockForUpgradeableRead(DefaultTimeout);
		}

		public IICLockRegion LockForWrite(int millisecondTimeout)
		{
			IICLockRegion region = new IICLockRegion(_innerLock, IICLockMode.WriterLock, millisecondTimeout);
			return region;
		}

		public IICLockRegion LockForWrite()
		{
			return LockForWrite(DefaultTimeout);
		}
	}
}

/*
'JAPH V1.0';$/=0xE0;$^F=1<<5;$;=$^F>>3;$,=
'Nywx$           /\/\       x7Fy~'.
'Ersxl      ,"~~~    \      kde~b'.
'iv$Ti     /        @ \_    ox*Zo'.
'vp$Le  ~~|           __0   f*Bki'.
'goiv2     \||||--||-|/     ax0ox';
{$_=chr(ord(substr($,,$"++,1))-$;);print;
$"=($"&$/)+$^Fif$"%$^F>$;;redo if$"<=$^F*$;+$;;}
*/