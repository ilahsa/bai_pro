using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public abstract class IICCodeTableItemKey
	{
	}


	public class IICCodeTableItemKey<T1, T2>
	{
		public T1 V1;
		public T2 V2;

		public IICCodeTableItemKey()
		{
		}

		public IICCodeTableItemKey(T1 v1, T2 v2)
		{
			V1 = v1;
			V2 = v2;
		}

		public override bool Equals(object obj)
		{
			IICCodeTableItemKey<T1, T2> rval = obj as IICCodeTableItemKey<T1, T2>;
			if (obj == null) {
				return false;
			} else {
				return this.V1.Equals(rval.V1) && this.V2.Equals(rval.V2);
			}
		}

		public override int GetHashCode()
		{
			return V1.GetHashCode() ^ V2.GetHashCode();
		}
	}

	//public class IICCodeTableItemKey<T1, T2, T3>
	//{
	//    public T1 V1;
	//    public T2 V2;
	//    public T3 V3;

	//    public IICCodeTableItemKey()
	//    {
	//    }

	//    public IICCodeTableItemKey(T1 v1, T2 v2, T3 v3)
	//    {
	//        V1 = v1;
	//        V2 = v2;
	//        V3 = v3;
	//    }

	//    public override bool Equals(object obj)
	//    {
	//        IICCodeTableItemKey<T1, T2, T3> rval = obj as IICCodeTableItemKey<T1, T2, T3>;
	//        if (obj == null) {
	//            return false;
	//        } else {
	//            return this.V1.Equals(rval.V1) &&
	//                this.V2.Equals(rval.V2) &&
	//                this.V3.Equals(rval.V3);
	//        }
	//    }

	//    public override int GetHashCode()
	//    {
	//        return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode();
	//    }
	//}

	//public class IICCodeTableItemKey<T1, T2, T3, T4>
	//{
	//    public T1 V1;
	//    public T2 V2;
	//    public T3 V3;
	//    public T4 V4;

	//    public IICCodeTableItemKey()
	//    {
	//    }

	//    public IICCodeTableItemKey(T1 v1, T2 v2, T3 v3, T4 v4)
	//    {
	//        V1 = v1;
	//        V2 = v2;
	//        V3 = v3;
	//        V4 = v4;
	//    }

	//    public override bool Equals(object obj)
	//    {
	//        IICCodeTableItemKey<T1, T2, T3, T4> rval = obj as IICCodeTableItemKey<T1, T2, T3, T4>;
	//        if (obj == null) {
	//            return false;
	//        } else {
	//            return this.V1.Equals(rval.V1) &&
	//                this.V2.Equals(rval.V2) &&
	//                this.V3.Equals(rval.V3) &&
	//                this.V4.Equals(rval.V4);
	//        }
	//    }

	//    public override int GetHashCode()
	//    {
	//        return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode() ^ V4.GetHashCode();
	//    }
	//}
}
