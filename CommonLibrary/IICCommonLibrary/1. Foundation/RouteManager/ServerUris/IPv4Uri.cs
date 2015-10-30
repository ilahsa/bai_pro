using System;
using System.Net;

namespace Imps.Services.CommonV4
{
	public class IPv4Uri: ServerUri
	{
		private IPEndPoint _endpoint;

		public IPEndPoint Endpoint
		{
			get { return _endpoint; }
		}

		public IPAddress Address
		{
			get { return _endpoint.Address; }
		}

		public int Port
		{
			get { return _endpoint.Port; }
		}

		public IPv4Uri(string protocol, IPAddress address, int port)
			: base(protocol)
		{
			_endpoint = new IPEndPoint(address, port);
		}

		public static bool TryParse(string uri, int start, out IPAddress address, out int port)
		{
			// "tcp://127.0.0.1:8080/";
			//  01234567890123456789
			//        s        p

			int p = uri.IndexOf(':', start);
			int end = uri.IndexOf('/', start);
			if (end < 0)
				end = uri.Length;

			string ip = uri.Substring(start, p - start);
			string portstr = uri.Substring(p + 1, end - p - 1);

			address = IPAddress.Parse(ip);
			port = int.Parse(portstr);
			return true;
		}

		public override string ToString()
		{
			return string.Format("{0}://{1}:{2}", Protocol, Address, Port);
		}

		public override int GetHashCode()
		{
			return _endpoint.GetHashCode();
		}

		public override bool Equals(object rval)
		{
			if (rval is IPv4Uri) {
				var r = (IPv4Uri)rval;
				return _endpoint.Equals(r._endpoint);
			} else {
				return false;
			}
		}
	}
}
