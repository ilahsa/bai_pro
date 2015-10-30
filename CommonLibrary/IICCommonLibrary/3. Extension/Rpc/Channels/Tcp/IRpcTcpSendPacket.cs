/*
 * RpcTcpClientTransaction 和 RpcTcpServerTranscation的共同继承接口，
 * 
 * 用于在RpcTcpSocketConnection中做SendPendingQueue的管理序列化接口
 * 
 * Gaolei
 * 2010-06-24
 */ 
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc.ProtoContract;

namespace Imps.Services.CommonV4.Rpc
{
	//
	// for Send
	interface IRpcTcpSendingPacket
	{
		RpcMessageDirection Direction { get; }

		RpcRequestHeader RequestHeader { get; }

		RpcResponseHeader ResponseHeader { get; }

		RpcBodyBuffer BodyBuffer { get; }

		void SendFailed(RpcErrorCode code, Exception ex);
	}
}
