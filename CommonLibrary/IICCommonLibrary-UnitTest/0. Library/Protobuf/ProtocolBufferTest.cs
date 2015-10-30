using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Imps.Services.CommonV4;

namespace UnitTest
{
	/// <summary>
	/// Summary description for ProtocolBufferTest
	/// </summary>
	[TestClass]
	public class ProtocolBufferTest
	{
		public ProtocolBufferTest()
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
		public void ProtoBuffer_TestSimple()
		{
			byte[] buffer1 = ProtoBufSerializer.ToByteArray<int>(123);

			byte[] buffer2 = ProtoBufSerializer.ToByteArray<HybridDictionary<int, int>>(new HybridDictionary<int, int>());

			HybridDictionary<int, string> s = new HybridDictionary<int, string>();
			s.Add(1, "!23123");

			byte[] buffer3 = ProtoBufSerializer.ToByteArray<HybridDictionary<int, string>>(s);

			var ss = ProtoBufSerializer.FromByteArray<HybridDictionary<int, string>>(buffer3);

			Assert.AreEqual(ss.Count, 1);
			Assert.AreEqual(ss.Keys.Count, 1);
			Assert.AreEqual(ss.Values.Count, 1);
		}

		[TestMethod]
		public void ProtoBuffer_TestSimple2()
		{
			byte[] buffer1 = ProtoBufSerializer.ToByteArray<int>(123);

			byte[] buffer2 = ProtoBufSerializer.ToByteArray<HybridDictionary<int, int>>(new HybridDictionary<int, int>());

			HybridDictionary<int, string> s = new HybridDictionary<int, string>();
			s.Add(1, "!23123");

			byte[] buffer3 = ProtoBufSerializer.ToByteArray<HybridDictionary<int, string>>(s);

			var ss = ProtoBufSerializer.FromByteArray<HybridDictionary<int, string>>(buffer3);

			Assert.AreEqual(ss.Count, 1);
			Assert.AreEqual(ss.Keys.Count, 1);
			Assert.AreEqual(ss.Values.Count, 1);
		}
	}
}
