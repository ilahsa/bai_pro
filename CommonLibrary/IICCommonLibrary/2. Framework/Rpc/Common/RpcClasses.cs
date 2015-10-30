using System;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4
{
	[ProtoContract]
	public class RpcClass<T>
	{
		[ProtoMember(1)]
		public T Value;

		public RpcClass()
		{
		}

		public RpcClass(T v)
		{
			Value = v;
		}

		public override bool Equals(object obj)
		{
			RpcClass<T> rval = obj as RpcClass<T>;
			if (obj == null) {
				return false;
			} else {
				return this.Value.Equals(rval.Value);
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("RpcClass<{0}>:{1}", typeof(T).Name, Value);
		}

		public static bool operator ==(RpcClass<T> lval, RpcClass<T> rval)
		{
			// If both are null, or both are same instance, return true.
			if (Object.ReferenceEquals(lval, rval)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)lval == null) || ((object)rval == null)) {
				return false;
			}

			// Return true if the fields match:
			return lval.Equals(rval);
		}

		public static bool operator !=(RpcClass<T> lval, RpcClass<T> rval)
		{
			return !(lval == rval);
		}
	}

	[ProtoContract]
	public class RpcClass<T1, T2>
	{
		[ProtoMember(1)]
		public T1 Value1;

		[ProtoMember(2)]
		public T2 Value2;

		public RpcClass()
		{
		}

		public RpcClass(T1 v1, T2 v2)
		{
			Value1 = v1;
			Value2 = v2;
		}
	}
	
	[ProtoContract]
	public class RpcClass<T1, T2, T3>
	{
		[ProtoMember(1)]
		public T1 Value1;

		[ProtoMember(2)]
		public T2 Value2;

		[ProtoMember(3)]
		public T3 Value3;

		public RpcClass()
		{
		}

		public RpcClass(T1 v1, T2 v2, T3 v3)
		{
			Value1 = v1;
			Value2 = v2;
			Value3 = v3;
		}
	}

	[ProtoContract]
	public class RpcClass<T1, T2, T3, T4>
	{
		[ProtoMember(1)]
		public T1 Value1;

		[ProtoMember(2)]
		public T2 Value2;

		[ProtoMember(3)]
		public T3 Value3;

		[ProtoMember(4)]
		public T4 Value4;

		public RpcClass()
		{
		}

		public RpcClass(T1 v1, T2 v2, T3 v3, T4 v4)
		{
			Value1 = v1;
			Value2 = v2;
			Value3 = v3;
			Value4 = v4;
		}
	}

	[ProtoContract]
	public class RpcList<T>
	{
		[ProtoMember(1)]
		public List<T> Value;

		public RpcList()
		{
			Value = new List<T>();
		}

		public RpcList(List<T> val)
		{
			Value = val;
		}
	}

	[ProtoContract]
	public class RpcDictionary<TKey, TValue>
	{
		[ProtoMember(1)]
		public Dictionary<TKey, TValue> Value;

		public RpcDictionary()
		{
		}

		public RpcDictionary(Dictionary<TKey, TValue> value)
		{
			Value = value;
		}
	}
}
