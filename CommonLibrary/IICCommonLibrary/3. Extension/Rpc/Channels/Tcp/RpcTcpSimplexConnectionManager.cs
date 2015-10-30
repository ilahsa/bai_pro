using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	static class RpcTcpSimplexConnectionManager
	{
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpSimplexConnectionManager));
		private static RpcTcpTransportCounter _counter = IICPerformanceCounterFactory.GetCounters<RpcTcpTransportCounter>();

		private static int _delayCloseSeconds;
		private static Thread _thread;
		private static Dictionary<RpcTcpSimplexConnectionWrapper, int> _connections;
		private static Queue<ComboClass<DateTime, RpcTcpSocketConnection>> _closeQueue;
		private static Object _syncRoot;
		

		public static void Initialize()
		{
			_syncRoot = new object();
			_connections = new Dictionary<RpcTcpSimplexConnectionWrapper, int>();
			_closeQueue = new Queue<ComboClass<DateTime, RpcTcpSocketConnection>>();
			_delayCloseSeconds = RpcTcpBufferManager.Configuration.ChannelItem.Timeout / 1000 + 10;

			_thread = new Thread(RecycleProc);
			_thread.Name = "RpcTcpSimplexConnectionManager.RecycleProc";
			_thread.IsBackground = true;
			_thread.Start();
		}

		/// <summary>
		///		延时关闭Socket, 保证所有的事务都执行完毕
		/// </summary>
		/// <param name="sock"></param>
		public static void DelayClose(RpcTcpSocketConnection sock)
		{
			DateTime t = DateTime.Now.AddSeconds(_delayCloseSeconds);
			_tracing.InfoFmt("enqueue delay close connection: {0} in {1}", sock.RemoteUri, t);
			_counter.ConnectionsRecycled.Increment();
			sock.Recycling = true;
			lock (_syncRoot) {
				_closeQueue.Enqueue(new ComboClass<DateTime, RpcTcpSocketConnection>(t, sock));
			}
		}

		public static void AddConnection(RpcTcpSimplexConnectionWrapper wrapper)
		{
			lock (_syncRoot) {
				_tracing.InfoFmt("add connection {0}", wrapper.GetInfo());
				_connections.Add(wrapper, 0);
			}
		}

		public static void RemoveConnection(RpcTcpSimplexConnectionWrapper wrapper)
		{
			lock (_syncRoot) {
				_connections.Remove(wrapper);
			}
		}

		private static void RecycleProc()
		{
			while (true) {
				try {
					Thread.Sleep(1000);

					List<RpcTcpSimplexConnectionWrapper> keys = new List<RpcTcpSimplexConnectionWrapper>();
					lock (_syncRoot) {
						foreach (var k in _connections)
							keys.Add(k.Key);
					}

					//
					// 尝试回收一个连接
					foreach (var k in keys) {
						k.TryRecycle();
					}

					//
					// 关闭延时Queue中的Socket
					while (_closeQueue.Count > 0) {
						ComboClass<DateTime, RpcTcpSocketConnection> a;
						lock (_syncRoot) {
							a = _closeQueue.Peek();
							if (DateTime.Now > a.V1) {
								a = _closeQueue.Dequeue();
							} else {
								break;
							}
						}
						try {
							RpcTcpSocketConnection sock = a.V2;
							_tracing.InfoFmt("delay close connection: {0}", sock.RemoteUri);
							sock.Disconnect();
							_tracing.InfoFmt("delay close connection ok {0}", sock.RemoteUri);
						} catch (Exception ex) {
							_tracing.Error(ex, "delay close connection failed");
						}
					}
				} catch (ThreadAbortException) {
					Thread.ResetAbort();
				} catch (Exception ex) {
					_tracing.Error(ex, "RecycleProc Failed");
				}
			}
		}
	}
}
