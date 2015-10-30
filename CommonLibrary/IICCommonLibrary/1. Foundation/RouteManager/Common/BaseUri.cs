using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public abstract class BaseUri
	{
		private string _protocol;

		public string Protocol
		{
			get { return _protocol; }
		}

		public BaseUri(string protocol)
		{
			_protocol = protocol;
		}

		public abstract override string ToString();

		public abstract override int GetHashCode();

		public abstract override bool Equals(object obj);
	}
}
