using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Configuration
{
	[ProtoContract]
	public class IICConfigTableBuffer
	{
		[ProtoMember(1)]
		public string TableName;

        [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
		public DateTime Version;

		[ProtoMember(3)]
		public string[] ColumnNames;

		[ProtoMember(4)]
		public List<RpcClass<string[]>> Rows = new List<RpcClass<string[]>>();
	}
}
