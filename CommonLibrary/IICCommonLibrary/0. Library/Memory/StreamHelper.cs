using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class StreamHelper
	{
		public static void CopyStream(Stream src, Stream dest, int len)
		{
            byte[] buffer = new byte[len];
            src.Read(buffer, 0, len);
            dest.Write(buffer, 0, len);
		}
	}
}
