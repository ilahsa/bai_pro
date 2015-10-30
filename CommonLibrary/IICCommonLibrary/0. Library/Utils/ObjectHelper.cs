/*
 * Lei Gao
 * 2009-08-20
 * 
 */
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Web.Script.Serialization;


namespace Imps.Services.CommonV4
{
	public static class ObjectHelper
	{
		public static string ToString(object obj)
		{
			return obj == null ? "" : obj.ToString();
		}

		public static object ConvertTo(string value, Type valueType)
		{
			if (valueType.IsEnum) {
				int p = value.IndexOf('|');
				if (p > 0) {
					string v = value.Substring(0, p);
					string d = value.Substring(p + 1);
					return EnumParser.Parse(v, valueType, d, true);
				} else {
					return EnumParser.Parse(value, valueType, null, true);
				}
			} else if (valueType == typeof(TimeSpan)) {
				return TimeSpan.Parse(value);
			} else if (valueType == typeof(bool)) {
				switch (value.ToLower()) {
					case "true":
					case "1":
						return true;
					case "false":
					case "0":
						return false;
					default:
						throw new FormatException("parse bool value failed:" + value);
				}
			} else {
				try {
					return Convert.ChangeType(value, valueType);
				} catch (Exception) {
					throw new FormatException(string.Format("Convert Failed <{1}>:{0}", value, valueType.Name));
				}
			}
		}

		public static T ConvertTo<T>(string value)
		{
			return (T)ConvertTo(value, typeof(T));
		}

		// TODO
		// 基于Object的TypeConvert
		public static T ConvertTo<T>(object value)
		{
			return (T)ConvertTo<T>(value.ToString());
		}

		public static void SetValue(FieldInfo field, object owner, object value)
		{
			field.SetValue(owner, value);
		}

		public static void SetValue(FieldInfo field, object owner, string value, Type valueType)
		{
			object valueObj = ObjectHelper.ConvertTo(value, valueType);
			field.SetValue(owner, valueObj);
		}

		public static string DumpObject(object obj)
		{
			return DumpObject(obj, "", 4);
		}
		
		public static string DumpObject(object obj, string objectName)
		{
			return DumpObject(obj, objectName, 4);
		}

		public static string DumpObject(object obj, string objectName, int maxDepth)
		{
			if (obj == null) {
				return "<null>";
			} else {
				StringBuilder str = new StringBuilder(8192);
				ObjectDumper.DumpInner(str, obj, "objectName", 0, maxDepth);
				return str.ToString();
			}
		}

		public static string GetTypeName(Type type, bool fullName)
		{
			if (type == null) {
				return "<NULL>";
			} if (!type.IsGenericType) {
				return fullName ? type.FullName : type.Name;
			} else {
				StringBuilder str = new StringBuilder();
				if (fullName) {
					str.Append(type.Namespace);
					str.Append('.');
				}
				string name = type.Name;
				int index = name.LastIndexOf("`");
				str.Append(index > 0 ? name.Substring(0, index) : name);
				str.Append('<');
				bool first = true;
				foreach (Type param in type.GetGenericArguments()) {
					if (first) {
						first = false;
					} else {
						str.Append(", ");
					}
					str.Append(GetTypeName(param, false));

				}
				str.Append('>');

				return str.ToString();
			}
		}

		public static object GetFieldValue(object obj, string fieldName)
		{
			FieldInfo fi;
			Type t;

			t = obj.GetType();
			fi = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (fi == null)
				return null;
			else
				return fi.GetValue(obj);
		}


		public static int CompatibleGetHashCode(int n)
		{
			return n;
		}

		public static int CompatibleGetHashCode(short n)
		{
			return n;
		}

		public static int CompatibleGetHashCode(object obj)
		{
			return obj.GetHashCode();
		}

		public static int CompatibleGetHashCode(byte n)
		{
			return (int)n | ((int)n << 8) | ((int)n << 16) | ((int)n << 24);
		}

		public static int CompatibleGetHashCode(long n)
		{
			return ((int)n) ^ ((int)(n >> 32));
		}

		public static int CompatibleGetHashCode(DateTime time)
		{
			long internalTicks = time.Ticks;
			return (((int)internalTicks) ^ ((int)(internalTicks >> 32)));
		}

		/// <summary>
		///		为了X32和X64的兼容, 和32bit的实现一样
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int CompatibleGetHashCode(string s)
		{
			int num = 0x15051505;
			int num2 = num;

			int np;
			int j = 0;
			int length = s.Length;
			for (int i = length; i > 0; i -= 4) {
				np = (length <= j) ? 0 : s[j] | ((length > j + 1 ? s[j + 1] : 0) << 16);
				num = (((num << 5) + num) + (num >> 0x1b)) ^ np;
				if (i <= 2)
					break;

				j += 2;
				np = (length <= j) ? 0 : s[j] | ((length > j + 1 ? s[j + 1] : 0) << 16);
				num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ np;
				j += 2;
			}
			return (num + (num2 * 0x5d588b65));
		}

        // 扩展方法，转换成JSON格式，记录日志和发送邮件附件消息使用
        public static string ToJSONString(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
	}
}
