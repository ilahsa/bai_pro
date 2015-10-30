using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public abstract class TccRpcWorkUnit<TContext, TArgs, TResults> : TccWorkUnit<TContext>
	{
		public TccRpcWorkUnit(string workName)
			: base(workName)
		{
		}

		protected override void Try(TccWorkUnitContext<TContext> ctx)
		{
			InvokeAction("try_", ctx, true);
		}

		protected override void Confirm(TccWorkUnitContext<TContext> ctx)
		{
			InvokeAction("confirm_", ctx, false);
		}

		protected override void Cancel(TccWorkUnitContext<TContext> ctx)
		{
			InvokeAction("cancel_", ctx, false);
		}

		private void InvokeAction(string prefix, TccWorkUnitContext<TContext> ctx, bool convertResults)
		{
			var uri = GetUri(ctx.Value);
			RpcClientProxy proxy = RpcProxyFactory.GetProxy<ITccRpcHostService>(uri);
			proxy.BeginInvoke<TArgs>(
				prefix + WorkName,
				ConvertArgs(ctx.Value),
				delegate(RpcClientContext context) {
					try {
						var rets = context.EndInvoke<TResults>();
						if (convertResults)
							ConvertResults(rets, ctx.Value);
						ctx.Return();
					} catch (Exception ex) {
						ctx.ThrowException(ex);
					}
				}
			);
		}

		protected abstract ResolvableUri GetUri(TContext context);

		protected abstract TArgs ConvertArgs(TContext context);

		protected abstract void ConvertResults(TResults results, TContext context);
	}
}
