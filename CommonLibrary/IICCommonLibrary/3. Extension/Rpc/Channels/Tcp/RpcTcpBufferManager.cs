using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpBufferManager
	{
		public const int SendBufferSizeMax = 256 * 1024;
		public const int P2Factor = 16;

		public static int SendBatchMax;
		public static int SendBufferSizeP2;
		public static int SendBufferSizeP1;
		public static int SendPendingMax;

		private static bool _inited = false;
		private static object _sync = new object();
		private static RpcTcpBufferCounter _counter = IICPerformanceCounterFactory.GetCounters<RpcTcpBufferCounter>();

		private static RpcTcpBufferPool<RpcTcpAsyncArgs> ReceivePool;
		private static RpcTcpBufferPool<RpcTcpAsyncArgs> AcceptPool;
		private static RpcTcpBufferPool<RpcTcpAsyncArgs> SendPool1;
		private static RpcTcpBufferPool<RpcTcpAsyncArgs> SendPool2;
		private static RpcTcpConfigSection _configuration;

		public static RpcTcpConfigSection Configuration
		{
			get { return _configuration; }
		}


		public static void Initialize()
		{
			if (!_inited) {
				lock (_sync) {
					if (_inited)
						return;

					_configuration = IICConfigurationManager.Configurator.GetConfigSecion<RpcTcpConfigSection>("RpcOverTcp", null);
					SendBatchMax = _configuration.SendItem.MaxBatch;
					SendPendingMax = _configuration.SendItem.MaxPending;

					GC.Collect();
					GC.Collect();
					GC.Collect();

					//TODO Configuration
					ReceivePool = new RpcTcpBufferPool<RpcTcpAsyncArgs>(_configuration.ReceiveItem.MaxConnections, 
						() => new RpcTcpAsyncArgs(_configuration.ReceiveItem.BufferSize, RpcTcpAsyncArgsType.Recv));

					AcceptPool = new RpcTcpBufferPool<RpcTcpAsyncArgs>(64, 
						() => new RpcTcpAsyncArgs(0, RpcTcpAsyncArgsType.Accept));

					SendPool1 = new RpcTcpBufferPool<RpcTcpAsyncArgs>(_configuration.SendItem.BufferCount,
						() => new RpcTcpAsyncArgs(_configuration.SendItem.BufferSize, RpcTcpAsyncArgsType.Send1));

					// 
					// 大缓冲区Buffer大小为小缓冲区的16倍, 数量为小缓冲区的1/16, 最小为4
					// 最大不超过256K
					SendBufferSizeP1 = _configuration.SendItem.BufferSize;
					SendBufferSizeP2 = _configuration.SendItem.BufferSize * P2Factor;

					if (SendBufferSizeP2 > SendBufferSizeMax)
						SendBufferSizeP2 = SendBufferSizeMax;

					int p2count = _configuration.SendItem.BufferCount / P2Factor;
					if (p2count == 0)
						p2count = 4;

					SendPool2 = new RpcTcpBufferPool<RpcTcpAsyncArgs>(p2count,
						() => new RpcTcpAsyncArgs(SendBufferSizeP2, RpcTcpAsyncArgsType.Send2));

					_counter.ReceiveTotal.SetRawValue(_configuration.ReceiveItem.MaxConnections);
					_counter.P1SendTotal.SetRawValue(_configuration.SendItem.BufferCount);
					_counter.P2SendTotal.SetRawValue(p2count);

					GC.Collect();
					GC.Collect();
					GC.Collect();
					_inited = true;
				}
			}
		}

		public static void DisposeAll()
		{
			ReceivePool.Dispose();
			AcceptPool.Dispose();
			SendPool1.Dispose();
			SendPool2.Dispose();
		}

		public static SocketAsyncEventArgs FetchAcceptArgs()
		{
			return AcceptPool.Fetch();
		}

		public static SocketAsyncEventArgs FetchReceiveArgs()
		{
			var e = ReceivePool.Fetch();
			if (e != null)
				_counter.Receiving.Increment();
			return e;
		}

		public static SocketAsyncEventArgs FetchSendArgs(int packetSize, int pendings)
		{

			//
			// 对于size期望的判断
			// 1. 最小buffer期望等于packetSize
			// 2. 最大buffer期望为max(MaxBatch, pendings) * packetSize
			pendings++;
			int expectSizeMin = packetSize;
			int expectSizeMax = packetSize * (pendings > SendBatchMax ? SendBatchMax : pendings);
			
			//
			// 选择BufferPool的规则
			// 1. expectSizeMin都太大的, 直接新分配
			SocketAsyncEventArgs e = null;
			if (expectSizeMin > SendBufferSizeP2) {
				e = null;
			} else {
				//
				// 2. 先试图满足expectSizeMax, 
				//    先尝试分配小的缓冲区, 再试图分配大的缓冲区
				if (expectSizeMax <= SendBufferSizeP1) {
					e = SendPool1.Fetch();
					if (e != null) {
						_counter.P1Sending.Increment();
					}
				} else if (expectSizeMax <= SendBufferSizeP2) {
					e = SendPool2.Fetch();
					if (e != null) {
						_counter.P2Sending.Increment();
					}
				}

				//
				// 3. 如果无法满足大的缓冲区, 则满足expectSizeMin 
				//    先尝试分配小的缓冲区, 再试图分配大的缓冲区
				if (e == null) {
					if (expectSizeMin <= SendBufferSizeP1) {
						e = SendPool1.Fetch();
						if (e != null) {
							_counter.P1Sending.Increment();
						}
					} else if (expectSizeMin <= SendBufferSizeP2) {
						e = SendPool2.Fetch();
						if (e != null) {
							_counter.P2Sending.Increment();
						}
					} 
				}
			}

			//
			// 4. 经过以上途径均无法满足的话, 重新分配缓冲区
			if (e != null) {
				_counter.HitRatio.IncreaseFraction(true);
			} else {
				//
				// 无法满足缓冲区需求分配新的缓冲区, 按照expectSizeMin
				_counter.HitRatio.IncreaseFraction(false);
				_counter.UnpooledSending.Increment();

				e = new SocketAsyncEventArgs();
				byte[] buffer = new byte[expectSizeMin];
				e.SetBuffer(buffer, 0, expectSizeMin);
			}
			return e;
		}

		public static void Release(SocketAsyncEventArgs e)
		{
			var t = e as RpcTcpAsyncArgs;
			if (t == null) {
				_counter.UnpooledSending.Decrement();
				e.Dispose();
			} else {
				switch (t.ArgsType) {
					case RpcTcpAsyncArgsType.Recv:
						_counter.Receiving.Decrement();
						ReceivePool.Release(t);
						break;
					case RpcTcpAsyncArgsType.Send1:
						_counter.P1Sending.Decrement();
						SendPool1.Release(t);
						break;
					case RpcTcpAsyncArgsType.Send2:
						_counter.P2Sending.Decrement();
						SendPool2.Release(t);
						break;
				}
			}
		}
	}
}
