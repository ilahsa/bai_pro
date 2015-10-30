using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpSimplexConnectionWrapper
	{
		private object _syncRoot;
		private DateTime _retryBegin;
		private RpcTcpSocketConnection _sock;
		private RpcTcpSimplexConnection _parent;

		public RpcTcpSimplexConnectionWrapper(RpcTcpSimplexConnection parent)
		{
			_syncRoot = new object();
			_parent = parent;
			_retryBegin = DateTime.MinValue;
			CreateConnection();
		}

		public RpcTcpSocketConnection GetSocket()
		{
			if (_sock == null || _sock.Closed) {
				if (DateTime.Now > _retryBegin) {
					lock (_syncRoot) {
						if (_sock == null || _sock.Closed) {
							CreateConnection();
						}
					}
				}
			}
			//
			// socket 是close的情况， client transaction不能放在这个socket上的pending queue上
			// 不然的话 因为soket已经closed，所以transaction永远也得不到处理的机会，会留在内存
			if (_sock == null || _sock.Closed)
				return null;
			else
				return _sock;
		}

		public void TryRecycle()
		{
			if (_sock == null || !_sock.Connected)
				return;

			var span = DateTime.Now - _sock.ConnectedTime;
			if (span.TotalSeconds > RpcTcpBufferManager.Configuration.ChannelItem.SimplexConnectionLife) {
				_tracing.InfoFmt("Try to recycle connection: {0} connected={1} span={2}", _sock.RemoteUri, _sock.ConnectedTime, span);
				RpcTcpSimplexConnectionManager.DelayClose(_sock);
				lock (_syncRoot) {
					CreateConnection();
				}
			}
		}

		public void Close()
		{
			if (_sock != null)
				_sock.Disconnect();
		}

		private void CreateConnection()
		{
			var sock = new RpcTcpSocketConnection(RpcConnectionDirection.Client);
			sock.Disconnected += new Action<RpcTcpSocketConnection>(
				(s) => {
					_parent.OnSubDisconnected();
				}
			);

			sock.RequestReceived += new Action<RpcTcpSocketConnection, int, RpcRequest>(
				(s, seq, request) => {
					// TODO
					// Not support this response
				}
			);

			sock.ResponseReceived += new Action<RpcTcpSocketConnection, int, RpcResponse>(
				(s, seq, response) => {
					RpcTcpTransactionManager.EndTransaction(seq, response);
				}
			);

			//
			// Auto Connect
			sock.BeginConnect(
				(TcpUri)_parent.RemoteUri,
				delegate(Exception ex) {
					if (ex != null) {
						_retryBegin = DateTime.Now.AddSeconds(1);
					} else {
						_parent.OnSubConnected();
					}
				},
				false
			);

			_sock = sock;
		}

		public string GetInfo()
		{
			return "SimplexConnectionWrapper:" + _parent.RemoteUri;
		}

		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpSimplexConnectionWrapper));
	}
}