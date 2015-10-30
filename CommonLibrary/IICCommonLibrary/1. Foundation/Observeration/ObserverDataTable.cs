using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4
{
	[ProtoContract]
	public enum ObserverColumnType
	{
		Interger	= 0,
		String		= 1,
		Double		= 2,
		DateTime	= 3,
	}

	[ProtoContract]
	public class ObserverDataTable
	{
		[ProtoMember(1)]
		public string[] Columns;

		[ProtoMember(2)]
		public ObserverColumnType[] ColumnTypes;

		[ProtoMember(3)]
		public List<RpcClass<string[]>> Rows;

		[ProtoMember(4)]
		public DateTime ClearTime;
	}
}
