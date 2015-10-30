using System;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Configuration
{
	[ProtoContract]
	public class IICConfigItemBuffer
	{
		[ProtoMember(1)]
		public string Path;

		[ProtoMember(2)]
		public string Key;

		[ProtoMember(3)]
		public string Field;

		[ProtoMember(4)]
		public string Value;

        [ProtoMember(5, DataFormat = DataFormat.FixedSize)]
		public DateTime Version;
	}
}
