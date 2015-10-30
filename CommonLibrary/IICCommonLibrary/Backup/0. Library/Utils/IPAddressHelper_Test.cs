using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Observation;

namespace UnitTest.Utils
{
	/// <summary>
	/// Summary description for IPAddressHelper_Test
	/// </summary>
	[TestClass]
	public class IPAddressHelper_Test
	{
		public IPAddressHelper_Test()
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
		public void IsLocalNetwork_Test()
		{
			//
			// TODO: Add test logic	here
			Assert.IsTrue(IPAddressHelper.IsLocalNetwork("192.168.1.100"));
			Assert.IsTrue(IPAddressHelper.IsLocalNetwork("192.168.1.102"));
			Assert.IsTrue(IPAddressHelper.IsLocalNetwork("127.0.0.1"));
			Assert.IsTrue(IPAddressHelper.IsLocalNetwork("10.10.1.10"));

			Assert.IsFalse(IPAddressHelper.IsLocalNetwork("202.112.69.20"));
			Assert.IsFalse(IPAddressHelper.IsLocalNetwork("64.112.69.20"));
		}

		[TestMethod]
		public void Stopwatch_TestPerformance()
		{
			PerformanceObserverItem item = new PerformanceObserverItem();
			Stopwatch s = new Stopwatch();
			s.Start();
			Thread.Sleep(1000);
			for (int i = 0; i < 1000000; i++) {
				item.Track(null, s.ElapsedTicks);
			}
			Console.WriteLine(ObjectHelper.DumpObject(item));
		}
	}
}
