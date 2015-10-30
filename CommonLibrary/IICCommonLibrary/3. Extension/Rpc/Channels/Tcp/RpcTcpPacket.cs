/*
 * RpcTcp包体结构
 * 参考http://10.10.41.234/trac/fae/wiki/RpcOverTcp
 * 
 * MessageHeader：
	参考rpc.proto文件
 * 
 * Bodys
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc.ProtoContract;

namespace Imps.Services.CommonV4.Rpc
{
	//
	// RpcTcpPacket Only for receive
	class RpcTcpPacket
	{
		public const int IdentityLength = 12;
		public const int IdentityMarkRequest = 0x0BADBEE0;
		public const int IdentityMarkResponse = 0x0BADBEE1;

		public RpcMessageDirection Direction;
		public int HeaderLength;
		public int BodyLength;
		public int PacketLength;
		public int Option;

		public int IdentityNeed;
		public int NextRecvSize;

		private byte[] _identityBuffer = null;
		private byte[] _headerBuffer;
		private byte[] _bodyBuffer;


		/// <summary>
		///		创建一个数据包, 首次调用的buffer长度应当小于等于IdentityLength
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count">小于等于IdentityLength</param>
		public RpcTcpPacket(byte[] buffer, int offset, int count)
		{
			IICAssert.IsTrue(count <= IdentityLength);
			if (count == IdentityLength) {
				IdentityNeed = 0;
				ParseIdentity(buffer, offset);
			} else {
				_identityBuffer = new byte[IdentityLength];
				Array.Copy(buffer, offset, _identityBuffer, 0, count);
				IdentityNeed = IdentityLength - count;
			}
		}

		public void FillIdentity(byte[] buffer, int offset, int count)
		{
			IICAssert.IsTrue(count <= IdentityNeed);
			int copySize = count == IdentityNeed ? IdentityNeed : count;
			int copyOffset = IdentityLength - IdentityNeed;

			Array.Copy(buffer, offset, _identityBuffer, copyOffset, copySize);

			IdentityNeed -= copySize;
			if (IdentityNeed == 0) {
				ParseIdentity(_identityBuffer, offset);
			}
		}

		public void FillNext(byte[] buffer, int offset, int count)
		{
			IICAssert.IsTrue(IdentityNeed == 0 && count <= NextRecvSize);
			//
			// 如果没接受完Header, copy到Header Buffer, 否则copy到Body Buffer

			//
			// 包头剩余长度
			int headerLeft = NextRecvSize - BodyLength;
			if (headerLeft > 0) {
				int copySize = count >= headerLeft ? headerLeft : count;
				int copyOffset = HeaderLength - headerLeft;
				Array.Copy(buffer, offset, _headerBuffer, copyOffset, copySize);
				offset += headerLeft;
				count -= headerLeft;
				NextRecvSize -= copySize;
			}

			//
			// 如果有包体需要接收, 并且还有剩余缓冲区
			if (BodyLength > 0 && count > 0) {
				int bodyLeft = NextRecvSize;
				int copySize = count >= bodyLeft ? bodyLeft : count;
				int copyOffset = BodyLength - bodyLeft;
				Array.Copy(buffer, offset, _bodyBuffer, copyOffset, copySize);
				NextRecvSize -= copySize;
			}
		}

		public RpcTcpMessage<RpcRequest> GetRequest()
		{
			var h = ProtoBufSerializer.FromByteArray<RpcRequestHeader>(_headerBuffer);

			RpcRequest request = new RpcRequest() {
				Service = h.Service,
				Method = h.Method,
				FromComputer = h.FromComputer,
				FromService = h.FromService,
				ContextUri = h.ContextUri,
				Options = (RpcMessageOptions)h.Option,
			};

			int len = h.BodyLength - 1;
			if (len > 0) {
				request.BodyBuffer = new RpcBodyBuffer(_bodyBuffer);
			} else if (len == 0) {
				request.BodyBuffer = RpcBodyBuffer.EmptyBody;
			} else {
				request.BodyBuffer = null;
			}
			var message = new RpcTcpMessage<RpcRequest>() {
				Sequence = h.Sequence,
				Message = request,
			};
			return message;
		}

		public RpcTcpMessage<RpcResponse> GetResponse()
		{
			var h = ProtoBufSerializer.FromByteArray<RpcResponseHeader>(_headerBuffer);

			int offset = IdentityLength + HeaderLength;
			RpcResponse response = new RpcResponse() {
				ErrorCode = (RpcErrorCode)h.ResponseCode,
				Options = (RpcMessageOptions)h.Option,
			};
			int len = h.BodyLength - 1;
			if (len > 0) {
				response.BodyBuffer = new RpcBodyBuffer(_bodyBuffer);
				response.BodyBuffer.TextError = true;
				offset += len;
			} else if (len == 0) {
				response.BodyBuffer = RpcBodyBuffer.EmptyBody;
			} else {
				response.BodyBuffer = null;
			}
			var message = new RpcTcpMessage<RpcResponse>() {
				Sequence = h.Sequence,
				Message = response,
			};
			return message;
		}

		public static void WriteMessage(Stream stream, IRpcTcpSendingPacket p)
		{
			long begin = stream.Position;
			int bodyLength;
			int headerLength = 0;
			if (p.BodyBuffer == null) {
				bodyLength = -1;
			} else {
				bodyLength = p.BodyBuffer.GetSize() + 1;
			}

			byte[] headerBuffer = new byte[IdentityLength];
			stream.Write(headerBuffer, 0, IdentityLength);

			if (p.Direction == RpcMessageDirection.Request) {
				var h = p.RequestHeader;
				h.BodyLength = bodyLength;
				headerLength = (int)ProtoBufSerializer.Serialize(stream, h);
				SetInt32(headerBuffer, 0, IdentityMarkRequest);
			} else {
				var h = p.ResponseHeader;
				h.BodyLength = bodyLength;
				headerLength = (int)ProtoBufSerializer.Serialize(stream, h);
				SetInt32(headerBuffer, 0, IdentityMarkResponse);
			}
			int bodySize = 0;
			if (p.BodyBuffer != null) {
				p.BodyBuffer.TextError = true;
				bodySize = p.BodyBuffer.WriteToStream(stream);
			}
			long end = stream.Position;

			SetInt32(headerBuffer, 4, IdentityLength + headerLength + bodySize);
			SetInt16(headerBuffer, 8, headerLength);
			SetInt16(headerBuffer, 10, 0);

			stream.Seek(begin, SeekOrigin.Begin);
			stream.Write(headerBuffer, 0, IdentityLength);
			stream.Seek(end, SeekOrigin.Begin);
		}

		private static int GetInt32(byte[] buffer, int offset)
		{
			return 
				buffer[offset + 3] |
				buffer[offset + 2] << 8 |
				buffer[offset + 1] << 16 |
				buffer[offset + 0] << 24;
		}

		private static void SetInt32(byte[] buffer, int offset, int value)
		{
			buffer[offset + 3] = (byte)(value & 0xff);
			buffer[offset + 2] = (byte)((value >> 8) & 0xff);
			buffer[offset + 1] = (byte)((value >> 16) & 0xff);
			buffer[offset + 0] = (byte)((value >> 24) & 0xff);
		}

		private static int GetInt16(byte[] buffer, int offset)
		{
			return
				buffer[offset + 1] |
				buffer[offset + 0] << 8;
		}

		private static void SetInt16(byte[] buffer, int offset, int value)
		{
			buffer[offset + 1] = (byte)(value & 0xff);
			buffer[offset + 0] = (byte)((value >> 8) & 0xff);
		}

		private void ParseIdentity(byte[] buffer, int offset)
		{
			//
			// 提取IdentityMark, 判断是请求还是应答
			int identityMark = GetInt32(buffer, offset + 0);

			if (identityMark == IdentityMarkRequest) {
				Direction = RpcMessageDirection.Request;
			} else if (identityMark == IdentityMarkResponse) {
				Direction = RpcMessageDirection.Response;
			} else {
  				throw new Exception("Identity Currpted:" + StrUtils.ToHexString(buffer, offset, IdentityLength));
			}

			PacketLength = GetInt32(buffer, offset + 4);
			HeaderLength = GetInt16(buffer, offset + 8);
			Option = GetInt16(buffer, offset + 10);

			//
			// 下一个包需要接受的大小
			NextRecvSize = PacketLength - IdentityLength;
			BodyLength = PacketLength - IdentityLength - HeaderLength;

			_headerBuffer = new byte[HeaderLength];
			if (BodyLength > 0)
				_bodyBuffer = new byte[BodyLength];
		}

		public string DumpInfo()
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("{0}:{1}\r\n", Direction, PacketLength);
			if (_identityBuffer != null)
				str.AppendFormat("Identity[{0}] : {1}\r\n", _identityBuffer.Length, StrUtils.ToHexString(_identityBuffer));
			else
				str.AppendFormat("HeaderLength={0}, BodyLength={1}", HeaderLength, BodyLength);

			if (_headerBuffer != null)
				str.AppendFormat("Header[{0}]: {1}\r\n", _headerBuffer.Length, StrUtils.ToHexString(_headerBuffer));

			if (_bodyBuffer != null)
				str.AppendFormat("Body[{0}]: {1}\r\n", _bodyBuffer.Length, StrUtils.ToHexString(_bodyBuffer));

			return str.ToString();
		}

		private static byte[] PlaceHolder = new byte[IdentityLength];
	}
}
