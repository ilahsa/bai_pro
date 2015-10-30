using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeServerTransaction : RpcServerTransaction
	{
		private byte[] _buffer;
		private RpcPipeContext _context;
		private NamedPipeServerStream _stream;
		private RpcPipeServerChannel _channel;

		public RpcPipeServerTransaction(RpcPipeServerChannel channel, NamedPipeServerStream stream)
			: base(channel, null, null)
		{
			_stream = stream;
			_channel = channel;

			_context = RpcPipeStreamHelper.ReadStream(_stream, out _buffer);
			var req = new RpcRequest() {
				ServiceAtComputer = _context.From,
				Service = _context.ServiceName,
				Method = _context.MethodName,
				ContextUri = _context.To,
			};

			if (_context.HasBody) {
				req.BodyBuffer = new RpcBodyBuffer(_buffer);
			} else {
				req.BodyBuffer = null;
			}

			SetRequest(req);
		}

		public override void SendResponse(RpcResponse response)
		{
			try {
				_context.RetCode = response.ErrorCode;
				_context.HasBody = response.BodyBuffer != null;

				if (_context.HasBody) {
					byte[] buffer = response.BodyBuffer.GetByteArray();
					RpcPipeStreamHelper.WriteStream(_stream, _context, buffer);
				} else {
					RpcPipeStreamHelper.WriteStream(_stream, _context, null);
				}
			} finally {
				_stream.Disconnect();
				_channel.RecycleServerStream(_stream);
			}
		}
	}
}
