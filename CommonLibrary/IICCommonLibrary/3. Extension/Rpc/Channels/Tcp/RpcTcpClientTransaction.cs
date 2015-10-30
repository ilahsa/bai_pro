/*
 * RpcTcp模式下的 RpcClientTransaction
 *
 * 
 * GaoLei 2010-06-24
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpClientTransaction : RpcClientTransaction, IRpcTcpSendingPacket
	{
		private RpcTcpSocketConnection _sock;
		private Action _callback;

		public int Sequence;
		public int Timeout;
		public int Ticks;

		internal RpcTcpClientTransaction(ServerUri uri, RpcTcpSocketConnection sock, RpcRequest request)
			: base(uri, request)
		{
			_sock = sock;
		}

		public bool HasBody
		{
			get { return Request.BodyBuffer != null; }
		}

		public void SendFailed(RpcErrorCode code, Exception ex)
		{
            Response = RpcResponse.Create(code, ex);
            RpcTcpTransactionManager.EndTransaction(Sequence, Response);
		}

		public void SerializeBody(Stream stream)
		{
			Request.BodyBuffer.WriteToStream(stream);
		}

		public override void SendRequest(Action callback, int timeout)
		{
			_callback = callback;
			if (_sock == null) {
				Callback(RpcResponse.Create(RpcErrorCode.ConnectionPending, null));
			} else {
				Timeout = timeout;
				RpcTcpTransactionManager.BeginTransaction(this);
				_sock.Send(this);
			}
		}

		public void Callback(RpcResponse response)
		{
			Response = response;
			_callback();
		}

        public RpcTcpSocketConnection GetSocket()
        {
            return _sock;
        }

		#region Interface IRpcTcpSendingPacket
		public RpcMessageDirection Direction
		{
			get { return RpcMessageDirection.Request; }
		}

		public ProtoContract.RpcRequestHeader RequestHeader
		{
			get {
				return new ProtoContract.RpcRequestHeader() {
					FromComputer = Request.FromComputer,
					FromService = Request.FromService,
					Service = Request.Service,
					Method = Request.Method,
					ContextUri = Request.ContextUri,
					Option = (int)Request.Options,
					Sequence = this.Sequence,
				};
			}
		}

		public ProtoContract.RpcResponseHeader ResponseHeader
		{
			get { throw new NotImplementedException(); }
		}

		public RpcBodyBuffer BodyBuffer
		{
			get { return Request.BodyBuffer; }
		}
		#endregion
	}
}
