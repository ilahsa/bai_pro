using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocClientTransaction: RpcClientTransaction
	{
		internal Action Callback;

		public RpcInprocClientTransaction(RpcRequest request)
			: base(InprocUri.Instance, request)
		{
		}

		public override void SendRequest(Action callback, int timeout)
		{
			Callback = callback;
			RpcInprocServerTransaction trans = new RpcInprocServerTransaction(this);
			RpcInprocServerChannel.LinkTransaction(trans);
		}
	}
}