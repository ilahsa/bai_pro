using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpDuplexConnection: RpcConnection, IDisposable
	{
		private RpcDuplexCallbackChannel _channel;
		private RpcTcpSocketConnection _socket;
		private TcpUri _serverUri;

		public override bool Connected
		{
			get { return _socket.Connected; }
		}

		public override ServerUri RemoteUri
		{
			get { return _serverUri; }
		}

		public RpcTcpDuplexConnection(RpcTcpClientChannel channel, TcpUri serverUri)
			: base(RpcConnectionMode.Duplex, RpcConnectionDirection.Client)
		{
			_serverUri = serverUri;

			_socket = new RpcTcpSocketConnection(RpcConnectionDirection.Client);
			_socket.Disconnected += new Action<RpcTcpSocketConnection>(
				(socket) => {
					OnDisconnected();
				}
			);

			_socket.RequestReceived += new Action<RpcTcpSocketConnection, int, RpcRequest>(
				(socket, seq, request) => {
					var tx = new RpcTcpServerTransaction(_channel, this, socket, request, seq);
					OnTransactionCreated(tx);
				}
			);

			_socket.ResponseReceived += new Action<RpcTcpSocketConnection,int,RpcResponse>(
				(socket, seq, response) => {
					RpcTcpTransactionManager.EndTransaction(seq, response);
				}
			);

			_channel = new RpcDuplexCallbackChannel("tcp", serverUri.ToString(), this, channel);
		}

		public override void BeginConnect(Action<Exception> callback)
		{
			_socket.BeginConnect((TcpUri)RemoteUri, callback, true);
		}

		public override void Disconnect()
		{
			_socket.Disconnect();
		}

		public override RpcClientTransaction CreateTransaction(RpcRequest request)
		{
			RpcTcpClientTransaction tx = RpcTcpTransactionManager.CreateClientTransaction(RemoteUri, _socket, request);
			return tx;
		}

		public void Dispose()
		{
			Disconnect();
		}
	}
}
