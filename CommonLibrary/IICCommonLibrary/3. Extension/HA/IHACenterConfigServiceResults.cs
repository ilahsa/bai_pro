using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.HA
{
	[ProtoContract]
	public class HAServiceSettings
	{
		[ProtoMember(1)]
		public string ServiceOriginName;

		[ProtoMember(2)]
		public string Domain;

		[ProtoMember(3)]
		public int PoolId;

		[ProtoMember(4)]
		public string Site;
	}

	[ProtoContract]
	public class HAGetConfigVersionResults
	{
		[ProtoMember(1)]
		public IICConfigType ConfigType;

		[ProtoMember(2)]
		public string ConfigPath;

		[ProtoMember(3)]
		public string ConfigKey;

        [ProtoMember(4, DataFormat = DataFormat.FixedSize)]
		public DateTime Version;
	}
}
