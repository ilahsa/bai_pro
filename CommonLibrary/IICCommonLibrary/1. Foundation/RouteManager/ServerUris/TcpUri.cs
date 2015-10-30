using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class TcpUri: IPv4Uri
	{
		public TcpUri(string address, int port)
			: this(IPAddress.Parse(address), port)
		{
		}

		public TcpUri(IPAddress ipAddress, int port)
			: base("tcp", ipAddress, port)
		{
		}

		public static new TcpUri Parse(string uri)
		{
			if (uri.StartsWith("tcp://"))
				return Parse(uri, "tcp://".Length);
			else if (uri.StartsWith("tcp:"))
				return Parse(uri, "tcp:".Length);
			else
				throw new FormatException("Unreconized Uri:" + uri);
		}

		public static TcpUri Parse(string uri, int start)
		{
			int port;
			IPAddress address;

			TryParse(uri, start, out address, out port);
			return new TcpUri(address, port);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public override ServerUri ReplaceWith(string address)
		{
			return new TcpUri(address, Port);
		}
	}
}
