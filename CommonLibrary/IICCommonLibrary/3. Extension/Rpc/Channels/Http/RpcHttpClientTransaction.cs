using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpClientTransaction: RpcClientTransaction, IDisposable
	{
		private Action _callback;
		private RpcHttpClientChannel _channel;
		private RegisteredWaitHandle _registeredHandle = null;

		private WebRequest _webRequest = null;
		private WebResponse _webResponse = null;
		private ManualResetEvent _waitHandle;

		public RpcHttpClientTransaction(RpcHttpClientChannel channel, HttpUri serverUri, RpcRequest request)
			: base(serverUri, request)
		{
			_channel = channel;
		}

		protected override void Dispose(bool disposing)
		{
			if (_webRequest != null) {
				try {
					_webRequest.Abort();
				} catch (Exception ex) {
					SystemLog.Unexcepted(ex);
				}
			}
			if (_waitHandle != null) {
				_waitHandle.Close();
			}
		}

		public override void SendRequest(Action callback, int timeout)
		{
			_callback = callback;

            _webRequest = HttpWebRequest.Create(ServiceUrl);
            _webRequest.Method = "POST";
			_webRequest.Proxy = null;
            _webRequest.ContentType = "multipart/byteranges";
            _webRequest.Headers.Add(HttpRequestHeader.From, Request.ServiceAtComputer);
			_webRequest.Headers.Add(HttpRequestHeader.Pragma, ServiceUrl);
			if (!string.IsNullOrEmpty(Request.ContextUri))
				_webRequest.Headers.Add("ToUri", Request.ContextUri);

			byte[] buffer = null;
			if (Request.BodyBuffer == null) {
				_webRequest.Headers.Add("Null", "true");
				_webRequest.ContentLength = 0;
			} else {
				buffer = Request.BodyBuffer.GetByteArray();
				_webRequest.ContentLength = buffer.Length;
			}

            timeout = timeout > 0 ? timeout : _channel.Timeout;

			if (timeout > 0) {
				_waitHandle = new ManualResetEvent(false);
				_registeredHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, new WaitOrTimerCallback(TimeoutCallback), this, timeout, true);
			}
			if (_webRequest.ContentLength == 0) {
				_webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
			} else {
				_webRequest.BeginGetRequestStream(
					delegate(IAsyncResult asyncResult) {
						try {
							Stream stream = _webRequest.EndGetRequestStream(asyncResult);
							stream.Write(buffer, 0, buffer.Length);
							stream.Close();
							_webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
						} catch (Exception ex) {
							Response = RpcResponse.Create(RpcErrorCode.SendFailed, ex);
							_callback();
						}
					}, 
					this
				);
			}
		}

		private static void ResponseCallback(IAsyncResult asyncResult)
		{
			RpcHttpClientTransaction trans = (RpcHttpClientTransaction)asyncResult.AsyncState;
			RpcResponse response = null;

			try {
				var webResponse = trans._webRequest.EndGetResponse(asyncResult);
				trans._webResponse = webResponse;

				string warn = webResponse.Headers.Get("Warning");

				if (!string.IsNullOrEmpty(warn)) {
					RpcErrorCode errCode = (RpcErrorCode)Enum.Parse(typeof(RpcErrorCode), warn);
					if (errCode != RpcErrorCode.OK) {
						Exception ex = null;
						if (webResponse.ContentLength > 0) {
							Stream stream = webResponse.GetResponseStream();
							ex = BinarySerializer.Deserialize<Exception>(stream);
						}
						response = RpcResponse.Create(errCode, ex);
					} else {
						SystemLog.Error(LogEventID.RpcFailed, "Unexcepted Message");
						response = RpcResponse.Create(RpcErrorCode.Unknown, null);
					}
				} else {
					bool hasBody = (webResponse.Headers["Null"] != "true");
					if (hasBody) {
						if (webResponse.ContentLength == 0) {
							response = RpcResponse.Create(RpcNull.EmptyStream, 0);
						} else {
							Stream stream = webResponse.GetResponseStream();
							response = RpcResponse.Create(stream, (int)webResponse.ContentLength);
						}
					} else {
						response = RpcResponse.Create();
					}
				}
			} catch (WebException ex) {
				if (ex.Status == WebExceptionStatus.Timeout) {
					response = RpcResponse.Create(RpcErrorCode.TransactionTimeout, ex);
				} else {
					response = RpcResponse.Create(RpcErrorCode.SendFailed, ex);
				}
			} catch (Exception ex) {
				response = RpcResponse.Create(RpcErrorCode.SendFailed, ex);
			}

			trans.OnCallback(response);
		}

		private static void TimeoutCallback(object state, bool timedOut)
		{
			RpcHttpClientTransaction trans = null;
			try {
				trans = (RpcHttpClientTransaction)state;

				if (trans._registeredHandle != null)
					trans._registeredHandle.Unregister(null);

				if (timedOut) { // Timeout
					var response = RpcResponse.Create(RpcErrorCode.TransactionTimeout, null);
					trans.OnCallback(response);
				}
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "TimeoutCallback");
			}
		}

		private void OnCallback(RpcResponse resposne)
		{
			Response = resposne;
			if (_waitHandle != null)
				_waitHandle.Set();

			_callback();
		}
	}
}