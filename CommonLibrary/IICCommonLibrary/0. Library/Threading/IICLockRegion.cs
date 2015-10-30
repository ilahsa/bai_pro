/*
 * A Wrapper of ReaderWriterLock
 * 
 * Author:	Lei Gao 
 * Created:	2008-03-06
 * Modified:
 *			2009-09-16 改用3.5版本的ReaderWriterLockSlim实现
 *			2009-11-?? ReaderWriterLockSlim有未知的性能问题, 又回滚了
 */
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public enum IICLockMode
	{
		ReaderLock,
		UpgradeableReadLock, 
		UpdatedWriterLock,
		WriterLock,
	}

	public class IICLockRegion: IDisposable
	{
		private ReaderWriterLock _innerLock;
		private IICLockMode _lockMode;
		private int _msTimeout;
		private LockCookie _lockCookie;

		public IICLockRegion(ReaderWriterLock innerLock, IICLockMode mode, int millisecondTimeout)
		{
			_innerLock = innerLock;
			_lockMode = mode;
			_msTimeout = millisecondTimeout;

			switch (mode) {
				case IICLockMode.ReaderLock:
					_innerLock.AcquireReaderLock(millisecondTimeout);
					break;
				case IICLockMode.WriterLock:
					_innerLock.AcquireWriterLock(millisecondTimeout);
					break;
				default:
					throw new NotSupportedException("Unexcepted LockMode: " + mode);
			}
		}

		public void Upgrade()
		{
			if (_lockMode != IICLockMode.ReaderLock)
					throw new InvalidOperationException("lock mode can't upgraded:" + _lockMode);

				_lockCookie = _innerLock.UpgradeToWriterLock(_msTimeout);
				_lockMode = IICLockMode.UpdatedWriterLock;
		}

		public void Dispose()
		{
			switch (_lockMode) {
				case IICLockMode.ReaderLock:
 					_innerLock.ReleaseReaderLock();
					break;
				case IICLockMode.UpdatedWriterLock:
					_innerLock.DowngradeFromWriterLock(ref _lockCookie);
					_innerLock.ReleaseReaderLock();
					break;
				case IICLockMode.WriterLock:
					_innerLock.ReleaseWriterLock();
					break;
			}
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