using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocServerChannel: RpcServerChannel
	{
		public static RpcInprocServerChannel Instance = new RpcInprocServerChannel();

		private RpcInprocServerChannel()
			: base("inproc", "inproc:///")
		{
			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.None,
				Version = "4.1",
			};
		}

		protected override void DoStart()
		{
			// do nothing
		}

		protected override void DoStop()
		{
			// do nothing
		}

		public static void LinkTransaction(RpcServerTransaction tx)
		{
			RpcInprocServerChannel.Instance.OnTransactionCreated(tx);
		}
	}
}
