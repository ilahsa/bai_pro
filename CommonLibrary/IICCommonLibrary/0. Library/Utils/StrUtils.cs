using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;

namespace Imps.Services.CommonV4
{
	/// <summary>
	/// Summary description for StrUtils.
	/// </summary>
	public class StrUtils
	{
		public static string Trim(string str)
		{
			string rt = "";
			string filter = " 　,，#＃~";
			int i = 0;
			for (i = 0; i < str.Length; i++) {
				if (filter.IndexOf(str[i]) < 0)
					break;

			}
			rt = str.Substring(i);
			for (i = rt.Length - 1; i >= 0; i--) {
				if (filter.IndexOf(rt[i]) < 0)
					break;

			}
			if (i + 1 <= 0)
				return rt;
			else
				rt = rt.Substring(0, i + 1);
			return rt;
		}

		public static string XmlHelperDecode(string str)
		{
			/*
			//string rt = "";
			str = str.Replace("&lt;", "<");
			str = str.Replace("&gt;", ">");
			str = str.Replace("&quot;", "\"");
			str = str.Replace("&apos;", "\'");
			str = str.Replace("&amp;", "&");
			return str;
			 * */
			return HttpUtility.HtmlDecode(str);
		}

		public static bool CheckPwd(string pwd)
		{
			foreach (char c in pwd)
				if (c < 0x21 || c > 0x7F)
					return false;
			return true;
		}

		public static string[] GetnewStrArry(int count)
		{
			string[] strs = new string[count];
			for (int i = 0; i < count; i++)
				strs[i] = "";
			return strs;
		}


		/// <summary>
		/// 截取0-len长度的string返回,out 剩下的。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="str"></param>
		/// <param name="len"></param>
		/// <returns>截取0-len长度的string返回,out 剩下的</returns>
		public static string Substring(out string left, string str, int len)
		{
			if (str.Length <= len) {
				left = "";
				return str;
			} else {
				left = str.Substring(len);
				return str.Substring(0, len);
			}
		}

		public static string Substring(string str, int start, int len)
		{
			if (str.Length <= start)
				return "";
			else
				return str.Substring(start, len);
		}

		public static string Substring(string str, int len)
		{
			if (str.Length <= len)
				return str;
			else
				return str.Substring(0, len);
		}

		public static string GetNullStr(string str)
		{
			if (str == null || str == string.Empty)
				return "";
			else return
				str;
		}

		public static string GetNullStr(int i)
		{
			string str = "";
			try {
				str = i.ToString();

			} catch {
				return str;
			}
			if (str == null || str == string.Empty)
				return "";
			else return
				str;
		}

		public static string GetNullStr(long i)
		{
			string str = "";
			try {
				str = i.ToString();

			} catch {
				return str;
			}
			if (str == null || str == string.Empty)
				return "";
			else return
				str;
		}

		public static string GetFirstChar(string str)
		{
			str = str.Trim().ToUpper();
			str = ToDBC(str);
			if (str.Length > 0)
				return str.Substring(0, 1);
			else
				return "";
		}
		public static string GetFormatStr(string str)
		{
			str = str.Trim().ToUpper();
			str = ToDBC(str);
			return str;
		}


		public static bool IsNum(string str)
		{
            if (string.IsNullOrEmpty(str))
                return false;
			bool issid = false;
			string regex = @"^(?<num>[0-9]*$)";//考滤放配置文件
			issid = Regex.IsMatch(str, regex, RegexOptions.Compiled);
			return issid;
		}

		public static int Str2int(string str, int def)
		{

			try {
				if (str.Length == 0)
					return def;
				str = ToDBC(str);
				return int.Parse(str);
			} catch {

				return def;
			}

		}

		public static long Str2int(string str, long def)
		{

			try {
				if (str.Length == 0)
					return def;
				str = ToDBC(str);
				return long.Parse(str);
			} catch {

				return def;
			}

		}
		/// <summary>
		/// i.tostring().length>length return i.tostring(
		/// </summary>
		/// <param name="i"></param>
		/// <param name="length"></param>
		/// <param name="fillwith"></param>
		/// <returns></returns>
		public static string Int2str(int i, int length, string fillwith)
		{
			string rt = i.ToString();
			if (fillwith.Length == 0)
				return rt;
			if (i.ToString().Length >= length)
				return rt;

			rt = string.Format("{" + fillwith + ":D" + length.ToString() + "}", i);
			return rt;
		}

		//	确保字符串以指定的字符或字符串结尾(如果不是则自动添加)
		public static string EnsureEndsWith(ref string str, char suffix)
		{
			string suffixStr = new string(suffix, 1);
			if (!str.EndsWith(suffixStr))
				str += suffixStr;
			return str;
		}

		public static string EnsureEndsWith(ref string str, string suffix)
		{
			if (!str.EndsWith(suffix))
				str += suffix;
			return str;
		}

		//	追加字符串，并且适当地添加逗号
		public static string AddByComma(ref string dest, string src)
		{
			return AddBySeperator(ref dest, src, ",", true);
		}

		public static string AddByComma(ref string dest, string src, bool addSpace)
		{
			return AddBySeperator(ref dest, src, ",", addSpace);
		}

		//	追加字符串，并且适当地添加分隔符
		public static string AddBySeperator(ref string dest, string src, string seperator)
		{
			return AddBySeperator(ref dest, src, seperator, false);
		}

		public static string AddBySeperator(ref string dest, string src, string seperator, bool addSpace)
		{
			if (dest == string.Empty)
				dest = src;
			else {
				if (addSpace)
					dest += seperator + " " + src;
				else
					dest += seperator + src;
			}
			return dest;
		}

		//作者：陈省
		//判断字符串是否是数字
		public static bool IsDigit(string s)
		{
			foreach (char c in s) {
				if (!Char.IsDigit(c))
					return false;
			}
			return true;
		}

		public static string SafeTruncate(string str, int len)
		{
			if (str.Length <= len)
				return str;
			else
				return str.Substring(0, len);
		}


		public static string TruncateUnicodeString(string str, int len)
		{
			return TruncateString(str, len, Encoding.Unicode);
		}

		//	按照Unicode宽度来截断字符串(即普通字符算1个字节，汉字算2个字节)
		public static string TruncateString(string str, int len, Encoding coding)
		{
			int n = str.Length;
			while (n > 0) {
				string s = str.Substring(0, n);
				if (coding.GetByteCount(s) <= len)
					return s;
				n--;
			}
			return string.Empty;
		}

		public static string MakeHexString(byte b)
		{
			return string.Format("{0,2:X}", b).Replace(' ', '0');
		}

		public static string MakeHexString(byte[] bytes)
		{
			if (bytes == null)
				return string.Empty;
			StringBuilder sb = new StringBuilder(512);
			for (int i = 0; i < bytes.Length; i++) {
				if (i == 0)
					sb.Append(MakeHexString(bytes[i]));
				else
					sb.Append(" " + MakeHexString(bytes[i]));
			}
			return sb.ToString();
		}
		/// <summary>
		/// 检查前len位是不是全半角
		/// </summary>
		/// <param name="str"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public static bool IsAllDBC(string str, int len)
		{
			bool rt = true;
			for (int i = 0; i < len && i < str.Length; i++)
				if (str[i] > '\x00FE')
					return false;
			return rt;
		}
		/// <summary>
		/// 是否全半角字串。
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsAllDBC(string str)
		{
			bool rt = true;
			for (int i = 0; i < str.Length; i++)
				if (str[i] > '\x00FE')
					return false;
			return rt;
		}
        /// <summary>
        /// 一个字符转半角
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static char ToDBC(char a)
        {
            if ((a >= '!' && a <= '~'))
                return a;
            else if ((a >= '！' && a <= '～'))
                return (char)((a & '\x00ff') + '\x0020');
            else
                return a;
        }

        ///// <summary>
        ///// 一个字串转半角
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static string ToDBC(string a)
        //{
        //    string rtstr = "";
        //    for (int i = 0; i < a.Length; i++)
        //        rtstr += ToDBC(a[i]);
        //    return rtstr;
        //}

        //修改说明：
        //考虑到原全半角转换方法内存开销巨大的问题，故将其替换
        //ToDBC(string a)与ToSBC(string a)已被新方法代替，但为方便回滚，故仅注释掉
        //但不知其关联的ToDBC(char a)和ToSBC(char a)是否被他其他方法依赖，故暂时保留
        //新方法经开发测试，未发现问题，但实际效果仍需测试人员测试确认

        /// <summary>一个字串转半角</summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 一个字符转全角
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static char ToSBC(char a)
        {
            if ((a >= '！' && a <= '～'))
                return a;
            else if ((a >= '!' && a <= '~'))
                return (char)((a - '\x0020') | '\xff00');
            else
                return a;
        }

        ///// <summary>
        ///// 一个字串转全角
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static string ToSBC(string a)
        //{
        //    string rtstr = "";
        //    for (int i = 0; i < a.Length; i++)
        //        rtstr += ToSBC(a[i]);
        //    return rtstr;
        //}

        /// <summary>
        /// 一个字串转全角
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

		public static string ToHexString(byte[] binary, int lenLimit)
		{
			StringBuilder str = new StringBuilder();
			int len = 0;
			foreach (byte b in binary) {
				if (len >= lenLimit) {
					str.Append("...");
					break;
				}
				if (b > 15)
					str.AppendFormat("{0:X}", b);
				else
					str.AppendFormat("0{0:X}", b);

				len++;
			}

			return str.ToString();
		}

		public static string ToHexString(byte[] buffer, int offset, int count)
		{
			StringBuilder str = new StringBuilder();
			for (int i = offset; i < offset + count; i++) {
				byte b = buffer[i];
				if (b > 0x0f)
					str.AppendFormat("{0:X}", b);
				else
					str.AppendFormat("0{0:X}", b);
			}
			return str.ToString();
		}

		public static string ToHexString(byte[] binary)
		{
			return ToHexString(binary, int.MaxValue);
		}

		public static byte[] FromHexString(string hex)
		{
			if (hex == null || hex.Length < 1)
				return new byte[0];

			int len = hex.Length / 2;
			byte[] result = new byte[len];
			len *= 2;

			for (int index = 0; index < len; index++) {
				string s = hex.Substring(index, 2);
				int b = int.Parse(s, NumberStyles.HexNumber);
				result[index / 2] = (byte)b;
				index++;
			}

			return result;
		}

		/// <summary>
		/// 截取字符串，全角长度算2个
		/// </summary>
		/// <param name="source"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string TruncateStringWithDBC(string source, int length)
		{
			int dbcCount = 0;
			for (int i = 0; i < length && i < source.Length; i++) {
				//全角字符
				if (source[i] > '\x00FE')
					dbcCount += 2;
				else
					dbcCount += 1;
				if (dbcCount >= length)
					return source.Substring(0, i + 1);
			}
			return source;
		}

        /// <summary>
        /// 用分隔符区分，返回第几个参数。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index">从1开始。</param>
        /// <returns></returns>
        public static string getStrBySplitWord(string str, int index)
        {
            string rt = "";
            index--;
            int splitIndex = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (IsSplitWord(str[i]))
                {
                    splitIndex++;
                    if (splitIndex > index)
                        return rt;
                    else
                        rt = "";

                }
                else
                    rt += str[i];
            }
            if (splitIndex < index)
                return "";
            else
                return rt;
        }

        public static bool IsSplitWord(char c)
        {
            if (" 　,，#＃~".IndexOf(c) >= 0)
                return true;
            return false;
        }

        public static void AppendTabs(StringBuilder str, int level)
        {
            for (int i = 0; i < level; i++)
            {
                str.Append("\t");
            }
        }
	}
}
