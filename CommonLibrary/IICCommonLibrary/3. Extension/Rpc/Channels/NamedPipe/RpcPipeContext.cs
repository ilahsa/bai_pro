using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc
{
	[ProtoContract]
	class RpcPipeContext
	{
		[ProtoMember(1)]
		public string ServiceName = string.Empty; 

		[ProtoMember(2)]
		public string MethodName = string.Empty; 

		[ProtoMember(3)]
		public bool HasBody;			// false mean null

		[ProtoMember(4)]
		public string From = string.Empty; 

		[ProtoMember(5)]
		public string To = string.Empty;

		[ProtoMember(6)]
		public RpcErrorCode RetCode = RpcErrorCode.Unknown;
	}
}
