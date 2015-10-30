using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Samples
{
    [ProtoContract]
    public class SampleClass
    {
        [ProtoMember(1)]
        public string Name;

        [ProtoMember(2)]
        public long MobileNo;

        [ProtoMember(3)]
        public List<SampleItem> Items;
    }

    [ProtoContract]
    public class SampleItem
    {
        [ProtoMember(1)]
        public int Id;

        [ProtoMember(2)]
        public string Value;
    }

    class ProtocolBufferTest
    {
        // 使用MemoryStream序列化
        static void Sample1(SampleClass c)
        {
            Stream stream = ProtoBufSerializer.Serialize<SampleClass>(c);
            SampleClass sampleclass = ProtoBufSerializer.Deserialize<SampleClass>(stream);
        }

        // 序列化到一个别的Stream中 
        static void Sample2(Stream stream, SampleClass c)
        {
            ProtoBufSerializer.Serialize(stream, c);
        }
    }
}
