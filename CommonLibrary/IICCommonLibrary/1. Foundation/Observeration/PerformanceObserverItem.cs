using System;
using System.Diagnostics;

namespace Imps.Services.CommonV4.Observation
{
	public class PerformanceObserverItem: ObserverItem
	{
		[ObserverField(ObserverColumnType.Interger)]
		public int ExecTotal = 0;

		[ObserverField(ObserverColumnType.Interger)]
		public int ExecFailed = 0;

		[ObserverField(ObserverColumnType.Double)]
		public double AvgElapseMs = 0.0f;

		[ObserverField(ObserverColumnType.Interger)]
		public double MaxElapseMs = -1;

		[ObserverField(ObserverColumnType.Interger)]
		public double MinElapseMs = -1;

		[ObserverField(ObserverColumnType.Interger)]
		public double LastElapseMs = -1;

		[ObserverField(ObserverColumnType.String)]
		public string LastException = string.Empty;

		public double TotalElapseMs = 0;

		public void Track(Exception ex, long elapseTicks)
		{
			Track(ex == null, ex, elapseTicks);
		}

		public void Track(bool successed, Exception ex, long elapseTicks)
		{
			double elapseMs = (double)elapseTicks * 1000 / Stopwatch.Frequency;

			lock (this) {
				LastElapseMs = elapseMs;

				if (ExecTotal == 0) {
					MaxElapseMs = elapseMs;
					MinElapseMs = elapseMs;
				} 

				ExecTotal++;
				TotalElapseMs += elapseMs;
				AvgElapseMs = TotalElapseMs / ExecTotal;

				if (!successed) {
					ExecFailed++;
					if (ex != null)
						LastException = ex.ToString();
				}

				if (elapseMs > MaxElapseMs)
					MaxElapseMs = elapseMs;

				if (elapseMs < MinElapseMs)
					MinElapseMs = elapseMs;
			}
		}
	}
}
