using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcDetectorService: IRpcDetectorService
	{
		public void GetProtocolInfo(RpcServerContext ctx)
		{
			//RpcProtocolInfo ret = new RpcProtocolInfo() {
			//    Version = "4.0",
			//    ServiceName = ServiceEnviornment.ServiceName,
			//    ComputerName = ServiceEnviornment.ComputerName,
			//    AutoBatch = true,
			//    BatchCount = 32,
			//    BatchIdleMs = 60,
			//    MaxBodySize = ctx.Channel.ChannelSettings,
			//};
			//ctx.Return(ret);
			throw new NotImplementedException();
		}

		public void EnumServiceInfo(RpcServerContext ctx)
		{
			throw new NotImplementedException();
		}

		public void EnumMethodInfo(RpcServerContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
