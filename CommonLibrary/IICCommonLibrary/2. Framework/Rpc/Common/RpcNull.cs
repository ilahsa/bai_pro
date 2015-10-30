/*
 * 标记空类型
 * 
 * 
 */ 
using System;
using System.IO;

namespace Imps.Services.CommonV4
{
	public class RpcNull
	{
		private RpcNull()
		{
			throw new NotSupportedException("This class should not instance");
		}
		public static readonly MemoryStream EmptyStream = new MemoryStream();
	}
}
