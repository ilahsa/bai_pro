using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4
{
	public class ProtoBufSerializer: ISerializer
	{
		public static readonly ISerializer Instance = new ProtoBufSerializer();

		#region ISerializer Members
		long ISerializer.Serialize<T>(Stream stream, T obj)
		{
			if (typeof(ISerializableObject).IsInstanceOfType(obj)) {
				var intf = (ISerializableObject)obj;
				long start = stream.Position;
				intf.Serialize(stream);
				return stream.Position - start;
			} else {
				long start = stream.Position;
				Serializer.Serialize<T>(stream, obj);
				return stream.Position - start;
			}
		}

		T ISerializer.Deserialize<T>(Stream stream)
		{
			if (typeof(T).GetInterface("ISerializableObject") != null) {
				T obj = Activator.CreateInstance<T>();
				((ISerializableObject)obj).Deserialize(stream);
				return obj;
			} else {
				return Serializer.Deserialize<T>(stream);
			}
		}
		#endregion

		#region Public Helper Method
		public static long Serialize<T>(Stream stream, T obj)
		{
			return ProtoBufSerializer.Instance.Serialize<T>(stream, obj);
		}

		public static long GetSize<T>(T obj)
		{
			DummyStream stream = new DummyStream();
			return Instance.Serialize(stream, obj);
		}

		public static MemoryStream Serialize<T>(T obj)
		{
			MemoryStream stream = new MemoryStream();
			Instance.Serialize<T>(stream, obj);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

		public static byte[] ToByteArray<T>(T obj)
		{
			MemoryStream stream = new MemoryStream();
			Instance.Serialize<T>(stream, obj);
			int length = (int)stream.Length;
			if (length == 0) {
				return _emptyBuffer;
			} else {
				byte[] buffer = new byte[length];
				stream.Seek(0, SeekOrigin.Begin);
				stream.Read(buffer, 0, length);
				return buffer;
			}
		}

		public static T Deserialize<T>(Stream stream)
		{
			return Instance.Deserialize<T>(stream);
		}

		public static T FromByteArray<T>(byte[] buffer)
		{
			return FromByteArray<T>(buffer, 0, buffer.Length);
		}

		public static T FromByteArray<T>(byte[] buffer, int offset, int length)
		{
			MemoryStream stream = new MemoryStream();
			stream.Write(buffer, offset, length);
			stream.Seek(0, SeekOrigin.Begin);
			return Deserialize<T>(stream);
		}
		#endregion

		private static byte[] _emptyBuffer = new byte[0];
	}
}
