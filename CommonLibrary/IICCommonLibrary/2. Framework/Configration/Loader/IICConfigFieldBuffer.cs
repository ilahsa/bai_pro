using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Configuration
{
	[ProtoContract]
	public class IICConfigFieldBuffer
	{
		[ProtoMember(1)]
		public string Key;

		[ProtoMember(2)]
		public string Value;

        [ProtoMember(3, DataFormat = DataFormat.FixedSize)]
		public DateTime Version;
	}
}
