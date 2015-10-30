using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class HttpUri: ServerUri
	{
		private string _url;

		public HttpUri(string uri)
			: base("http")
		{
			_url = uri;
		}

		public static new HttpUri Parse(string uri)
		{
			return new HttpUri(uri);
		}

		public override ServerUri ReplaceWith(string address)
		{
			int p = _url.IndexOf(':', "http://".Length);
			int e = _url.IndexOf('/', "http://".Length);

			string url;
			if (p < 0) {
				if (e < 0) {
					// like  http://192.168.100.170
					url = "http://" + address;
				} else {
					// like  http://192.168.100.170/Something
					url = "http://" + address + _url.Substring(e);
				}
			} else {
				// like  http://192.168.100.170:8800		or
				// like  http://192.168.100.170:8800/Someting
				url = "http://" + address + _url.Substring(p);
			}
			return new HttpUri(url);
		}

		public override string ToString()
		{
			return _url;
		}

		public override int GetHashCode()
		{
			return _url.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is HttpUri) && ((HttpUri)obj)._url == this._url;
		}
	}
}
