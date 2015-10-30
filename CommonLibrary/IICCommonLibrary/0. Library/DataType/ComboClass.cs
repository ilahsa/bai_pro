using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class ComboClass<T>
	{
		public T Value;

		public ComboClass()
		{
		}

		public ComboClass(T val)
		{
			Value = val;
		}

		public override bool Equals(object obj)
		{
			if (obj == null && this.GetType() != obj.GetType()) {
				return false;
			} else {
				ComboClass<T> rval = (ComboClass<T>)obj;
				return ComboClass<T>.SafeEquals(this.Value, rval.Value);
			}
		}

		public override int GetHashCode()
		{
			return SafeGetHashCode(Value);
		}

		public override string ToString()
		{
			return Value == null ? "NULL" : Value.ToString();
		}

		internal static bool SafeEquals(T v1, T v2)
		{
			if (Object.ReferenceEquals(v1, null)) {
				return Object.ReferenceEquals(v2, null);
			}  else {
				return v1.Equals(v2);
			}
		}

		internal static int SafeGetHashCode(T v)
		{
			return v == null ? 0 : v.GetHashCode();
		}
	}

	public class ComboClass<T1, T2>
	{
		public T1 V1;
		public T2 V2;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2)
		{
			V1 = v1;
			V2 = v2;
		}

		public override bool Equals(object obj)
		{
			if (obj == null && this.GetType() != obj.GetType()) {
				return false;
			} else {
				ComboClass<T1, T2> rval = (ComboClass<T1, T2>)obj;
				return ComboClass<T1>.SafeEquals(this.V1, rval.V1) &&
					ComboClass<T2>.SafeEquals(this.V2, rval.V2);
			}
		}

		public override int GetHashCode()
		{
			return ComboClass<T1>.SafeGetHashCode(V1) ^
				ComboClass<T2>.SafeGetHashCode(V2);
		}

		public override string ToString()
		{
			return String.Format("V1:{0} V2:{1}", V1, V2);
		}
	}

	public class ComboClass<T1, T2, T3>
	{
		public T1 V1;
		public T2 V2;
		public T3 V3;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2, T3 v3)
		{
			V1 = v1;
			V2 = v2;
			V3 = v3;
		}

		public override bool Equals(object obj)
		{
			if (obj == null && this.GetType() != obj.GetType()) {
				return false;
			} else {
				ComboClass<T1, T2, T3> rval = (ComboClass<T1, T2, T3>)obj;
				return ComboClass<T1>.SafeEquals(this.V1, rval.V1) &&
					ComboClass<T2>.SafeEquals(this.V2, rval.V2) &&
					ComboClass<T3>.SafeEquals(this.V3, rval.V3);
			}
		}

		public override int GetHashCode()
		{
			return ComboClass<T1>.SafeGetHashCode(V1) ^
				ComboClass<T2>.SafeGetHashCode(V2) ^
				ComboClass<T3>.SafeGetHashCode(V3);
		}

		public override string ToString()
		{
			return String.Format("V1:{0} V2:{1} V3:{2}", V1, V2, V3);
		}
	}

	public class ComboClass<T1, T2, T3, T4>
	{
		public T1 V1;
		public T2 V2;
		public T3 V3;
		public T4 V4;

		public ComboClass()
		{
		}

		public ComboClass(T1 v1, T2 v2, T3 v3, T4 v4)
		{
			V1 = v1;
			V2 = v2;
			V3 = v3;
			V4 = v4;
		}

		public override bool Equals(object obj)
		{
			if (obj == null && this.GetType() != obj.GetType()) {
				return false;
			} else {
				ComboClass<T1, T2, T3, T4> rval = (ComboClass<T1, T2, T3, T4>)obj;
				return ComboClass<T1>.SafeEquals(this.V1, rval.V1) &&
					ComboClass<T2>.SafeEquals(this.V2, rval.V2) &&
					ComboClass<T3>.SafeEquals(this.V3, rval.V3) &&
					ComboClass<T4>.SafeEquals(this.V4, rval.V4);
			}
		}

		public override int GetHashCode()
		{
			return ComboClass<T1>.SafeGetHashCode(V1) ^
				ComboClass<T2>.SafeGetHashCode(V2) ^
				ComboClass<T3>.SafeGetHashCode(V3) ^
				ComboClass<T4>.SafeGetHashCode(V4);
		}

		public override string ToString()
		{
			return String.Format("V1:{0} V2:{1} V3:{2} V4:{3}", V1, V2, V3, V4);
		}
	}
}
