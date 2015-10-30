/*
 * 包装一个Key值对象，不改变GetHashCode和Equal的表现，
 * 允许在Key上附加一个Token对象，附加Token对象的时候请自己管理锁
 * 
 * 2010-06-11
 * 和ParallQueue一起使用最佳
 */ 
using System;

namespace Imps.Services.CommonV4
{
	public class KeyWrapper<K, T>
	{
		private K _key;
		private T _token;

		public KeyWrapper(K key, T token)
		{
			_key = key;
			_token = token;
		}

		public K Key
		{
			get { return _key; }
		}

		public T Token
		{
			get { return _token; }
			set { _token = value; }
		}

		public override int GetHashCode()
		{
			return _key.GetHashCode();
		}

		public override bool Equals(object rval)
		{
			if (rval.GetType() != this.GetType()) {
				return false;
			} else {
				KeyWrapper<K, T> r = (KeyWrapper<K, T>)rval;
				return _key.Equals(r.Key);
			}
		}
	}
}
