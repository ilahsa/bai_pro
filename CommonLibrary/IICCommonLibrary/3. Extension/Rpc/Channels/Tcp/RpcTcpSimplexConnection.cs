using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		处理Rpc客户端的Simplex连接
	/// </summary>
	/// <remarks>
	/// 1. 自动创建conncurrentConnection条连接
	/// 2. 每条连接创建时如果出错则自动重试
	/// 3. 被动断线后不自动重连, 由业务触发
	/// 4. 所有连接实行最大回收时间内重连的机制, 确保负载均衡下对新启动机器的连通率
	/// </remarks>
	class RpcTcpSimplexConnection : RpcConnection, IDisposable
	{
		#region Private fields
		private LoopCounter _lc;
		private RpcTcpClientChannel _channel;
		private RpcTcpSimplexConnectionWrapper[] _connections;
		private ComboClass<int> _connectionCount;
		private TcpUri _serverUri;
		// private int _connectionPendings;
		#endregion

		public override ServerUri RemoteUri
		{
			get { return _serverUri; }
		}

		#region Public members
		public RpcTcpSimplexConnection(RpcTcpClientChannel channel, TcpUri serverUri, int concurrentConnection)
			: base(RpcConnectionMode.Simplex, RpcConnectionDirection.Client)
		{
			_channel = channel;
			_serverUri = serverUri;
			_connections = new RpcTcpSimplexConnectionWrapper[concurrentConnection];
			_lc = new LoopCounter(concurrentConnection);
			_connectionCount = new ComboClass<int>(0);

			for (int i = 0; i < concurrentConnection; i++) {
				var wrapper = new RpcTcpSimplexConnectionWrapper(this);
				RpcTcpSimplexConnectionManager.AddConnection(wrapper);
				_connections[i] = wrapper;
			}
		}

		public override void Disconnect()
		{
			foreach (var a in _connections) {
				a.Close();
			}
		}

		/// <summary>
		///		被动关闭连接, 所有的子连接都断开
		/// </summary>
		internal void OnSubDisconnected()
		{
			// TODO:
			// 应当增加的逻辑:
			// 1. 如何回收长期不用的Simplex连接，这个不是一个非常重要的问题，但是也需要进行考虑
			//foreach (
			//OnDisconnected();
		}

		/// <summary>
		///		子连接创建成功
		/// </summary>
		internal void OnSubConnected()
		{
			//OnDisconnected();
		}

		public override RpcClientTransaction CreateTransaction(RpcRequest request)
		{
			int n = _lc.Next();
			var wrapper = _connections[n];
			var sock = wrapper.GetSocket();
			if (sock == null) {
				throw new RpcException(RpcErrorCode.ConnectionPending, _serverUri.ToString(), "no SimplexConnection available", null);
			}
			var tx = RpcTcpTransactionManager.CreateClientTransaction(_serverUri, sock, request);
			return tx;
		}

		public void Dispose()
		{
		}

		public override void BeginConnect(Action<Exception> callback)
		{
			throw new NotSupportedException("Should never run this");
		}

		public override bool Connected
		{
			get { return _connectionCount.Value > 0; }
		}
		#endregion
	}
}
