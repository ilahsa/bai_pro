using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
    #region 格式化dump方法
    interface IDumpable<T> { bool DumpInfo(T T1, StringBuilder str, string fieldName, int level, int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac); }

    public class ListDumpable<T> : IDumpable<T>
    {
        #region IDumpable Members

        public bool DumpInfo(T T1, StringBuilder str, string fieldName, int level, int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac)
        {
            bool isContinue = true;
            IList list = (IList)T1;

            StrUtils.AppendTabs(str, level);
            str.AppendFormat("{0}:{1} {{\r\n", fieldName, ObjectHelper.GetTypeName(list.GetType(), false));
            for (int i = 0; i < list.Count; i++)
            {
                Type type = list[i].GetType();
                if (type.IsValueType || type == typeof(string))
                {
                    StrUtils.AppendTabs(str, level);
                    str.AppendFormat("{0}[{1}]= {2}\r\n", fieldName, i, list[i]);

                    if (str.Length > 8192)
                    {
                        str.Append("...TOO LONG...");
                        isContinue = false;
                    }

                    if (!isContinue)
                        break;
                }
                else
                {
                    isContinue = ac.Invoke(str, list[i], fieldName, level, maxDepth);

                    if (!isContinue)
                        break;
                }
            }
            StrUtils.AppendTabs(str, level);
            str.Append("}\r\n");

            return isContinue;
        }

        #endregion
    }
    public class DictionaryDumpable<T> : IDumpable<T>
    {
        #region IDumpable Members

        public bool DumpInfo(T T1, StringBuilder str, string fieldName, int level, int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac)
        {
            bool isContinue = true;
            IDictionary dict = (IDictionary)T1;

            StrUtils.AppendTabs(str, level);
            str.AppendFormat("{0}:{1} {{\r\n", fieldName, ObjectHelper.GetTypeName(dict.GetType(), false));
            IDictionaryEnumerator e = dict.GetEnumerator();
            bool Iskey = false;
            bool isValue = false;

            while (e.MoveNext())
            {
                Iskey = e.Key.GetType().IsValueType || e.Key.GetType() == typeof(string);
                isValue = e.Value.GetType().IsValueType || e.Value.GetType() == typeof(string);

                if (Iskey)
                {
                    StrUtils.AppendTabs(str, level);
                    str.AppendFormat("[{0}, ",e.Key);

                    if (str.Length > 8192)
                    {
                        str.Append("...TOO LONG...");
                        isContinue = false;
                    }

                    if (!isContinue)
                        break;
                }
                else
                {
                    isContinue = ac.Invoke(str, e.Key, "Key", level, maxDepth);
                    if (!isContinue)
                        break;
                }

                if (isValue)
                {
                    str.AppendFormat("{0}]\r\n ", e.Value);

                    if (str.Length > 8192)
                    {
                        str.Append("...TOO LONG...");
                        isContinue = false;
                    }

                    if (!isContinue)
                        break;
                }
                else
                {
                    isContinue = ac.Invoke(str, e.Value, "Value", level,maxDepth);
                    str.Append("\r\n");
                    if (!isContinue)
                        break;
                }
            }
            StrUtils.AppendTabs(str, level);
            str.Append("}\r\n");

            return isContinue;
        }

        #endregion
    }
    public class CollectionDumpable<T> : IDumpable<T>
    {
        #region IDumpable Members

        public bool DumpInfo(T T1, StringBuilder str, string fieldName, int level, int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac)
        {
            bool isContinue = true;
            ICollection coll = (ICollection)T1;

            StrUtils.AppendTabs(str, level);
            str.AppendFormat("{0}:{1} {{\r\n", fieldName, ObjectHelper.GetTypeName(coll.GetType(), false));
            IEnumerator ie = ((IEnumerable)coll).GetEnumerator();
            int i = 0;
            while (ie.MoveNext())
            {
                if (ie.Current.GetType().IsValueType || ie.Current.GetType() == typeof(string))
                {
                    StrUtils.AppendTabs(str, level);
                    str.AppendFormat("{0}[{1}]= {2}\r\n", fieldName, i, ie.Current);

                    if (str.Length > 8192)
                    {
                        str.Append("...TOO LONG...");
                        isContinue = false;
                    }

                    if (!isContinue)
                        break;
                }
                else
                {
                    isContinue = ac.Invoke(str, ie.Current, fieldName, level, maxDepth);
                    str.Append("\r\n");
                    if (!isContinue)
                        break;
                }
                i++;
            }
           
            StrUtils.AppendTabs(str, level);
            str.Append("}\r\n");

            return isContinue;
        }

        #endregion
    }
    public class EnumerableDumpable<T> : IDumpable<T>
    {
        #region IDumpable Members

        public bool DumpInfo(T T1, StringBuilder str, string fieldName, int level, int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac)
        {
            bool isContinue = true;
            IEnumerator ie = ((IEnumerable)T1).GetEnumerator();

            StrUtils.AppendTabs(str, level);
            str.AppendFormat("{0}:{1} {{\r\n", fieldName, ObjectHelper.GetTypeName(T1.GetType(), false));
            int i = 0;
            while (ie.MoveNext())
            {
                if (ie.Current.GetType().IsValueType || ie.Current.GetType() == typeof(string))
                {
                    StrUtils.AppendTabs(str, level);
                    str.AppendFormat("{0}[{1}]= {2}\r\n", fieldName, i, ie.Current);

                    if (str.Length > 8192)
                    {
                        str.Append("...TOO LONG...");
                        isContinue = false;
                    }

                    if (!isContinue)
                        break;
                }
                else
                {
                    isContinue = ac.Invoke(str, ie.Current, fieldName, level, maxDepth);
                    str.Append("\r\n");
                    if (!isContinue)
                        break;
                }
                i++;
            }
            StrUtils.AppendTabs(str, level);
            str.Append("}\r\n");

            return isContinue;
        }

        #endregion
    }

    #endregion

    public static class ObjectDumper
	{
		public static bool DumpInner(StringBuilder str, object obj, string fieldName, int level, int maxDepth)
		{
            bool isContinue = true;

			if (str.Length > 8192) {
				str.Append("...TOO LONG...");
				return false;
			}

            if (level > maxDepth)
            {
                str.Append("...MAX DEPTH...");
                return true;
            }

			if (obj == null) {
				DumpValue(str, obj, fieldName, level, maxDepth);
				return true;
			}

			Type type = obj.GetType();

			if (type.IsValueType || type == typeof(string) || type == typeof(byte[])) {
				DumpValue(str, obj, fieldName, level, maxDepth);
				return true;
			}
            else 
            {
                Func<StringBuilder, object, string, int, int, bool> ac = DumpInner;
                bool IsAssign= IsAssignInstance(obj, str, fieldName, level,maxDepth, ac,out isContinue);

                if (IsAssign)
                    return isContinue;
            }

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (fields.Length == 0) {
				DumpValue(str, obj, fieldName, level, maxDepth);
				return true;
			} else {
                StrUtils.AppendTabs(str, level);
				string typeName = ObjectHelper.GetTypeName(obj.GetType(), false);
				str.AppendFormat("{0}: {1} {{\r\n", fieldName, typeName, obj);
				foreach (FieldInfo field in fields) {
					object fieldObj = field.GetValue(obj);
					isContinue = DumpInner(str, fieldObj, field.Name, level + 1, maxDepth);

                    if (!isContinue)
                        break;
				}
                StrUtils.AppendTabs(str, level);
				str.Append("}");
			}
            return isContinue;
		}

		public static bool DumpValue(StringBuilder str, object obj, string fieldName, int level, int maxDepth)
		{
            StrUtils.AppendTabs(str, level);
			if (obj == null) {
				str.AppendFormat("{0} = null\r\n", fieldName);
				return true;
			}

			IEnumerable e = obj as IEnumerable;
			string typeName = ObjectHelper.GetTypeName(obj.GetType(), false);

			if (e == null || obj.GetType() == typeof(string)) {
				str.AppendFormat("{0}: {1} = {2}\r\n", fieldName, typeName, obj);
			} else if (obj.GetType() == typeof(byte[])) {
				var b = obj as byte[];
				str.AppendFormat("{0}: byte[] = {{ ", fieldName);
				if (b.Length > 16) {
					str.Append("\r\n");
					StrUtils.AppendTabs(str, level + 1);
				}
				for (int i = 0; i < b.Length; i++) {
					if (i % 16 == 0 && i > 0) {
						str.Append("\r\n");
                        StrUtils.AppendTabs(str, level + 1);
					}
					str.AppendFormat("{0:X2}", b[i]);
					if (i != b.Length - 1)
						str.Append(",");
				}
				str.Append(" }}");
				str.Append("\r\n");
			} else {
				int i = 0;
				str.AppendFormat("{0}: {1} = {{\r\n", fieldName, typeName);
				foreach (object o in e) {
					string s = string.Format("{0}[{1}]", fieldName, i + 1);
					if (!DumpInner(str, o, fieldName, level + 1, maxDepth))
						return false;
				}
				str.Append("}\r\n");
			}
			return true;
		}

        public static bool IsAssignInstance(object obj, StringBuilder str, string fieldName, int level,int maxDepth, Func<StringBuilder, object, string, int, int, bool> ac,out bool isContinue)
        {
            isContinue = true;

            if (typeof(IList).IsInstanceOfType(obj))
            {
                IDumpable<IList> dump = new ListDumpable<IList>();
                isContinue = dump.DumpInfo((IList)obj, str, fieldName, level, maxDepth, ac);
                return true;
            }
            else if (typeof(IDictionary).IsInstanceOfType(obj))
            {
                IDumpable<IDictionary> dump = new DictionaryDumpable<IDictionary>();
                isContinue = dump.DumpInfo((IDictionary)obj, str, fieldName, level, maxDepth, ac);
                return true;
            }
            else if (typeof(ICollection).IsInstanceOfType(obj))
            {
                IDumpable<ICollection> dump = new CollectionDumpable<ICollection>();
                isContinue = dump.DumpInfo((ICollection)obj, str, fieldName, level, maxDepth, ac);
                return true;
            }
            else if (typeof(IEnumerable).IsInstanceOfType(obj))
            {
                IDumpable<IEnumerable> dump = new EnumerableDumpable<IEnumerable>();
                isContinue = dump.DumpInfo((IEnumerable)obj, str, fieldName, level, maxDepth, ac);
                return true;
            }

            return false;
        }
    
    }
}
