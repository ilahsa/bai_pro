using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	static class RpcTcpTransactionManager
	{
		private static object _syncRoot = new object();
		private static SessionPool<RpcTcpClientTransaction> _txPool;
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpTransactionManager));
		private static Thread _monitorThread;
		private static int _currentTicks;

		public static void Initialize()
		{
			if (_txPool == null) {
				lock (_syncRoot) {
					if (_txPool == null) {
						_txPool = new SessionPool<RpcTcpClientTransaction>(640 * 1024);
						_monitorThread = new Thread(MonitorProc);
						_monitorThread.Name = "RpcTcpTransactionManager.MonitorThread";
						_monitorThread.IsBackground = true;

						_monitorThread.Start();
						_currentTicks = Environment.TickCount;
						// RpcTcpClientPerfCounter.Instance.ActiveTransactions.SetRawValue(0);
					}
				}
			}
		}

		public static RpcTcpClientTransaction CreateClientTransaction(ServerUri serverUri, RpcTcpSocketConnection sock, RpcRequest request)
		{
			var tx = new RpcTcpClientTransaction(serverUri, sock, request);
			return tx;
		}

		public static void BeginTransaction(RpcTcpClientTransaction tx)
		{
			int seq = _txPool.Add(tx);
			if (seq < 0) {
				var resp = RpcResponse.Create(
					RpcErrorCode.SendFailed,
					new RpcException(RpcErrorCode.SendFailed, tx.ServiceUrl, "session pool is full!", null)
				);
				tx.Callback(resp);
				return;
			}
			tx.Sequence = seq;
			tx.Ticks = _currentTicks;
		}

		public static void EndTransaction(int seq, RpcResponse response)
		{
			var tx = _txPool[seq];
			if (tx != null) {
				tx.Callback(response);
				_txPool.Remove(tx.Sequence);
			}
		}

		private static void MonitorProc()
		{
			while (true) {
				try {
					//
					// 总觉着1s太短，为cpu感到累得慌，忍不住改了 
					Thread.Sleep(5 * 1000);

					_currentTicks = Environment.TickCount;
					foreach (var k in _txPool.Items) {
						var tx = k.Value;
						if (tx == null)
							continue;

						//
						// 判断链接已经断开
						if (tx.GetSocket().Closed) {
							//
							// 无论怎么样，先移走
							_txPool.Remove(k.Key);
							try {
								tx.Callback(RpcResponse.Create(RpcErrorCode.ConnectionBroken, null));
								_tracing.ErrorFmt("ConnectionBroken in {0} begin {1}: tx: {2}", 
									tx.GetSocket().RemoteUri, 
									tx.GetSocket().ConnectedTime, 
									ObjectHelper.DumpObject(tx));
							} catch (Exception ex) {
								_tracing.ErrorFmt(ex, "RpcTcpTransactionManager.MonitorProc Failed");
							}
						}

						//
						// 判断事务超时
						if (tx.Timeout > 0 && (_currentTicks - tx.Ticks) > tx.Timeout) {
							//
							// 无论怎么样，先移走
							// 原先如果Callback出了exception的话，会没有移走value，再次循环到此还是一样的情况，pool中value会累积
							_txPool.Remove(k.Key);
							try {
								k.Value.Callback(RpcResponse.Create(RpcErrorCode.TransactionTimeout, null));
							} catch (Exception ex) {
								_tracing.ErrorFmt(ex, "RpcTcpTransactionManager.MonitorProc Failed");
							}
						}
					}
					//RpcTcpClientPerfCounter.Instance.ActiveTransactions.SetRawValue(_txPool.Count);
				} catch (ThreadAbortException) {
					Thread.ResetAbort();
					return;
				} catch (Exception ex) {
					_tracing.ErrorFmt(ex, "RpcTcpTransactionManager.MonitorProc Failed");
				}
			}
		}
	}
}
