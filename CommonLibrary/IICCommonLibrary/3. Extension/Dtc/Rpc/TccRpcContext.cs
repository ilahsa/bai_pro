using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Dtc
{
	public class TccRpcContext<TArgs, TResults>
	{
		public TArgs Args;

		public RpcServerContext InnerContext;

		public TccRpcContext(RpcServerContext context)
		{
			Args = context.GetArgs<TArgs>();
			InnerContext = context;
		}

		public void ThrowException(Exception ex)
		{
			InnerContext.ThrowException(ex);
		}

		public void Return(TResults ret)
		{
			InnerContext.Return<TResults>(ret);
		}

		public void Return()
		{
			InnerContext.Return();
		}
	}
}
