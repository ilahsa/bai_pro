using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class NamedPipeUri: ServerUri
	{
		private string _computer;
		private string _pipeName;

		public string Computer
		{
			get { return _computer; }
		}

		public string PipeName
		{
			get { return _pipeName; }
		}

		public NamedPipeUri(string computer, string pipeName)
			: base("pipe")
		{
			_computer = computer;
			_pipeName = pipeName;
		}

		public static new NamedPipeUri Parse(string uri)
		{
			int startIndex = "pipe://".Length;

			int p = uri.IndexOf(':', startIndex);
			int end = uri.IndexOf('/', startIndex);
			if (end < 0)
				end = uri.Length;

			string computer = uri.Substring(startIndex, p - startIndex);
			string pipename = uri.Substring(p + 1, end - p - 1);

			return new NamedPipeUri(computer, pipename);
		}

		public override string ToString()
		{
			return string.Format("pipe://{0}:{1}", _computer, _pipeName);
		}

		public override int GetHashCode()
		{
			return _computer.GetHashCode() ^
				_pipeName.GetHashCode();
		}

		public override bool Equals(object rval)
		{
			if (rval is NamedPipeUri) {
				NamedPipeUri r = (NamedPipeUri)rval;
				return r._computer == this._computer &&
					r._pipeName == this._pipeName;
			} else {
				return false;
			}	 
		}
	}
}
