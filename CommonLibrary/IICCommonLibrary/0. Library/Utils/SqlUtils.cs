using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	/// 用于Sql访问的应用类
	/// </summary>
    public static class SqlUtils
    {
        /// <summary>
        ///		将一个list转换为逗号分割的值, 形如(1, 2, 3)
        /// </summary>
        /// <param name="list">列表, 可以为数组, 泛性list等</param>
        /// <param name="bracketed">是否被()包围</param>
        /// <returns></returns>
        public static string ConvertToCsv(IEnumerable list, bool bracketed)
        {
            return ConvertToCsv(list, string.Empty, bracketed);
        }

        /// <summary>
        ///		将一个list转换为逗号分割的值, 对象被单引号包围, 形如('a', 'b', 'c')
        /// </summary>
        /// <param name="list">列表, 可以为数组, 泛性list等</param>
        /// <param name="bracketed">是否被()包围</param>
        /// <returns></returns>
        public static string ConvertToQuotedCsv(IEnumerable list, bool bracketed)
        {
            return ConvertToCsv(list, "'", bracketed);
        }

        /// <summary>
        ///		将一个list转换为逗号分割的值, 形如(1, 2, 3)
        /// </summary>
        /// <param name="list">列表, 可以为数组, 泛性list等</param>
        /// <param name="quotation">引号, 如果不需要请赋值string.Empty</param>
        /// <param name="bracketed">是否被()包围</param>
        /// <returns></returns>
        public static string ConvertToCsv(IEnumerable list, string quotation, bool bracketed)
        {
            StringBuilder ret = new StringBuilder(1024);
            bool first = true;
            if (bracketed)
                ret.Append("(");
            foreach (object o in list)
            {
                if (first)
                    first = false;
                else
                    ret.Append(",");

                if (quotation != string.Empty)
                {
                    ret.Append(quotation);
                    ret.Append(o.ToString());
                    ret.Append(quotation);
                }
                else
                {
                    ret.Append(o.ToString());
                }
            }
            if (bracketed)
                ret.Append(")");
            return ret.ToString();
        }

        /// <summary>
        ///		转换为xml字段，用于传给SQLSERVER，形如"<root><i v="1"/><i v="2"/></root>"
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ConvertItemsToXml(IEnumerable list)
        {
            StringBuilder sb = new StringBuilder(1024);
            //sb.Append(Database.XmlTypePreTag); TODO 新标志
            sb.Append("$");
            //sqlserver
            sb.Append("<root>");
            foreach (object o in list)
                sb.AppendFormat("<i v=\"{0}\"/>", o);
            sb.Append("</root>");
            sb.Append("$");
            //mysql
            foreach (object o in list)
                sb.AppendFormat("{0};", o);
            return sb.ToString().TrimEnd(';');
        }

        /// <summary>
        ///		封装字符串，将null转化为string.Empty""
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string WrapNullString(string s)
        {
            return s == null ? string.Empty : s;
        }

		/// <summary>
		///		自动进行可能是DbNull类型的类型转换
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
        public static T Get<T>(object obj)
        {
            return obj is DBNull ? default(T) : (T)Convert.ChangeType(obj, typeof(T));
        }

        public static T Get<T>(object obj, T nullValue)
        {
            return obj is DBNull ? nullValue : (T)Convert.ChangeType(obj, typeof(T));
        }

        public static List<string> Split(string field, char sparator)
        {
            string[] ss = field.Split(sparator);

            List<string> ret = new List<string>();
            foreach (string s in ss)
            {
                string s2 = s.Trim();
                if (s2.Length > 0)
                {
                    ret.Add(s2);
                }
            }
            return ret;
        }

        //
        //	照这话，约翰来了，在旷野施洗，传悔改的洗礼，使罪得赦。
        //								--- 《马可福音 1:4》 
        //
        public static string BlessGuid(string guid)
        {
            //	correct "CEE99CA1-3B31-462E-A060-A21F7730D1AC"
            if (guid.IndexOf('-') < 0)
            {
                if (guid.Length != 32)
                    throw new FormatException("Corrupted Guid Format:" + guid);

                return string.Format(
                    "{0}-{1}-{2}-{3}-{4}",
                    guid.Substring(0, 8),
                    guid.Substring(8, 4),
                    guid.Substring(12, 4),
                    guid.Substring(16, 4),
                    guid.Substring(20, 12)
                );
            }
            else
                return guid;
        }

        public static bool VerfiyXml(string xml)
        {
            if (xml == string.Empty)
                return true;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool VerfiySmallDateTime(DateTime time)
        {
            if (time >= SmallDateTime_MinValue && time <= SmallDateTime_MaxValue)
                return true;
            else
                return false;
        }

        public static bool VerfiyDateTime(DateTime time)
        {
            if (time >= DateTime_MinValue && time <= DateTime_MaxValue)
                return true;
            else
                return false;
        }

        public static DateTime GetMysqlTimeStamp(DateTime time)
        {
            if (time > MysqlTimeStamp_Max)
                time = MysqlTimeStamp_Max;

            if (time < MysqlTimeStamp_Min)
                time = MysqlTimeStamp_Min;

            return time;
        }

		public static string FormatSql(object obj)
		{
			if (obj == null) {
				return "null";
			}  if (obj.GetType() == typeof(string)) {
				//
				// TODO:  处理换行等
				return "'" + ((string)obj).Replace("'", "''") + "'";
			} else if (obj.GetType() == typeof(DateTime)) {
				return "'" + ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
			} else {
				return obj.ToString();
			}
		}

        public static readonly DateTime SmallDateTime_MinValue = DateTime.Parse("1900-01-01");
        public static readonly DateTime SmallDateTime_MaxValue = DateTime.Parse("2079-06-06");

        public static readonly DateTime MysqlTimeStamp_Min = DateTime.Parse("1970-01-01");
        public static readonly DateTime MysqlTimeStamp_Max = DateTime.Parse("2038-01-01");

        public static readonly DateTime DateTime_MinValue = DateTime.Parse("1753-01-01");
        public static readonly DateTime DateTime_MaxValue = DateTime.Parse("9999-12-31");
    }
}
