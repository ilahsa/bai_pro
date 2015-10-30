using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingSniffer: IDisposable
	{
		public const int MaxSniffer = 8;

		private string _url;
		private bool _enabled;
		private LazyQueue<TracingEvent>	_queueTracing;
		private LazyQueue<SystemLogEvent> _queueSystemLog;
		private RpcClientProxy _proxy;
		private TracingLevel _level;
		private string _from;
		private string _to;

		public string Url
		{
			get { return _url; }
		}

		~TracingSniffer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (disposing) {
				// no managed
			}

			if (_queueTracing != null) {
				_queueTracing.Dispose();
				_queueTracing = null;
			}

			if (_queueSystemLog != null) {
				_queueSystemLog.Dispose();
				_queueSystemLog = null;
			}
		}

		public TracingSniffer(string url, TracingLevel level, string from , string to)
		{
			_url = url;
			_level = level;
			_from = from;
			_to = to;
			_enabled = true;
			_queueTracing = new LazyQueue<TracingEvent>("TracingSniffer.Tracing", 32, 50, DequeueActionTracing);
			_queueSystemLog = new LazyQueue<SystemLogEvent>("TracingSniffer.SystemLog", 32, 50, DequeueActionSystemLog);

			_proxy = RpcProxyFactory.GetProxy<ITracingSniffer>(_url);
			// _proxy.ShutUp = true;
		}

		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		public void Enqueue(TracingEvent evt)
		{
			if (!_enabled)
				return;

			try {
				_queueTracing.Enqueue(evt);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.TracingFailed, ex, "TracingSniffer.Enqueue:{0}", _url);
			}
		}

		public void Enqueue(SystemLogEvent evt)
		{
			if (!_enabled)
				return;

			try {
				_queueSystemLog.Enqueue(evt);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.TracingFailed, ex, "TracingSniffer.Enqueue:{0}", _url);
			}
		}

		public bool CanSniff(TracingLevel level)
		{
			return _enabled && (level >= _level);
		}

		public bool CanSniff(TracingLevel level, string from, string to)
		{
			return CanSniff(level)
				&& string.IsNullOrEmpty(_from) ? true : (from == _from)
				&& string.IsNullOrEmpty(_to) ? true : (to == _to);
		}

		public void SendData<T>(string methodName, T args)
		{
			if (!_enabled)
				return;
				
			int retryCount = 0;
			while (retryCount < 3) {
				try {
					_proxy.Invoke<T, RpcNull>(methodName, args);
					return;
				} catch (Exception ex) {
					SystemLog.Warn(LogEventID.TracingFailed, ex, "TracingSniffer<{0}> Failed! try: {1}", _url, retryCount);
					Thread.Sleep(3000 * retryCount);
					retryCount++;
				}
			}

			_enabled = false;
			_queueTracing.Dispose();
			_queueSystemLog.Dispose();
			SystemLog.Warn(LogEventID.TracingFailed, "TracingSniffer<{0}> over MaxError, AutoDisabled", _url);
			TracingManager.RemoveSniffer(Url);
		}

		private void DequeueActionTracing(TracingEvent[] evts)
		{
			SendData<TracingEvent[]>("AppendTrace", evts);
		}

		private void DequeueActionSystemLog(SystemLogEvent[] evts)
		{
			SendData<SystemLogEvent[]>("AppendSystemLog", evts);
		}
	}
}
