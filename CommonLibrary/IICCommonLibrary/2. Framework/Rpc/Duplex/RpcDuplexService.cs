using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcDuplexService : IRpcDuplexService
	{
		public void Register(RpcServerContext ctx)
		{
			var token = (RpcDuplexConnectionToken)ctx.Connection.Contexts["Duplex"];

			if (token.Registerd) {
				ctx.Return(RpcErrorCode.ConnectionFailed);
				return;
			} else {
				token.ProcessRegister(ctx);
			}
		}

		public void KeepAlive(RpcServerContext ctx)
		{
			var token = (RpcDuplexConnectionToken)ctx.Connection.Contexts["Duplex"];

			if (token != null && token.Registerd) {
				token.ProcessKeepalive(ctx);				
			} else {
				ctx.Return(RpcErrorCode.ConnectionFailed);
				return;
			}
		}
	}
}
