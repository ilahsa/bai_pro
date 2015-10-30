using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc的Tcp服务器通道，只支持IPv4
	/// </summary>
	public sealed class RpcTcpServerChannel : RpcServerChannel
	{
		#region Private Members
		private const int MaxPendingConnection = 100;

		private int _port;
		private Socket _socket;
		private object _syncRoot = new object();
		private SocketAsyncEventArgs _eAccept;
		private Dictionary<ServerUri, RpcTcpServerConnection> _connections;
		#endregion

		#region Constructor
		/// <summary>
		///		构造函数
		/// </summary>
		/// <param name="port">tcp的监听端口</param>
		public RpcTcpServerChannel(int port)
			: base("tcp", "tcp://" + ServiceEnvironment.ComputerAddress + ":" + port)
		{
			RpcTcpBufferManager.Initialize();

			_port = port;
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_connections = new Dictionary<ServerUri, RpcTcpServerConnection>();

			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.Connection | RpcChannelSupportModes.DuplexConnection,
				Version = "4.3",
				Timeout = RpcTcpBufferManager.Configuration.ChannelItem.Timeout,
				ConcurrentConnection = RpcTcpBufferManager.Configuration.ChannelItem.SimplexConnections,
			};

			_eAccept = RpcTcpBufferManager.FetchAcceptArgs();
			((RpcTcpAsyncArgs)_eAccept).Callback = (e) => ProcessAccept(e);
		}
		#endregion

		#region Protected methods
		protected override void DoStart()
		{
			_socket.Bind(new IPEndPoint(IPAddress.Any, _port));
			_socket.Listen(MaxPendingConnection);
            
			BeginAccept();
		}

		protected override void DoStop()
		{
			_socket.Close();
		}
		#endregion

		#region Private Members
		private void BeginAccept()
		{
			try {
				if (!_socket.AcceptAsync(_eAccept)) {
					ProcessAccept(_eAccept);
				}
			} catch (Exception ex) {
				_tracing.ErrorFmt(ex, "BeginAccept Failed");
			}
		}

		void ProcessAccept(SocketAsyncEventArgs e)
		{
			_tracing.InfoFmt("Accept Connection");

			try {
				RpcTcpSocketConnection s = new RpcTcpSocketConnection(RpcConnectionDirection.Server);
				RpcTcpServerConnection conn = new RpcTcpServerConnection(this, s);

				conn.TransactionCreated += new Action<RpcConnection, RpcServerTransaction>(
					(cnn, tx) => {
						OnTransactionCreated(tx);
					}
				);

				conn.Disconnected += new Action<RpcConnection>(
					(cnn) => {
						RemoveConnection((RpcTcpServerConnection)cnn);
					}
				);

				//
                // 把事件都挂全了的，再beginrecieve
                s.Accept(e);

                // 按原来的逻辑此时conn.RemoteUri = null；
				lock (_syncRoot) {
					 _connections.Add(conn.RemoteUri, conn);
				}
				_tracing.InfoFmt("Connection Created from {0}", conn.RemoteUri);
				OnConnectionCreated(conn);
			} catch (Exception ex) {
				try {
					e.AcceptSocket.Close();
				} catch (Exception ex2) {
					_tracing.Error(ex2, "AcceptSocket.Close() Failed");
				}
				_tracing.Error(ex, "ProcessConnect Failed");
			} finally {
				e.AcceptSocket = null;
				BeginAccept();
			}
		}
		#endregion

		#region Private methods
		private void RemoveConnection(RpcTcpServerConnection cnn)
		{
			lock (_syncRoot) {
				if (_connections.ContainsKey(cnn.RemoteUri)) {
					_connections.Remove(cnn.RemoteUri);
				}
			}
		}

		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpServerChannel));
		#endregion
	}
}
