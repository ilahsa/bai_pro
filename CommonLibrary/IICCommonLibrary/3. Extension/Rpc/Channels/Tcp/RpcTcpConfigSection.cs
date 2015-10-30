using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[IICConfigSection("RpcOverTcp", IsRequired = false)]
	public class RpcTcpConfigSection: IICConfigSection
	{
		[IICConfigItem("Channel", IsRequired = false)]
		public RpcTcpChannelConfigItem ChannelItem;

		[IICConfigItem("Receive", IsRequired = false)]
		public RpcTcpReceiveConfigItem ReceiveItem;

		[IICConfigItem("Send", IsRequired = false)]
		public RpcTcpSendConfigItem SendItem;

		public override void SetDefaultValue()
		{
			ChannelItem = IICConfigItem.CreateDefault<RpcTcpChannelConfigItem>();
			ReceiveItem = IICConfigItem.CreateDefault<RpcTcpReceiveConfigItem>();
			SendItem = IICConfigItem.CreateDefault<RpcTcpSendConfigItem>();
		}
	}

	[IICConfigItem("Channel", IsRequired = false)]
	public class RpcTcpChannelConfigItem: IICConfigItem
	{
		[IICConfigField("SimplexConnections", DefaultValue = 3)]
		public int SimplexConnections;

		[IICConfigField("SimplexConnectionLife", DefaultValue = 180)]
		public int SimplexConnectionLife;

		[IICConfigField("Timeout", DefaultValue = 180000)]
		public int Timeout;
	}

	[IICConfigItem("Receive", IsRequired = false)]
	public class RpcTcpReceiveConfigItem: IICConfigItem
	{
		[IICConfigField("MaxConnections", DefaultValue = 128)]
		public int MaxConnections;

		[IICConfigField("BufferSize", DefaultValue = 4 * 1024)]
		public int BufferSize;
	}

	[IICConfigItem("Send", IsRequired = false)]
	public class RpcTcpSendConfigItem : IICConfigItem
	{
		[IICConfigField("MaxPending", DefaultValue = 2 * 1024)]
		public int MaxPending;

		[IICConfigField("MaxBatch", DefaultValue = 8)]
		public int MaxBatch;

		[IICConfigField("BufferSize", DefaultValue = 8 * 1024)]
		public int BufferSize;

		[IICConfigField("BufferCount", DefaultValue = 64)]
		public int BufferCount;
	}
}
