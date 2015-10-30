using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public class IICAssertFailedException : Exception
	{
		public IICAssertFailedException(string message)
			: base(message)
		{
		}

		protected IICAssertFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}
