using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
    class RpcTcpServerTransaction : RpcServerTransaction, IRpcTcpSendingPacket
    {
		private RpcTcpSocketConnection _sock;
		private RpcResponse _response;
		public int Sequence;

		public RpcTcpServerTransaction(RpcServerChannel channel, RpcConnection conn, RpcTcpSocketConnection socket, RpcRequest request, int sequence)
			: base(channel, conn, request)
		{
			_sock = socket;
			Sequence = sequence;
		}	

		public override void SendResponse(RpcResponse response)
		{
			_response = response;
			if (response.BodyBuffer != null) {
				response.BodyBuffer.TextError = true;
			}
			_sock.Send(this);
		}

		#region Interface IRpcTcpPendingPackets
		public RpcMessageDirection Direction
		{
			get { return RpcMessageDirection.Response; }
		}

		public ProtoContract.RpcRequestHeader RequestHeader
		{
			get { throw new NotSupportedException("never used"); }
		}

		public ProtoContract.RpcResponseHeader ResponseHeader
		{
			get {
				return new ProtoContract.RpcResponseHeader() {
					Sequence = this.Sequence,
					ResponseCode = (int)_response.ErrorCode,
					Option = (int)_response.Options
				};
			}
		}

		public RpcBodyBuffer BodyBuffer
		{
			get { return _response.BodyBuffer; }
		}

		public void SendFailed(RpcErrorCode code, Exception ex)
		{
			// do Nothing
			_tracing.ErrorFmt(ex, "TcpServerTransaction SendFailed {0} - {1}", code, ex);
		}
		#endregion


		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpServerTransaction));
	}
}


// HEADER
// REQUEST
// BODY
// REWQuest
