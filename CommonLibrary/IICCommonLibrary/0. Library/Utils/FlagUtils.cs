using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class FlagUtils
	{
		#region SetBit Methods
		public static byte SetBit(byte origin, byte mask, bool val)
		{
			return (byte)_SetBit(origin, mask, val);
		}

		public static short SetBit(short origin, short mask, bool val)
		{
			return (short)_SetBit(origin, mask, val);
		}

		public static int SetBit(int origin, int mask, bool val)
		{
			return (int)_SetBit(origin, mask, val);
		}

		public static long SetBit(long origin, long mask, bool val)
		{
			return _SetBit(origin, mask, val);
		}

		public static byte SetBit<T>(byte origin, T mask, bool val)
		{
			return (byte)_SetBit(origin, Convert.ToInt64(mask), val);
		}

		public static short SetBit<T>(short origin, T mask, bool val)
		{
			return (short)_SetBit(origin, Convert.ToInt64(mask), val);
		}

		public static int SetBit<T>(int origin, T mask, bool val)
		{
			return (int)_SetBit(origin, Convert.ToInt64(mask), val);
		}

		public static long SetBit<T>(long origin, T mask, bool val)
		{
			return (long)_SetBit(origin, Convert.ToInt32(mask), val);
		}
		#endregion

		#region GetBit Methods
		public static bool GetBit(long origin, long mask)
		{
			return _GetBits(origin, mask) > 0;
		}

		public static bool GetBit<T>(long origin, T mask)
		{
			return _GetBits(origin, Convert.ToInt64(mask)) > 0;
		}
		#endregion

		#region SetBits Methods
		public static byte SetBits<T1, T2>(byte origin, T1 mask, T2 val)
		{
			return (byte)_SetBits(origin, Convert.ToInt64(mask), Convert.ToInt64(val));
		}

		public static short SetBits<T1, T2>(short origin, T1 mask, T2 val)
		{
			return (short)_SetBits(origin, Convert.ToInt64(mask), Convert.ToInt64(val));
		}

		public static int SetBits<T1, T2>(int origin, T1 mask, T2 val)
		{
			return (int)_SetBits(origin, Convert.ToInt64(mask), Convert.ToInt64(val));
		}
		public static long SetBits<T1, T2>(long origin, T1 mask, T2 val)
		{
			return (long)_SetBits(origin, Convert.ToInt64(mask), Convert.ToInt64(val));
		}
		#endregion

		#region GetBits Methods
		public static byte GetBits(byte origin, byte mask)
		{
			return (byte)_GetBits(origin, mask);
		}

		public static short GetBits(short origin, short mask)
		{
			return (short)_GetBits(origin, mask);
		}

		public static int GetBits(int origin, int mask)
		{
			return (int)_GetBits(origin, mask);
		}

		public static long GetBits(long origin, long mask)
		{
			return _GetBits(origin, mask);
		}

		public static byte GetBits<T>(byte origin, T mask)
		{
			return (byte)_GetBits(origin, Convert.ToInt64(mask));
		}

		public static short GetBits<T>(short origin, T mask)
		{
			return (short)_GetBits(origin, Convert.ToInt64(mask));
		}

		public static int GetBits<T>(int origin, T mask)
		{
			return (int)_GetBits(origin, Convert.ToInt64(mask));
		}

		public static long GetBits<T>(long origin, T mask)
		{
			return _GetBits(origin, Convert.ToInt64(mask));
		}
		#endregion

		#region Other HelperMethods
		public static int GetMask(int order)
		{
			return 1 << order;
		}

		public static long GetMask(long order)
		{
			return 1L << (int)order;
		}

		public static int GetMask(int orderStart, int orderEnd)
		{
			int ret = 1;
			for (int i = 0; i < orderStart; i++)
				ret = ret << 1;

			for (int i = (int)orderStart; i < (int)orderEnd; i++)
				ret = (ret << 1) + ret;

			return ret;
		}

		public static long GetMask(long orderStart, long orderEnd)
		{
			int ret = 1;
			for (int i = 0; i < (int)orderStart; i++)
				ret = ret << 1;

			for (int i = (int)orderStart; i < (int)orderEnd; i++)
				ret = (ret << 1) + ret;

			return ret;
		}

		public static int GetOrder(long mask)
		{
			int n = 0;
			while (mask > 0) {
				if ((mask & 1) > 0)
					return n;
				n++;
				mask = mask >> 1;
			}
			throw new Exception("Holyshit!!!");
		}
		#endregion

		#region Private Implementation
		private static long _SetBit(long origin, long mask, bool val)
		{
			if (val) {
				return origin | mask;
			} else {
				return (origin | mask) ^ mask;
			}
		}

		private static long _SetBits(long origin, long mask, long val)
		{
			int order = GetOrder(mask);
			val = val << order;
			origin = (origin | mask) ^ mask;
			return origin | val;
		}

		private static bool _GetBit(long origin, long mask)
		{
			return (origin & mask) > 0;
		}

		private static long _GetBits(long origin, long mask)
		{
			int order = GetOrder(mask);
			return (origin & mask) >> order;
		}
		#endregion
	}
}
