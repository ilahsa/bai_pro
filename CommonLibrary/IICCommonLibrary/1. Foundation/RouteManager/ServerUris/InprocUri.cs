using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public sealed class InprocUri: ServerUri
	{
		public static InprocUri Instance = new InprocUri();

		public InprocUri()
			: base("inproc")
		{
		}

		public override string ToString()
		{
			return "inproc://";
		}

		public override int GetHashCode()
		{
			return 0;	// only one hash code
		}

		public override bool Equals(object obj)
		{
			// all inproc Uri is Equals
			return obj is InprocUri;
		}
	}
}
