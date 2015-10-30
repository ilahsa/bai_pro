using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public interface IUriParser
	{
		ResolvableUri ParseUri(string fullUri);
	}
}
