using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Imps.Services.CommonV4;

namespace UnitTest
{
	/// <summary>
	/// Summary description for LRUCacheManagerTest
	/// </summary>
	[TestClass]
	public class LRUCacheManagerTest
	{
		public LRUCacheManagerTest()
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
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext) 
		{
			ServiceSettings.InitService("UnitTest-Common");
		}
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
		public void LRUCacheManager_Test()
		{
			//
			// TODO: Add test logic	here
			//
			LRUCacheManager<int, int> cache = new LRUCacheManager<int, int>("UnitTest", 1000);

			Random r = new Random();

			while (true) {
				for (int i = 0; i < 100; i++) {
					int n = r.Next(5000);

					if (n < 4000) {
						n = n % 1000;
					}

					int v;
					if (!cache.TryGetValue(n, out v)) {
						cache.Put(n, n);
					} else {
						Assert.AreEqual(n, v);
					}
				}
				Thread.Sleep(10);
			}
		}
	}
}
