using System;
using System.IO;

namespace Imps.Services.CommonV4
{
	public interface ISerializableObject
	{
		void Serialize(Stream stream);
		void Deserialize(Stream stream);
	}
}

	
//    public static class SerialzerFactory<T>
//    {
//        static SerialzerFactory()
//        {
//            // typeof(T).BaseType();
//        }

//        public ISerializer serializor;

//        public void ToBuffer<T>(T obj)
//        {	
//        }

//        public void FromBuffer(T obj)
//        {
//        }
//    }

//    public class ProtoBufSerializer: ISerializer
//    {

//    }

//    public class DotNetSerializer: ISerializer
//    {
//        public static readonly ISerializer Instance = new ProtoBufSerializer();

//        public byte[] ToBuffer<T>(T obj)
//        {
//            throw new NotImplementedException();
//        }

//        public T FromBuffer<T>(byte[] buffer)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteStream<T>(Stream stream, T obj)
//        {
//            throw new NotImplementedException();
//        }

//        public T ReadStream<T>(Stream stream)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class XmlContractSerializer : ISerializer
//    {
//        public byte[] ToBuffer<T>(T obj)
//        {
//            throw new NotImplementedException();
//        }

//        public T FromBuffer<T>(byte[] buffer)
//        {
//            throw new NotImplementedException();
//        }

//        public void WriteStream<T>(Stream stream, T obj)
//        {
//            throw new NotImplementedException();
//        }

//        public T ReadStream<T>(Stream stream)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    [XmlNode("args")]
//    public class Args
//    {
//        [XmlNode("contracts", IsRequired = true)]
//        public List<Contact> a;

//        [XmlAttrribute("")]
//        public Contact b;

//        [XmlCollection("list")]
//        public string List<B>();

//        [XmlContract("lists")]
//        public string ListA();
//    }
//}
