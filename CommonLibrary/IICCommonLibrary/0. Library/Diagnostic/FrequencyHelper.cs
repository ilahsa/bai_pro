using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class FrequencyHelper
	{
		public static long TicksPerMs = Stopwatch.Frequency / 1000;

		public static long TicksPerNano = Stopwatch.Frequency / 1000 / 1000;
	}
}
