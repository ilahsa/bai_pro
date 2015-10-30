using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpServerConnection: RpcConnection, IDisposable
	{
		private RpcTcpServerChannel _channel;
		private RpcTcpSocketConnection _sock;

        public override ServerUri RemoteUri
		{
            get { return _sock.RemoteUri; }
		}

		public RpcTcpServerConnection(RpcTcpServerChannel channel, RpcTcpSocketConnection sock)
			: base(RpcConnectionMode.Unknown, RpcConnectionDirection.Server)
		{
			_channel = channel;
			_sock = sock;

			_sock.RequestReceived += new Action<RpcTcpSocketConnection, int, RpcRequest>(
				(socket, seq, request) => {
					RpcTcpServerTransaction tx = new RpcTcpServerTransaction(_channel, this, socket, request, seq);
					OnTransactionCreated(tx);
				}	
			);

			_sock.ResponseReceived += new Action<RpcTcpSocketConnection, int, RpcResponse>(
				(socket, seq, response) => {
					RpcTcpTransactionManager.EndTransaction(seq, response);
				}
			);

			_sock.Disconnected += new Action<RpcTcpSocketConnection>(
				(socket) => {
					OnDisconnected();
				}
			);
		}

		public override void Disconnect()
		{
			_sock.Disconnect();
		}

		public override RpcClientTransaction CreateTransaction(RpcRequest request)
		{
			RpcTcpClientTransaction trans = RpcTcpTransactionManager.CreateClientTransaction(_sock.RemoteUri, _sock, request);
			return trans;
		}
		
		public void Dispose()
		{
			_sock.Dispose();
		}

		public override bool Connected
		{
			get { return _sock.Connected; }
		}

		public override void BeginConnect(Action<Exception> callback)
		{
			throw new NotSupportedException("This is a passive server connection");
		}
	}
}
