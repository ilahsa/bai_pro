using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class EnumParser
	{
		public static object Parse(string enumValue, Type enumType, string defaultValue, bool ignoreUnknown)
		{
			bool isFlags = AttributeHelper.TryGetAttribute<FlagsAttribute>(enumType) != null;

			try {
				long n = 0;
				if (enumValue.StartsWith("0x") || enumValue.StartsWith("0X")) {
					string hex = enumValue.Substring(2);
					if (hex.Length > 8) {
						throw new FormatException("Flag HexToLong: " + enumValue);
					}

					n = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
					return Enum.ToObject(enumType, n);
				} else if (long.TryParse(enumValue, out n)) {
					return Enum.ToObject(enumType, n);
				} else if (isFlags && ignoreUnknown) {
					return ParseFlags(enumValue, enumType, true);
				} else {
					return Enum.Parse(enumType, enumValue, true);
				}
			} catch (Exception ex) {
				if (defaultValue == null) {
					throw new FormatException("EnumParse Failed:" + enumValue, ex);
				} else {
					return Parse(defaultValue, enumType, "", ignoreUnknown);
				}
			}
		}

		public static object ParseFlags(string enumValue, Type enumType, bool ignoreUnknown)
		{
			long n = 0;
			foreach (string s in enumValue.Split(',')) {
				try {
					object e = Enum.Parse(enumType, s, true);
					n |= Convert.ToInt64(e);
				} catch (Exception ex) {
					if (!ignoreUnknown) {
						throw new FormatException("Flag Parse Failed:" + enumValue, ex);
					}
				}
			}
			return Enum.ToObject(enumType, n);
		}
	}
}
