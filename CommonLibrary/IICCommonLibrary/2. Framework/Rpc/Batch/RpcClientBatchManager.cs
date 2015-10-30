using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcClientBatchManager
	{
		private object _syncRoot = new object();
		private ParallelQueue<KeyWrapper<ServerUri, RpcConnection>, RpcBatchClientTransaction> _queue;

		public RpcClientBatchManager(int batchCount, int idleMs)
		{
            _queue = new ParallelQueue<KeyWrapper<ServerUri, RpcConnection>, RpcBatchClientTransaction>(
				"RpcBatchClientMamager", batchCount, idleMs, BatchDequeueProc);
		}

		public RpcBatchClientTransaction CreateTransaction(ServerUri uri, RpcRequest request)
		{
			RpcBatchClientTransaction tx = new RpcBatchClientTransaction(uri, request, this);
			return tx;
		}

		public void EnqueueTransaction(RpcBatchClientTransaction tx)
		{
            _queue.Enqueue(new KeyWrapper<ServerUri, RpcConnection>(tx.ServerUri, null), tx);
		}

        private void BatchDequeueProc(KeyWrapper<ServerUri, RpcConnection> key, RpcBatchClientTransaction[] txs)
		{
			RpcBatchRequest[] requests = new RpcBatchRequest[txs.Length];

			for (int i = 0; i < txs.Length; i++) {
				requests[i] = txs[i].BatchRequest;
			}

			RpcRequest request = new RpcRequest(txs[0].Request.Service, txs[0].Request.Method, null);
			request.BodyBuffer = new RpcBodyBuffer<RpcBatchRequest[]>(requests);

			if (key.Token == null) {
				lock (_syncRoot) {
					key.Token = RpcProxyFactory.GetConnection(key.Key, txs[0].ServiceRole);
				}
			}

			RpcClientTransaction trans = key.Token.CreateTransaction(request);
			RpcClientContext ctx = new RpcClientContext(trans);

			ctx.SendRequest(
				delegate(long elapseTicks, RpcClientContext c2, bool successed) {
					try {
						RpcBatchResponse[] resps = ctx.EndInvoke<RpcBatchResponse[]>();
						if (resps.Length != txs.Length) {
							ProcessFailedTxs(txs, RpcErrorCode.InvaildResponseArgs, new Exception("Batch Length NotMatch"));
						} else {
							for (int i = 0; i < resps.Length; i++) {
								txs[i].BatchResponse = resps[i];
								txs[i].OnTransactionEnd();
							}
						}
					} catch (RpcException ex) {
						ProcessFailedTxs(txs, ex.RpcCode, ex.InnerException);
					} catch (Exception ex) {
						ProcessFailedTxs(txs, RpcErrorCode.ServerError, ex);
					}
				},
				-1
			);
		}

		private void ProcessFailedTxs(RpcBatchClientTransaction[] trans, RpcErrorCode code, Exception ex)
		{
			foreach (var tx in trans) {
				tx.Response = RpcResponse.Create(code, ex);
				tx.OnTransactionEnd();
			}
		}
	}
}
