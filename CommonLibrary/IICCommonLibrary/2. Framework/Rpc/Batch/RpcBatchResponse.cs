using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc
{
	[ProtoContract]
	public class RpcBatchResponse
	{
		[ProtoMember(1)]
		public RpcErrorCode ErrorCode;

		[ProtoMember(2)]
		public byte[] ResponseData;
	}
}
