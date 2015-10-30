using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc
{
	[ProtoContract]
	public class RpcBatchRequest
	{
		[ProtoMember(1)]
		public string ContextUri = null;

		[ProtoMember(2)]
		public bool HasBody = false;

		[ProtoMember(3)]
		public byte[] RequestData = null;
	}
}
