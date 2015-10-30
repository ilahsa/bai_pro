using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcSimplexConnection: RpcConnection
	{
		private ServerUri _serverUri;
		private RpcClientChannel _channel;

		public override ServerUri RemoteUri
		{
			get { return _serverUri; }
		}

		public RpcSimplexConnection(RpcClientChannel channel, ServerUri uri)
			: base(RpcConnectionMode.Simplex, RpcConnectionDirection.Client)
		{
			_channel = channel;
			_serverUri = uri;
		}

		public override bool Connected
		{
			get { return true; }
		}

		public override void Disconnect()
		{	
		}

		public override void BeginConnect(Action<Exception> callback)
		{
			callback(null);
		}

		public override RpcClientTransaction CreateTransaction(RpcRequest request)
		{
			return _channel.CreateTransaction(_serverUri, request);
		}
	}
}
