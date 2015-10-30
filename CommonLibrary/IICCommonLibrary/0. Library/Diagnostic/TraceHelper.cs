using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Imps.Services.CommonV4
{
	public static class TracingHelper
	{
		public static string FormatMessage(string format, params object[] args)
		{
			try {
				return string.Format(format, args);
			} catch (Exception) {
				StringBuilder str = new StringBuilder();
				str.Append("FormatFailed: \"");
				str.Append(format);
				str.AppendFormat("\" Args({0}): ", args.Length);
				foreach (object obj in args) {
					str.Append(obj);
					str.Append(",");
				}
				return str.ToString();
			}
		}

		public static string FormatThreadInfo(Thread trd)
		{
			if (trd.IsThreadPoolThread) {
				return string.Format("P:{0}", trd.ManagedThreadId, trd.Name);
			} else if (trd.IsBackground) {
				if (string.IsNullOrEmpty(trd.Name))
					return string.Format("B:{0}", trd.ManagedThreadId, trd.Name);
				else
					return string.Format("B:{0} - {1}", trd.ManagedThreadId, trd.Name);
			} else {
				if (string.IsNullOrEmpty(trd.Name))
					return string.Format("{0}", trd.ManagedThreadId, trd.Name);
				else
					return string.Format("{0} - {1}", trd.ManagedThreadId, trd.Name);
			}
		}

		public static string FormatMethodInfo(MethodBase method)
		{
			StringBuilder sb = new StringBuilder();
			ParameterInfo[] pis = method.GetParameters();
			foreach (ParameterInfo pi in pis) {
				string pName = pi.Name;

			}
			MethodBody body = method.GetMethodBody();
			IList<LocalVariableInfo> lvis = body.LocalVariables;
			foreach (LocalVariableInfo lvi in lvis) {

			}
			return sb.ToString();
		}

        public static string ReplaceNullChars(string input)
        {
            if (string.IsNullOrEmpty(input)) {
                return input;
            }
            List<int> list = new List<int>();
            for (int i = 0; i < input.Length; i++) {
                if (input[i] == '\0') {
                    list.Add(i);
                }
            }
            if (list.Count <= 0) {
                return input;
            }
            StringBuilder builder = new StringBuilder(input.Length + list.Count);
            int startIndex = 0;
            foreach (int num3 in list) {
                builder.Append(input.Substring(startIndex, num3 - startIndex));
                builder.Append(@"\0");
                startIndex = num3 + 1;
            }
            builder.Append(input.Substring(startIndex));
            return builder.ToString();
        }

        public static string ReplaceNulls(object input)
        {
            if (input == null) {
                return "<null>";
            }
            string str = input.ToString();
            if (str == null) {
                return "<object>";
            }
            return str;
        }
	}
}
