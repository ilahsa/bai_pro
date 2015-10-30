using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocServerTransaction: RpcServerTransaction
	{
		private RpcInprocClientTransaction _peerTx;

		public RpcInprocServerTransaction(RpcInprocClientTransaction tx)
			: base(RpcInprocServerChannel.Instance, null, tx.Request)
		{	
			_peerTx = tx;
		}

		public override void SendResponse(RpcResponse response)
		{
			_peerTx.Response = response;
			_peerTx.Callback();
		}
	}
}
