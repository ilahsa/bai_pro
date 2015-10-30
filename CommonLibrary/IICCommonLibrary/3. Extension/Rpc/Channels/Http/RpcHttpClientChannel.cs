using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpClientChannel: RpcClientChannel
	{
		public RpcHttpClientChannel()
			: base("http")
		{
			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.None,
				Version = "4.1",
			};
		}

		public override RpcClientTransaction CreateTransaction(ServerUri serverUri, RpcRequest request)
		{
			IICAssert.IsInstanceOfType(serverUri, typeof(HttpUri));
			return new RpcHttpClientTransaction(this, (HttpUri)serverUri, request);
		}
	}
}
