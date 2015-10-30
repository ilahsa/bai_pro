using System;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	static class RpcPipeStreamHelper
	{
		public static byte[] EmptyBuffer = new byte[0];

		public static void WriteStream(PipeStream stream, RpcPipeContext context, byte[] bodyBuf)
		{
			byte[] contextBuffer = ProtoBufSerializer.ToByteArray<RpcPipeContext>(context);

			RpcPipeHeader header;
			header.Mark = RpcPipeHeader.MagicMark;
			header.ContextSize = contextBuffer.Length;
			header.BodySize = bodyBuf == null ? 0 : bodyBuf.Length;

			byte[] headerBuffer = RpcPipeHeader.ToByteArray(header);
			stream.Write(headerBuffer, 0, headerBuffer.Length);
			stream.Write(contextBuffer, 0, contextBuffer.Length);

			if (header.BodySize > 0) {
				stream.Write(bodyBuf, 0, bodyBuf.Length);
			}

			stream.Flush();
		}

		public static void WriteStreamEx(PipeStream stream, RpcPipeContext context, Exception ex)
		{
			byte[] contextBuffer = ProtoBufSerializer.ToByteArray<RpcPipeContext>(context);
			byte[] bodyBuffer = null;
			if (ex != null) {
				bodyBuffer = BinarySerializer.ToByteArray(ex);
			} else {
				bodyBuffer = EmptyBuffer;
			}

			RpcPipeHeader header;
			header.Mark = RpcPipeHeader.MagicMark;
			header.ContextSize = contextBuffer.Length;
			header.BodySize = bodyBuffer.Length;

			byte[] headerBuffer = RpcPipeHeader.ToByteArray(header);
			stream.Write(headerBuffer, 0, headerBuffer.Length);
			stream.Write(contextBuffer, 0, contextBuffer.Length);

			if (header.BodySize > 0) {
				stream.Write(bodyBuffer, 0, bodyBuffer.Length);
			}

			stream.Flush();
		}

		public static RpcPipeContext ReadStream(PipeStream stream, out byte[] buffer)
		{
			byte[] headerBuffer = new byte[RpcPipeHeader.Size]; 
			stream.Read(headerBuffer, 0, RpcPipeHeader.Size);
			RpcPipeHeader header = RpcPipeHeader.FromByteArray(headerBuffer);

			if (header.Mark != RpcPipeHeader.MagicMark) {
				// throw new RpcException("RpcPipeHeader Crashed", "", RpcErrorCode.SendFailed, null);
				buffer = null;
				return null;
			}

			if (header.ContextSize > 1024 || header.BodySize > 64000000) {
				// throw new RpcException("RpcPipeHeader Length To Long", "", RpcErrorCode.SendFailed, null);
				buffer = null;
				return null;
			}

			byte[] contextBuffer = new byte[header.ContextSize];
			stream.Read(contextBuffer, 0, contextBuffer.Length);
			RpcPipeContext context = ProtoBufSerializer.FromByteArray<RpcPipeContext>(contextBuffer);

			if (context.HasBody) {
				if (header.BodySize == 0) {
					buffer = EmptyBuffer;
				} else {
					buffer = new byte[header.BodySize];
					stream.Read(buffer, 0, buffer.Length);
				}
			} else {
				buffer = null;
			}
			return context;
		}
	}
}
