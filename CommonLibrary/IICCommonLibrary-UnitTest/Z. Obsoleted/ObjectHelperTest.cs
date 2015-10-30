using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Imps.Services.CommonV4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
	/// <summary>
	/// Summary description for ObjectHelperTest
	/// </summary>
	[TestClass]
	public class ObjectHelperTest
	{
		public ObjectHelperTest()
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
		public void TestConvertEnum()
		{
			//
			// TODO: Add test logic	here
			//
			
			//[Flags]
			//    public enum IPListType
			//    {
			//        Unknown,
			//        WapWhiteList,
			//        SapBlockList,
			//        WapCookiesWhiteList,//新增新加坡手机号解密方式, by yueyuan
			//    }

			TestEnum e;
			e = ObjectHelper.ConvertTo<TestEnum>("A");
			Assert.AreEqual(TestEnum.A, e);

			e = ObjectHelper.ConvertTo<TestEnum>("1");
			Assert.AreEqual(TestEnum.B, e);

			e = ObjectHelper.ConvertTo<TestEnum>("0x2");
			Assert.AreEqual(TestEnum.C, e);

			e = ObjectHelper.ConvertTo<TestEnum>("3");
			Assert.AreEqual((TestEnum)3, e);

			e = ObjectHelper.ConvertTo<TestEnum>("E|A");
			Assert.AreEqual(TestEnum.A, e);

			TestFlags f;
			f = ObjectHelper.ConvertTo<TestFlags>("A,B,C");
			Assert.AreEqual(TestFlags.A | TestFlags.B | TestFlags.C, f);

			f = ObjectHelper.ConvertTo<TestFlags>("A,B,C,E");
			Assert.AreEqual(TestFlags.A | TestFlags.B | TestFlags.C, f);

			f = ObjectHelper.ConvertTo<TestFlags>("0x0f");
			Assert.AreEqual(TestFlags.A | TestFlags.B | TestFlags.C | TestFlags.D, f);

			f = ObjectHelper.ConvertTo<TestFlags>("A,B,C,D,E");
			Assert.AreEqual(TestFlags.A | TestFlags.B | TestFlags.C | TestFlags.D, f);
		}

		public enum TestEnum
		{
			A = 0,
			B = 1,
			C = 2,
		}

		[Flags]
		public enum TestFlags
		{
			None = 0,
			A = 1,
			B = 2,
			C = 4,
			D = 8
		}
	}
}
