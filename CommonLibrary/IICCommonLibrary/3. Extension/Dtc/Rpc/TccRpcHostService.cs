using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	[RpcService("TccRpcHostService", ClientChecking = false)]
	public interface ITccRpcHostService
	{

	}

	public class TccRpcHostService: RpcServiceBase
	{
		private static TccRpcHostService _instance = new TccRpcHostService();
		private Dictionary<string, ITccRpcHostUnit> _units = new Dictionary<string, ITccRpcHostUnit>();

		public TccRpcHostService()
			: base("TccRpcHostService")
		{
		}

		public static void Initialize()
		{
			RpcServiceManager.RegisterRawService(_instance);
		}

		public static void RegisterWorkUnit(ITccRpcHostUnit unit)
		{
			_instance._units.Add(unit.WorkUnitName, unit);
		}

		public override void OnTransactionStart(RpcServerContext context)
		{
			string m = context.MethodName;
			string unitName;
			TccAction action;
			if (m.StartsWith("try_")) {
				action = TccAction.Try;
				unitName = m.Substring("try_".Length);
			} else if (m.StartsWith("confirm_")) {
				action = TccAction.Confirm;
				unitName = m.Substring("confirm_".Length);
			} else if (m.StartsWith("cancel_")) {
				action = TccAction.Cancel;
				unitName = m.Substring("cancel_".Length);
			} else {
				context.ReturnError(RpcErrorCode.MethodNotFound, null);
				return;
			}

			ITccRpcHostUnit unit;
			if (_instance._units.TryGetValue(unitName, out unit)) {
				unit.Invoke(context, action);
			} else {
				context.ReturnError(RpcErrorCode.MethodNotFound, null);
			}
		}
	}
}
