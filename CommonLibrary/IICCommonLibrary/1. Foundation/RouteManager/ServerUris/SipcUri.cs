using System;
using System.Net;

namespace Imps.Services.CommonV4
{
	public sealed class SipcUri: IPv4Uri
	{
		public SipcUri(string address, int port)
			: this(IPAddress.Parse(address), port)
		{
		}

		public SipcUri(IPAddress address, int port)
			: base("sipc", address, port)
		{
		}

		public static new SipcUri Parse(string uri)
		{
			if (uri.StartsWith("sipc://"))
				return Parse(uri, "sipc://".Length);
			else if (uri.StartsWith("sipc:"))
				return Parse(uri, "sipc:".Length);
			else
				throw new FormatException("Unreconized Uri:" + uri);
		}

		public static SipcUri Parse(string uri, int start)
		{
			int port;
			IPAddress address;

			TryParse(uri, start, out address, out port);
			return new SipcUri(address, port);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public override ServerUri ReplaceWith(string address)
		{
			return new SipcUri(address, Port);
		}
	}
}
