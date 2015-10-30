using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Text;


namespace Imps.Services.CommonV4
{
	public static class BinarySerializer
	{
		public static long Serialize<T>(Stream stream, T obj)
		{
			BinaryFormatter fmt = new BinaryFormatter();
			long start = stream.Position;
			fmt.Serialize(stream, obj);
			return stream.Position - start;
		}

		public static Stream Serialize<T>(T obj)
		{
			Stream stream = new MemoryStream();
			Serialize<T>(stream, obj);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

		public static byte[] ToByteArray<T>(T obj)
		{
			Stream stream = new MemoryStream();
			Serialize<T>(stream, obj);
			stream.Seek(0, SeekOrigin.Begin);
			int length = (int)stream.Length;

			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, length);
			return buffer;
		}

        public static T FromByteArray<T>(byte[] buffer)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter fmt = new BinaryFormatter();
            return (T)fmt.Deserialize(stream);
        }

		public static T Deserialize<T>(Stream stream)
		{
			BinaryFormatter fmt = new BinaryFormatter();
			return (T)fmt.Deserialize(stream);
		}

		public static T SerializeClone<T>(T obj)
		{
			BinaryFormatter fmt = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			fmt.Serialize(stream, obj);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)fmt.Deserialize(stream);
		}
	}
}
