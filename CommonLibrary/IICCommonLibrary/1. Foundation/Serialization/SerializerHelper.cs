using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class SerializerHelper
	{
		public static byte[] ToBuffer<T>(T obj)
		{
			throw new NotImplementedException();
		}

		public static byte[] ToBuffer(Exception ex)
		{
			throw new NotImplementedException();
		}

		public static byte[] ToBuffer(ISerializableObject ob)
		{
			throw new NotImplementedException();
		}

		public static T FromBuffer<T>(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public static void WriteStream<T>(Stream stream, T obj)
		{
			throw new NotImplementedException();
		}

		public static T ReadStream<T>(Stream stream, T obj)
		{
			throw new NotImplementedException();
		}
	}
}
