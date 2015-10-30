using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Dtc
{
	public interface ITccRpcHostUnit
	{
		string WorkUnitName { get; }
		void Invoke(RpcServerContext context, TccAction action);
	}

	public abstract class TccRpcHostUnit<TArgs, TResults>: ITccRpcHostUnit
	{
		private string _workUnitName;

		public TccRpcHostUnit(string workUnitName)
		{
			_workUnitName = workUnitName;
		}

		public string WorkUnitName
		{
			get { return _workUnitName; }
		}

		void ITccRpcHostUnit.Invoke(RpcServerContext context, TccAction action)
		{
			var ctx = new TccRpcContext<TArgs, TResults>(context);

			switch (action) {
				case TccAction.Try:
					Try(ctx);
					break;
				case TccAction.Confirm:
					Confirm(ctx);
					break;
				case TccAction.Cancel:
					Cancel(ctx);
					break;
				default:
					throw new NotSupportedException();
			}
		}

		protected abstract void Try(TccRpcContext<TArgs, TResults> context);
		protected abstract void Confirm(TccRpcContext<TArgs, TResults> context);
		protected abstract void Cancel(TccRpcContext<TArgs, TResults> context);
	}
}
