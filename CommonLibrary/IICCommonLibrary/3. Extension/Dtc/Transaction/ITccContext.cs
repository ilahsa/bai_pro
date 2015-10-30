using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public interface ITccContext
	{
		string Key { get; }

		byte[] Serialize();

        T Deserialize<T>(byte[] bytes);
	}
}
