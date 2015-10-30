using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeClientTransaction: RpcClientTransaction
	{
		private Action _callback;
		private RpcPipeContext _context;
		private RpcPipeClientChannel _channel;

		private byte[] _bodyBuffer;
		private NamedPipeClientStream _stream;

		public RpcPipeClientTransaction(RpcPipeClientChannel channel, NamedPipeUri serverUri, RpcRequest request)
			: base(serverUri, request)
		{
			_channel = channel;
		}

		public override void SendRequest(Action callback, int timeout)
		{
			_callback = callback;

			_context = new RpcPipeContext();

			_context.From = Request.ServiceAtComputer;
			_context.To = Request.ContextUri;
			_context.ServiceName = Request.Service;
			_context.MethodName = Request.Method;
			_context.HasBody = Request.BodyBuffer != null;

			var pipeUri = (NamedPipeUri)ServerUri;
			_stream = new NamedPipeClientStream(pipeUri.Computer, pipeUri.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

			try {
				timeout = timeout > 0 ? timeout : _channel.Timeout;
				if (timeout > 0) {
					_stream.Connect(timeout);
				} else {
					_stream.Connect();
				}

				if (Request.BodyBuffer != null) {
					byte[] buffer = Request.BodyBuffer.GetByteArray();
					RpcPipeStreamHelper.WriteStream(_stream, _context, buffer);
				} else {
					RpcPipeStreamHelper.WriteStream(_stream, _context, null);
				}

				_context = RpcPipeStreamHelper.ReadStream(_stream, out _bodyBuffer);

				if (_context.RetCode == RpcErrorCode.OK) {
					if (_context.HasBody) {
						Response = RpcResponse.Create(_bodyBuffer);
					} else {
						Response = RpcResponse.Create();
					}
				} else {
					if (_bodyBuffer != null) {
						Exception ex = BinarySerializer.FromByteArray<Exception>(_bodyBuffer);
						Response = RpcResponse.Create(_context.RetCode, ex);
					} else {
						Response = RpcResponse.Create(_context.RetCode, null);
					}
				}
				_callback();
			} catch (Exception ex) {
				Response = RpcResponse.Create(RpcErrorCode.SendFailed, ex);
				_callback();
			} finally {
				if (_stream != null) {
					_stream.Close();
				}
			}
		}
	}
}
