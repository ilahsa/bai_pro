using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
    public sealed class RetryProtector
    {
        private string _name;
		private int _retryCount;
		private int[] _delaySeconds;
        private DateTime _retryTime;
		private Exception _lastException;

		public RetryProtector(string name, int[] delaySeconds)
		{
		    _name = name;
			_delaySeconds = delaySeconds;
			_retryTime = DateTime.MinValue;
			_retryCount = 0;
		}

		public void OnSuccess()
		{
			_retryCount = 0;
		}

		public void OnException(Exception ex)
		{
			_lastException = ex;
			int i;
			if (_retryCount >= _delaySeconds.Length) {
				i = _delaySeconds.Length - 1;
			} else {
				i = _retryCount;
			}

			_retryCount++;
			_retryTime = DateTime.Now.AddSeconds(_delaySeconds[i]);
		}

		public bool Failing
		{
			get { return _retryCount > 0 && DateTime.Now < _retryTime; }
		}

		public string FailingMessage
		{
			get { return string.Format("RetryProtector Failed Count {0} will retry in {1} \r\n {2}", 
				_retryCount, _retryTime, _lastException); }
		}
    }
}
