using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Sample
{
	class RpcProxyService: RpcServiceBase
	{
		public RpcProxyService()
			: base("MasterProxy")
		{
		}

		public override void OnTransactionStart(RpcServerContext context)
		{
			//MasterClientUri uri = (MasterClientUri)(context.ContextUri);
			//RpcDuplexServer.findClient(uri);

			//RpcConnection connection;

			//context.
		}
	}
}
