/*
 * Rpc Tcp Channel 的核心部分
 * 
 * 管理RpcTcp连接的Request和Response的发送
 * 协议部分参照 http://research.feinno.com/trac/FAE/RpcOverTcp
 * 
 * 使用Socket的XxxAsync模式发送，
 *
 * 其中每条连接上，保持一个InputStream和一个OutputStream
 * 
 * 
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using Imps.Services.CommonV4.Rpc.ProtoContract;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///	RpcOverTcp的实际连接对象
	///	1. 发送接收Request, Response, 完成序列化
	///	2. 管理不超过MaxSendPendings的发送缓冲
	///	
	/// 状态变化:
	///		被创建后连接, 连接上以后, 如果被关闭, 则只能释放后重建连接		
	/// </summary>
	class RpcTcpSocketConnection : IDisposable
	{
		#region Static
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpSocketConnection));
		private static RpcTcpTransportCounter _counter = IICPerformanceCounterFactory.GetCounters<RpcTcpTransportCounter>();
		#endregion

		#region Const Values
		public const int MaxHeaderSize = 512;
		#endregion

		#region Private fields
		private int _connecting;		// 0: 未连接, 1: 正在连接
		private int _sending = 0;		// 0: 发送IO线程空闲, 1: 发送IO线程工作中
		private bool _connected;		// 是否已连接上
		private bool _closed;			// 是否已关闭, 被关闭后只能释放后重建连接
		private bool _releasing;		// 是否正处于最后的回收阶段
		private bool _recycling;		// 是否正处于回收阶段
		private DateTime _connectedTime;
		private object _syncSend = new object();
		private object _syncClose = new object();
		private Action<Exception> _connectCallback;
		private Queue<IRpcTcpSendingPacket> _pendingPackets;

		private Socket _socket;
		private TcpUri _remoteUri;
		private RpcConnectionDirection _direction;
		private SocketAsyncEventArgs _eRecv;			// 接收Buffer
		#endregion

		#region Public properties
		public event Action<RpcTcpSocketConnection> Disconnected;
		public event Action<RpcTcpSocketConnection, int, RpcRequest> RequestReceived;
		public event Action<RpcTcpSocketConnection, int, RpcResponse> ResponseReceived;

		public TcpUri RemoteUri
		{
			get { return _remoteUri; }
		}

		public bool Connected
		{
			get { return _connected; }
		}

		public bool Closed
		{
			get { return _closed; }
		}

		public RpcConnectionDirection Direction
		{
			get { return _direction; }
		}

		public DateTime ConnectedTime
		{
			get { return _connectedTime; }
		}
		
		public bool Recycling
		{
			get { return _recycling; }
			set { _recycling = value; }
		}
		#endregion

		#region Public methods
		public RpcTcpSocketConnection(RpcConnectionDirection direction)
		{
			_eRecv = RpcTcpBufferManager.FetchReceiveArgs();
			if (_eRecv == null)
				throw new Exception("Exceed max connections!");

			((RpcTcpAsyncArgs)_eRecv).Callback = (e) => ProcessReceive(e);

			_pendingPackets = new Queue<IRpcTcpSendingPacket>();
			_direction = direction;
			_connected = false;
			_closed = false;
			_recycling = false;

			_counter.Connections.Increment();
		}

		public void Accept(SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success) {
				throw new Exception("Accept Failed:" + e.SocketError);
			} else {
				_socket = e.AcceptSocket;
				IPEndPoint ep = (IPEndPoint)_socket.RemoteEndPoint;
				_remoteUri = new TcpUri(ep.Address, ep.Port);
				_connecting = 0;
				_connected = true;
				_connectedTime = DateTime.Now;
				BeginReceive();
			}
		}

		public void BeginConnect(TcpUri uri, Action<Exception> callback, bool enableKeepalive)
		{
			if (_connected)
				return;

			_counter.ConnectionsPending.Increment();

			if (Interlocked.CompareExchange(ref _connecting, 1, 0) == 0) {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_remoteUri = uri;
				_connectCallback = callback;

				if (enableKeepalive)
					EnableKeepAlive(150000, 1000);

				SocketAsyncEventArgs e = new SocketAsyncEventArgs();
				e.RemoteEndPoint = new IPEndPoint(uri.Address, uri.Port);
				e.Completed += new EventHandler<SocketAsyncEventArgs>(
					(sender, e2) => ProcessConnect(e2)
				);

				if (!_socket.ConnectAsync(e)) {
					ProcessConnect(e);
				}
			}
		}

		/// <summary>
		///		发送一个请求
		/// </summary>
		/// <param name="tx"></param>
		public void Send(IRpcTcpSendingPacket tx)
		{
			if (_closed) {
				ProcessSendFailed(new IRpcTcpSendingPacket[] { tx } , RpcErrorCode.SendFailed, null);
			} else {
				if (_connected && Interlocked.CompareExchange(ref _sending, 1, 0) == 0) {
					SendPackets(tx);
				} else {
					if (_pendingPackets.Count > RpcTcpBufferManager.SendPendingMax) {
						ProcessSendFailed(new IRpcTcpSendingPacket[] { tx } , RpcErrorCode.SendPending, null);
					} else {
						lock (_syncSend) {
							_pendingPackets.Enqueue(tx);
						}
						_counter.SendPending.Increment();
					}
				}
			}
		}

		/// <summary>
		///		断开连接，这个方法不会抛出异常
		/// </summary>
		/// <param name="err"></param>
		public void Disconnect(SocketError err)
		{
			string msg = string.Format("Socket error {0}-{1} with {2}", (int)err, err, _remoteUri);
			try {
				Close(new Exception(msg));
			} catch (Exception ex2) {
				_tracing.ErrorFmt(ex2, "RpcTcpSocketConnection.Disconnect(err,action) Failed");
			}
		}

		/// <summary>
		///		断开连接，这个方法不会抛出异常
		/// </summary>
		/// <param name="ex"></param>
        /// <param name ="action"></param>
		public void Disconnect(Exception ex, string action)
		{
			_tracing.ErrorFmt(ex, "Disconnected with {0} in {1}", _remoteUri, action);
			try {
				Close(ex);
			} catch (Exception ex2) {
				_tracing.ErrorFmt(ex2, "RpcTcpSocketConnection.Disconnect(ex,action) Failed");
			}
		}

		/// <summary>
		///		断开连接，这个方法不会抛出异常
		/// </summary>
		public void Disconnect()
		{
			try {
				_tracing.InfoFmt("Disconnected with {0}", _remoteUri);
				Close(null);
			} catch (Exception ex2) {
				_tracing.ErrorFmt(ex2, "RpcTcpSocketConnection.Disconnect() Failed");
			}
		}

		/// <summary>
		///		断开连接，这个方法不会抛出异常
		/// </summary>
		/// <param name="err"></param>
		public void Close(Exception err)
		{
			if (_closed) {
				return;
			} else {
				lock (_syncClose) {
					if (_closed) {
						return;
					} else {
						_closed = true;
						_connected = false;
					}
				}
			}

			_counter.Connections.Decrement();
			if (_socket.Connected)
				_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();
			_releasing = true;

			var l = new List<IRpcTcpSendingPacket>();
			lock (_syncSend) {
				while (_pendingPackets.Count > 0) {
					l.Add(_pendingPackets.Dequeue());
				}
				_pendingPackets.Clear();
			}

			if (l.Count > 0) {
				_counter.SendPending.IncrementBy(- l.Count);
				ProcessSendFailed(l, RpcErrorCode.SendFailed, err);
			}

			if (_eRecv != null) {
				//
				// TODO: 应该有更优雅的实现方式
                // 最好 _eRecv有一个opertion的标志位， 如果正在recieve中，就在receive返回后Release(_eRecv),否则在此处release
				Thread.Sleep(500);
				RpcTcpBufferManager.Release(_eRecv);
			}

			if (err != null) {
				_counter.CorruptedTransmissions.Increment();
			}

			if (Disconnected != null) {
				Disconnected(this);
			}
		}

		public void Dispose()
		{
			Disconnect();
		}
		#endregion

		#region Private methods
		private void ProcessConnect(SocketAsyncEventArgs e)
		{
			_counter.ConnectionsPending.Decrement();
			Exception cex = null;
			try {
				if (e.SocketError == SocketError.Success) {
					_connecting = 0;
					_connected = true;
					_connectedTime = DateTime.Now;

					BeginReceive();
					if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0) {
						SendPackets(null);
					}
				} else {
					cex = new Exception("Connect failed " + e.SocketError);
					Disconnect(e.SocketError);
				}
			} catch (Exception ex) {
				cex = ex;
				Disconnect(ex, "ProcessConnect");
			} finally {
				e.Dispose();
			}
			_connectCallback(cex);
		}

		private void BeginReceive()
		{
			_eRecv.SetBuffer(0, _eRecv.Buffer.Length);
			if (!_socket.ReceiveAsync(_eRecv)) {
				ProcessReceive(_eRecv);
			}
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			/*
			 * 接收策略: 
			 *	总是尝试收满一个缓冲区, 
			 *	如果到N个字节, 先尝试填充一个Packet, 
			 *		分为两步, 填充头和填充体
			 *		
			 *	上一个没收完的Packet会放在e.UserToken中
			 *	
			 */
			byte[] buffer = null;
			int offset = 0;
			int count = 0;
			RpcTcpPacket packet = null;

			try {
				if (e.BytesTransferred == 0) {
					Disconnect();
				} else if (e.SocketError != SocketError.Success) {
					Disconnect(e.SocketError);
				} else {
					_counter.ReceivePerSec.Increment();
					_counter.ReceiveTotal.Increment();
					_counter.ReceiveBytesPerSec.IncrementBy(e.BytesTransferred);
					_counter.ReceiveBytesTotal.IncrementBy(e.BytesTransferred);

					_tracing.InfoFmt("Socket Receive {0} Bytes", e.BytesTransferred);

					packet = (RpcTcpPacket)e.UserToken;
					buffer = e.Buffer;
					offset = e.Offset;
					count = e.BytesTransferred;

					while (count > 0) {
						// 从头接收一个包
						if (packet == null) {
							if (count >= RpcTcpPacket.IdentityLength) {
								//_tracing.InfoFmt("Receive Header {0} {1}", offset, RpcTcpPacket.IdentityLength);
								packet = new RpcTcpPacket(buffer, offset, RpcTcpPacket.IdentityLength);
								offset += RpcTcpPacket.IdentityLength;
								count -= RpcTcpPacket.IdentityLength;
							} else {
								// 缓冲区耗尽, 跳出
								//_tracing.InfoFmt("Receive Header Empty {0} {1}", offset, count);
								packet = new RpcTcpPacket(buffer, offset, count);
								break;
							}
						}
						// 如果上一个包没收完, 则先接受完包头
						int identityNeed = packet.IdentityNeed;
						if (identityNeed > 0) {
							if (count >= identityNeed) {
								packet.FillIdentity(buffer, offset, identityNeed);
								offset += identityNeed;
								count -= identityNeed;
							} else {
								// 缓冲区耗尽, 跳出
								packet.FillIdentity(buffer, offset, count);
								break;
							}
						}
						// 继续读包体
						int nextRecvSize = packet.NextRecvSize;
						if (count >= nextRecvSize) {
							packet.FillNext(buffer, offset, nextRecvSize);
							offset += nextRecvSize;
							count -= nextRecvSize;
							ProcessReceivePacket(packet);
							packet = null; // 置空,读下一个包
						} else {
							// 缓冲区耗尽,跳出
							packet.FillNext(buffer, offset, count);
							break;
						}
					}

					//
					// 将未接受完的包放到上下文中, 继续接收下一个
					e.UserToken = packet;
					BeginReceive();
				}
			} catch (Exception ex) {
				if (buffer != null) {
					StringBuilder str = new StringBuilder();
					for (int i = offset; i < count; i++) {
						str.AppendFormat("{0:X2}", buffer[offset + i]);
						if ((i - offset) % 100 == 0) {
							str.Append("\r\n");
						}
					}
					_tracing.ErrorFmt("ProcessReceive buffer parse failed!:\r\n{0}\r\n with {1}",
						str.ToString(),
						packet == null ? "" : packet.DumpInfo());
				}

				Disconnect(ex, "ProcessReceive");
			}
		}

		private void ProcessReceivePacket(RpcTcpPacket packet)
		{
			_counter.ReceiveMessagePerSec.Increment();
			_counter.ReceiveMessageTotal.Increment();

			if (packet.Direction == RpcMessageDirection.Request) {
				ThreadPool.QueueUserWorkItem(
					new WaitCallback(delegate(object state) {
					try {
						RpcTcpMessage<RpcRequest> request = packet.GetRequest();
						RequestReceived(this, request.Sequence, request.Message);
					} catch (Exception ex) {
						_tracing.Error(ex, "ProcessRequest Failed");
						_tracing.ErrorFmt("Packet: {0}", packet.DumpInfo());
					}
				})
				);
			} else {
				ThreadPool.QueueUserWorkItem(
					new WaitCallback(delegate(object state) {
					try {
						RpcTcpMessage<RpcResponse> response = packet.GetResponse();
						ResponseReceived(this, response.Sequence, response.Message);
					} catch (Exception ex) {
						_tracing.Error(ex, "ProcessResponse Failed");
						_tracing.ErrorFmt("Packet: {0}", packet.DumpInfo());
					}
				})
				);
			}
		}
		#endregion

		#region Send Implementation
		/// <summary>
		///		发送报文
		///		如果sendFirst == null, 则优先从queue中dequeue
		///		否则, 先取出sendFirst, 然后再从queue中取
		///		这是一个回调链，完全靠回调组织起来的，当_sending = 1时，存在一个回调链
		/// </summary>
		/// <param name="sendFirst"></param>
		private void SendPackets(IRpcTcpSendingPacket sendFirst)
		{
			int size;
			SocketAsyncEventArgs eSend = null;
			List<IRpcTcpSendingPacket> batch = new List<IRpcTcpSendingPacket>();

			//
			// 阶段1. 获得第一个能够发送的报文
			try {
				//
				// 先至少放置一个报文进去
				if (sendFirst != null) {
					batch.Add(sendFirst);
				} else {
					if (_pendingPackets.Count == 0) {
						//
						// 回调链的最终返回，只有当Queue完全清空以后，置空标记位
						_sending = 0;
						return;
					} else {
						lock (_syncSend) {
							batch.Add(_pendingPackets.Dequeue());
						}
						_counter.SendPending.Decrement();
					}
				}

				//
				// 获取第一个包的大小, 并估算对缓冲区的大小的需求
				size = GetExpectSize(batch[0]);

			} catch (Exception ex) {
				//
				// 在上面的代码块中如果出现异常，则本次发送失败
				_tracing.ErrorFmt(ex, "SendPackets 1 failed");
				ProcessSendFailed(batch, RpcErrorCode.SendFailed, ex);

				// 
				// 扔出异常会导致连接断开，所以直接引发下一个调用链
				SendPackets(null);	
				return;
			}


			//
			// 阶段2. 获取缓冲区，并写入数据
			try {
				// 
				// 创建缓冲区, 如果过大会建立新缓冲区
				eSend = RpcTcpBufferManager.FetchSendArgs(size, _pendingPackets.Count);

				MemoryStream stream = new MemoryStream(eSend.Buffer, 0, eSend.Buffer.Length, true, true);
				int bufferRemain = eSend.Buffer.Length;

				//
				// 写入第一个报文
				TryWriteBuffer(batch[0], stream, bufferRemain);
				bufferRemain = bufferRemain - (int)stream.Position;

				//
				// 尝试向缓冲区内放置能够发送出去的最多的报文
				IRpcTcpSendingPacket packet = null;
				while (true) {
					if (_pendingPackets.Count > 0) {
						lock (_syncSend) {
							if (_pendingPackets.Count > 0) {
								packet = _pendingPackets.Dequeue();
							}
						}
						_counter.SendPending.Decrement();
					} else {
						break;
					}
					int offsetBefore = (int)stream.Position;
					if (TryWriteBuffer(packet, stream, bufferRemain)) {
						batch.Add(packet);
						packet = null;
					} else {
						break;
					}
					int offsetAfter = (int)stream.Position;
					bufferRemain = bufferRemain - (offsetAfter - offsetBefore);
				}

				int bufferSize = (int)stream.Position;
				_counter.SendPerSec.Increment();
				_counter.SendTotal.Increment();

				_counter.SendMessageTotal.IncrementBy(batch.Count);
				_counter.SendMessagePerSec.IncrementBy(batch.Count);

				_counter.SendBytesPerSec.IncrementBy(bufferSize);
				_counter.SendBytesTotal.IncrementBy(bufferSize);

				eSend.UserToken = new SendingContext() {
					NextPacket = packet,
					SendingBatch = batch,
				};

				if (eSend is RpcTcpAsyncArgs) {
					((RpcTcpAsyncArgs)eSend).Callback = delegate(SocketAsyncEventArgs e) {
						ProcessSend(e);
					};
				} else {
					eSend.Completed += new EventHandler<SocketAsyncEventArgs>(
						(sender, e) => ProcessSend(e)
					);
				}

				eSend.SetBuffer(0, bufferSize);
			} catch (Exception ex) { 
				//
				// 无论如何，先交回缓冲区
				RpcTcpBufferManager.Release(eSend);

				//
				// 在组织缓冲区的代码块中如果出现异常，则本次发送失败，整批失败
				_tracing.ErrorFmt(ex, "SendPackets 2 failed");
				ProcessSendFailed(batch, RpcErrorCode.SendFailed, ex);

				// 
				// 扔出异常会导致连接断开，所以直接引发下一个调用链
				SendPackets(null);	
				return;
			}

			if (!_socket.SendAsync(eSend)) {
				ProcessSend(eSend);
			}
		}

		private int GetExpectSize(IRpcTcpSendingPacket packet)
		{
			int bodySize = 0;
			if (packet.BodyBuffer != null) {
				bodySize = packet.BodyBuffer.GetSize();
			}
			return bodySize + MaxHeaderSize;
		}

		private bool TryWriteBuffer(IRpcTcpSendingPacket packet, Stream stream, int remain)
		{
			int bodySize = 0;
			if (packet.BodyBuffer != null) {
				bodySize = packet.BodyBuffer.GetSize();
			}
			if (bodySize + MaxHeaderSize > remain) {
				_tracing.InfoFmt("Write Packet Not Enough {0} < {1}", bodySize, remain);
				return false;
			} else {
				long begin = stream.Position;
				RpcTcpPacket.WriteMessage(stream, packet);
				_tracing.InfoFmt("Write Packet OK {0} remain {1}", stream.Position - begin, remain);
				return true;
			}
		}

		private void ProcessSend(SocketAsyncEventArgs e)
		{
			SendingContext sctx = null;
			try {
				sctx = (SendingContext)e.UserToken;
				if (e.SocketError != SocketError.Success) {
					var err = e.SocketError;
					ProcessSendFailed(sctx.SendingBatch, RpcErrorCode.SendFailed, new Exception("Send Body Failed: " + err));
					Disconnect(e.SocketError);
					RpcTcpBufferManager.Release(e);
					e = null;
				} else {
					_tracing.InfoFmt("Socket Send {0} Bytes", e.Count);
					RpcTcpBufferManager.Release(e);
					e = null;
					SendPackets(sctx.NextPacket);
				}
			} catch (Exception ex) {
				_tracing.ErrorFmt(ex, "ProcessSend Failed {0}:{1}", e, e.Buffer.Length);
				Disconnect(ex, "ProcessSend");
			} finally {
				//
				// 保护，如果出现了异常，则在此处释放
				if (e != null)
					RpcTcpBufferManager.Release(e);
			}
		}

		private void ProcessSendFailed(IEnumerable<IRpcTcpSendingPacket> packets, RpcErrorCode code, Exception ex) 
		{
			foreach (var p in packets) {
				_counter.SendFailed.Increment();
				try {
					p.SendFailed(RpcErrorCode.SendFailed, ex);
				} catch (Exception e) {
					_tracing.Error(e, "ProcessSendFailed error");
				}
			}
		}

		private void EnableKeepAlive(int timeout, int interval)
		{
			byte[] kaBuffer = new byte[12];

			kaBuffer[0] = (byte)0x01;
			int value = timeout;
			for (int i = 4; value != 0 && i < 8; ++i) {
				kaBuffer[i] = (byte)(value & 0xFF);
				value >>= 8;
			}
			value = interval;
			for (int i = 8; value != 0 && i < 12; ++i) {
				kaBuffer[i] = (byte)(value & 0xFF);
				value >>= 8;
			}
			_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, kaBuffer);
		}
		#endregion

		private class SendingContext
		{
			public IRpcTcpSendingPacket NextPacket;
			public List<IRpcTcpSendingPacket> SendingBatch;
		}
	}
}
