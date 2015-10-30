using System;

namespace Imps.Services.CommonV4
{
	public static class NumberUtils
	{
		public static int NextPower2(int a)
		{
			int n = 1;
			for (int i = 0; i < 31; i++) {
				if (a <= n)
					return n;
				n = n << 1;
			}
			throw new NotSupportedException("Too Big Number:" + a);
		}

		public static int Log2(int a)
		{
			int n = 0;
			while (a > 0) {
				a = a >> 1;
				n++;
			}
			return n;
		}
	}
}
