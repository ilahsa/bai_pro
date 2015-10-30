using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc
{
	[RpcService(RpcInternalServiceName.DuplexService, ClientChecking = false)]
	interface IRpcDuplexService
	{
		[RpcServiceMethod()]
		void Register(RpcServerContext ctx);

		[RpcServiceMethod()]
		void KeepAlive(RpcServerContext ctx);
	}

	[ProtoContract]
	class DuplexRegisterArgs<T>
	{
		[ProtoMember(1)]
		public T Args = default(T);
	}

	[ProtoContract]
	class DuplexRegisterResults<T>
	{
		[ProtoMember(1)]
		public string ServiceName = null;

		[ProtoMember(2)]
		public string ComputerName = null;

		[ProtoMember(3)]
		public T Results = default(T);
	}
}
