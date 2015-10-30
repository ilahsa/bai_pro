using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class ServerUriManager
	{
		private static Dictionary<string, ServerUri> _allUris = new Dictionary<string,ServerUri>();

		public static ServerUri Parse(string uri)
		{
			return Parse(uri, string.Empty);
		}

		public static ServerUri Parse(string uri, string serviceRole)
		{
			try {
				int p = uri.IndexOf(":");
				if (p < 0) {
					throw new FormatException("Uri FormatFailed'" + uri + "'");
				}

				string protocol = uri.Substring(0, p);

				//
				// skip '//'
				if (uri[p + 1] == '/' && uri[p + 2] == '/')
					p = p + 3;
				else
					p = p + 1;

				switch (protocol.ToLower()) {
					case "tcp":
						return TcpUri.Parse(uri, p);
					case "sipc":
						return SipcUri.Parse(uri, p);
					case "http":
						return HttpUri.Parse(uri);
					case "inproc":
						return InprocUri.Instance;
					case "pipe":
						return NamedPipeUri.Parse(uri);
					default:
						throw new Exception("Unrecongized Uri '" + uri + "'");
				}
			} catch (Exception ex) {
				throw new FormatException("Unrecognized Uri:" + uri, ex);
			}
		}
	}
}
