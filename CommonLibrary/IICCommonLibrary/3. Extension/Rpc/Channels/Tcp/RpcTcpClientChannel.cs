using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc的Tcp客户端通道
	/// </summary>
    public class RpcTcpClientChannel : RpcClientChannel
    {
		private object _syncRoot = new object();
        private Dictionary<ServerUri, RpcTcpSimplexConnection> _simplexConnections;
        private Dictionary<ServerUri, RpcTcpDuplexConnection> _duplexConnections;

		/// <summary>
		///		构造函数, 不需要参数
		/// </summary>
        public RpcTcpClientChannel()
            : base("tcp")
        {
			RpcTcpBufferManager.Initialize();
			RpcTcpSimplexConnectionManager.Initialize();

            _simplexConnections = new Dictionary<ServerUri, RpcTcpSimplexConnection>();
            _duplexConnections = new Dictionary<ServerUri, RpcTcpDuplexConnection>();

			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.Connection | RpcChannelSupportModes.DuplexConnection,
				Version = "4.3",
				Timeout = RpcTcpBufferManager.Configuration.ChannelItem.Timeout,
				ConcurrentConnection = RpcTcpBufferManager.Configuration.ChannelItem.SimplexConnections,
			};

			RpcTcpTransactionManager.Initialize();
        }

		/// <summary>
		///		创建一个Transaction
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <remarks>这个事务会建立在单工的Tcp连接上</remarks>
        public override RpcClientTransaction CreateTransaction(ServerUri uri, RpcRequest request)
        {
            RpcTcpSimplexConnection conn;
            lock (_syncRoot) {
                if (!_simplexConnections.TryGetValue(uri, out conn)) {
					conn = new RpcTcpSimplexConnection(this, (TcpUri)uri, p_channelSettings.ConcurrentConnection);
					conn.Disconnected += new Action<RpcConnection>(conn_Disconnected);
                    _simplexConnections.Add(uri, conn);
                }
            }
            return conn.CreateTransaction(request);
        }

		private void conn_Disconnected(RpcConnection obj)
		{
			lock (_syncRoot) {
				RpcSimplexConnection conn = (RpcSimplexConnection)obj;
				_simplexConnections.Remove(conn.RemoteUri);
			}
		}

		/// <summary>
		///		创建连接
		/// </summary>
		/// <param name="serverUri"></param>
		/// <param name="mode">单工或是双工连接</param>
		/// <returns></returns>
		public override RpcConnection CreateConnection(ServerUri serverUri, RpcConnectionMode mode)
		{
			switch (mode) {
				case RpcConnectionMode.Simplex:
					return new RpcTcpSimplexConnection(this, (TcpUri)serverUri, p_channelSettings.ConcurrentConnection);
				case RpcConnectionMode.Duplex:
					return new RpcTcpDuplexConnection(this, (TcpUri)serverUri);
				default:
					throw new NotSupportedException();
			}
		}
    }
}
