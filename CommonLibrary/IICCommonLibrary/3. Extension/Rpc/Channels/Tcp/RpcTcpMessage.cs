using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	class RpcTcpMessage<T>
	{
		public int Sequence;

		public T Message;
	}
}
