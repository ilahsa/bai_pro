using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Google.ProtoBuf;
using Imps.Services.CommonV4;

namespace Imps.Services.CommonV4.UnitTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ProtoBufSerializerText
    {
        [Flags]
        public enum TestEnum
        {
            A,
            B,
            C
        };

        public ProtoBufSerializerText()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SimpleTypeTest()
        {
            //
            // TODO: Add test logic	here
            //
			ProtoBufSerializer.ToByteArray(false);
			ProtoBufSerializer.ToByteArray((int)64);
			ProtoBufSerializer.ToByteArray((long)640000000000L);
			ProtoBufSerializer.ToByteArray((float)64.00);
			ProtoBufSerializer.ToByteArray((double)64.00);
			ProtoBufSerializer.ToByteArray((decimal)64.00);
			ProtoBufSerializer.ToByteArray((string)"GFW");
			ProtoBufSerializer.ToByteArray(TestEnum.A | TestEnum.B);
			ProtoBufSerializer.ToByteArray(DateTime.Parse("1989-06-04"));
			ProtoBufSerializer.ToByteArray(TimeSpan.Parse("6:0:0"));
			ProtoBufSerializer.ToByteArray(new Uri("emailto:hexie@gfw.com"));
			ProtoBufSerializer.ToByteArray(new int[] { 1, 2, 3, 4, 5});
			ProtoBufSerializer.ToByteArray(new byte[] { 1, 2, 3, 4, 5 });
			ProtoBufSerializer.ToByteArray(new byte[] { 1, 2, 3, 4, 5 });
			ProtoBufSerializer.ToByteArray(Guid.NewGuid());
			ProtoBufSerializer.ToByteArray(new List<string>());
			ProtoBufSerializer.ToByteArray(new Dictionary<int, string>());
        }

    }


    [ProtoContract]
    public class SampleItem
    {
        [ProtoMember(1)]
        public int Id;

        [ProtoMember(2)]
        public string Name;
    }

    [ProtoContract]
    [ProtoInclude(50, typeof(SampleClass))]
    public class SampleBaseClass
    {
        [ProtoMember(1)]
        public string Name;

        [ProtoMember(2)]
        public long MobileNo;

        [ProtoMember(3)]
		public List<SampleItem> Items;
    }

    [ProtoContract]
    public class SampleClass: SampleBaseClass
    {
        [ProtoMember(1)]
        public string NameEx;      
    }
}
