using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc的应答实体类, 直接使用Transaction模式的时候, 可以直接使用
	/// </summary>
	public sealed class RpcResponse
	{
		private Exception _error = null;

		public RpcErrorCode ErrorCode;				// ErrorCode 
		public RpcBodyBuffer BodyBuffer;			// Body
		public RpcMessageOptions Options = RpcMessageOptions.None;	// Options, Channel Add

		internal object BodyValue
		{
			get { return BodyBuffer != null ? BodyBuffer.Value : null; }
		}

		public Exception Error
		{
			get 
			{
				if (ErrorCode == RpcErrorCode.OK) {
					return null;
				} else if (_error != null) {
					return _error;
				} else {
					if (BodyBuffer == null) {
						return null;
					} else {
						_error = BodyBuffer.GetException();
						return _error;
					}
				}
			}
		}

		public RpcResponse()
		{
		}

		public RpcResponse(RpcErrorCode code, RpcBodyBuffer body)
		{
			ErrorCode = code;
			BodyBuffer = body; 
		}

		public static RpcResponse Create()
		{
			return new RpcResponse(RpcErrorCode.OK, null);
		}

		public static RpcResponse Create<T>(T results)
		{
			if (typeof(T) == typeof(RpcNull) || results == null) {
				return new RpcResponse(RpcErrorCode.OK, null);
			} else {
				var body = new RpcBodyBuffer<T>(results);
				return new RpcResponse(RpcErrorCode.OK, body);
			}
		}

		public static RpcResponse Create(Stream stream, int streamLen)
		{
			return new RpcResponse(RpcErrorCode.OK, new RpcBodyBuffer(stream, streamLen));
		}

		public static RpcResponse Create(byte[] buffer)
		{
			return new RpcResponse(RpcErrorCode.OK, new RpcBodyBuffer(buffer));
		}

		public static RpcResponse Create(RpcErrorCode code)
		{
			return new RpcResponse(code, null);
		}

		public static RpcResponse Create(RpcErrorCode code, Stream stream, int streamLen)
		{
			return new RpcResponse(code, new RpcBodyBuffer(stream, streamLen));
		}

		public static RpcResponse Create(RpcErrorCode code, Exception ex)
		{
			if (ex != null) {
				var body = new RpcBodyBuffer(ex);
				return new RpcResponse(code, body);
			} else {
				return new RpcResponse(code, null);
			}
		}
	}
}
