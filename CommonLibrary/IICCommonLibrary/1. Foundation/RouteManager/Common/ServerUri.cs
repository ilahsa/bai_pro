using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public abstract class ServerUri: BaseUri
	{
		public string ServiceRole
		{
			get;
			set;
		}

		public ServerUri(string protocol)
			: base(protocol)
		{
		}

		public virtual ServerUri ReplaceWith(string address)
		{
			throw new NotSupportedException("");
		}

		public static ServerUri Parse(string uri)
		{
			return ServerUriManager.Parse(uri);
		}
	}
}
