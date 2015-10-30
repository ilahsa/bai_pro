using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		当使用RpcDuplexClient时, 被callback的Transaction.Connection.Channel引用
	/// </summary>
	/// <remarks>实质上将一个ClientChannel封装成ServerChannel</remarks>
	public class RpcDuplexCallbackChannel: RpcServerChannel
	{
		private RpcConnection _connection;
		private RpcClientChannel _channel;

		public RpcDuplexCallbackChannel(string protocol, string serverUri, RpcConnection conn, RpcClientChannel channel)
			: base(protocol, serverUri)
		{
			_connection = conn;
			_channel = channel;

			p_channelSettings = _channel.DefaultSettings;
		}

		protected override void DoStart()
		{
			// nothing to do
		}

		protected override void DoStop()
		{
			// nothing to do
		}
	}
}
