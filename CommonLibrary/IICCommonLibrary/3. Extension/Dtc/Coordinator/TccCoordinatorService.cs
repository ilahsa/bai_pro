using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	[RpcService("TccCoordinatorService")]
	public interface ITccCoordinatorService
	{
		[RpcServiceMethod(ArgsType = typeof(string), ResultType = typeof(RpcList<TccTransactionData>))]
		void ListActiveTransactions(RpcServerContext ctx);

        [RpcServiceMethod(ArgsType = typeof(string), ResultType = typeof(RpcList<TccTransactionData>))]
		void ListFailedTransactions(RpcServerContext ctx);
	}

    public class TccCoordinatorService<TContext> : ITccCoordinatorService where TContext : ITccContext
	{
        private TccCoordinator<TContext> _coordinator;

        public TccCoordinatorService(TccCoordinator<TContext> coordinator)
        {
            _coordinator = coordinator;
        }

	    #region ITccCoordinatorService Members

        public void  ListActiveTransactions(RpcServerContext ctx)
        {
            if (_coordinator == null)
                throw new Exception("Coordinator is Null");
            string transName = ctx.GetArgs<string>();
            if (transName != _coordinator.TransName)
                throw new Exception("TransName not Matched");
            List<TccTransactionData> dataList = (List<TccTransactionData>)_coordinator.GetActiveTransactionDatas();
            RpcList<TccTransactionData> rpcList = null;
            if (dataList != null && dataList.Count > 0)
            {
                rpcList = new RpcList<TccTransactionData>();
                foreach (TccTransactionData data in dataList)
                    rpcList.Value.Add(data);
            }
            ctx.Return<RpcList<TccTransactionData>>(rpcList);
        }

        public void  ListFailedTransactions(RpcServerContext ctx)
        {
            if (_coordinator == null)
                throw new Exception("Coordinator is Null");
            string transName = ctx.GetArgs<string>();
            if (transName != _coordinator.TransName)
                throw new Exception("TransName not Matched");
            List<TccTransactionData> dataList = (List<TccTransactionData>)_coordinator.GetFailedTransactions();
            RpcList<TccTransactionData> rpcList = null;
            if (dataList != null && dataList.Count > 0)
            {
                rpcList = new RpcList<TccTransactionData>();
                foreach (TccTransactionData data in dataList)
                    rpcList.Value.Add(data);
            }
            ctx.Return<RpcList<TccTransactionData>>(rpcList);
        }

        #endregion
    }
}