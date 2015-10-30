using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class AttributeHelper
	{
		public static T GetAttribute<T>(MemberInfo member) where T: Attribute
		{
			return GetAttribute<T>(member.GetCustomAttributes(typeof(T), false), member.Name, false);
		}

		public static T TryGetAttribute<T>(MemberInfo member) where T: Attribute
		{
			return GetAttribute<T>(member.GetCustomAttributes(typeof(T), false), member.Name, true);
		}

		private static T GetAttribute<T>(object[] attrs, string hostName, bool isTry)
		{
			if (attrs.Length == 0) {
				if (isTry)
					return default(T);
				else
					throw new Exception(string.Format("Attribute{0} not found in {1}", typeof(T).Name, hostName));
			}

			if (attrs.Length > 1)
				throw new Exception(string.Format("More than 1 Attribute{0} found in {1}", typeof(T).Name, hostName));

			if (attrs[0] is T)
				return (T)attrs[0];
			else
				throw new Exception("Unknown Type:" + typeof(T).Name);
		}
	}
}
