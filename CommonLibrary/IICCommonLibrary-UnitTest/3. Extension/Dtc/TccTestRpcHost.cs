using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Dtc;

namespace UnitTest.Dtc
{
	class TccTestRpcHost: TccRpcHostUnit<string, string>
	{
		public TccTestRpcHost()
			: base("RpcWork1")
		{
		}

		protected override void Try(TccRpcContext<string, string> context)
		{
			context.Return("Hello");
		}

		protected override void Confirm(TccRpcContext<string, string> context)
		{
			context.Return("Hello");
		}

		protected override void Cancel(TccRpcContext<string, string> context)
		{
			context.Return("Hello");
		}
	}
}
