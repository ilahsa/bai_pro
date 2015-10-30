using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocClientChannel: RpcClientChannel
	{
		public static RpcInprocClientChannel Instance = new RpcInprocClientChannel();

		private RpcInprocClientChannel()
			: base("inproc")
		{
			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.None,
				Version = "4.1",
			};
		}

		public override RpcClientTransaction CreateTransaction(ServerUri serverUri, RpcRequest request)
		{	
			return new RpcInprocClientTransaction(request);
		}
	}
}
