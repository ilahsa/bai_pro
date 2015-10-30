using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	class Class1
	{
		public static void foo() 
		{

			RpcTcpServerChannel channel = new RpcTcpServerChannel(8000);

			channel.TransactionCreated += new Action<RpcServerTransaction>(channel_TransactionCreated);
			RpcServiceManager.RegisterServerChannel(channel);

			RpcServiceManager.RegisterRawService(new RpcRouteService());
		}

		static void channel_TransactionCreated(RpcServerTransaction obj)
		{
			//obj.Bypass();
			//obj.SendResponse();
		}
			
	}

	class RpcRouteService : RpcServiceBase
	{
		public RpcRouteService()
			:base("workerService")
		{
		}

		public override void OnTransactionStart(RpcServerContext context)
		{
			//if (context.ContextUri) {
				RpcConnection conn = null;
				RpcClientTransaction tx = conn.CreateTransaction(context.Request);
				tx.SendRequest(
					delegate() {
						context.Return(tx.Response);
					},
					-1
					
				);
			//}
		}
	}
}
