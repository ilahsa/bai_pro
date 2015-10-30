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
	/// Summary description for CommonTest
	/// </summary>
	[TestClass]
	public class CommonTest
	{
		public CommonTest()
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
		public void ReadWriterLock_Test()
		{
			IICReaderWriterLock l = new IICReaderWriterLock();
			string s;
			using (IICLockRegion r = l.LockForRead()) {
				Console.WriteLine("ReaderLock: 1");
				using (IICLockRegion r2 = l.LockForRead()) {
					Console.WriteLine("ReaderLock: 1 - 2");
					using (IICLockRegion r3 = l.LockForRead()) {
						Console.WriteLine("ReaderLock: 1 - 3");

					}
				}
			}
			using (IICLockRegion r = l.LockForWrite()) {
				Console.WriteLine("WriterLock: 2");
				using (IICLockRegion r3 = l.LockForRead()) {
					Console.WriteLine("ReaderLock: 2 - 1");
				}
				s = "3";
			}

			using (IICLockRegion r = l.LockForUpgradeableRead()) {
				using (IICLockRegion r4 = l.LockForUpgradeableRead(300)) {
					s = "2-4";
					using (IICLockRegion r3 = l.LockForRead()) {
						s = "2-3";

					}
					r4.Upgrade();
				}
				r.Upgrade();
				s = "4";
			}
		}


		[TestMethod]
		public void FlagUtils_TestAll()
		{
			Assert.IsTrue(FlagUtils.GetOrder(0x08) == 3);
			Assert.IsTrue(FlagUtils.GetOrder(0x01) == 0);
			Assert.IsTrue(FlagUtils.GetMask(0) == 1);
			Assert.IsTrue(FlagUtils.GetMask(2) == 4);

			int test1 = 0x1010;
			Assert.IsTrue(FlagUtils.GetBit(test1, 0x1000) == true);
			Assert.AreEqual(FlagUtils.GetBit(test1, 0x0100), false);
			Assert.AreEqual(FlagUtils.GetBit(test1, 0x0010), true);

			test1 = FlagUtils.SetBit(test1, 0x0100, true);
			Assert.AreEqual(test1, 0x1110);

			test1 = FlagUtils.SetBit(test1, 0x1000, false);
			Assert.AreEqual(test1, 0x0110);

			int test2 = 0x1055;
			Assert.AreEqual(FlagUtils.GetBits(test2, 0x00f0), 5);
			Assert.AreEqual(FlagUtils.GetBits(test2, 0x000f), 5);

			test2 = FlagUtils.SetBits(test2, 0x00f0, 6);
			Assert.AreEqual(test2, 0x1065);

			test2 = FlagUtils.SetBits(test2, 0x000f, 8);
			Assert.AreEqual(test2, 0x1068);
		}

		[TestMethod]
		public void ParralelQueue_Test()
		{
			long sum1 = 0;
			long sum1a = 0;

			long sum2 = 0;
			long sum2a = 0;
			object syncRoot = new object();
			ParallelQueue<int, int> queue = new ParallelQueue<int, int>("Test", 30, 30,
				delegate(int i, int[] q) {
					lock (syncRoot) {
						foreach (int a in q) {
							if (i == 2)
								sum1a += a;
							sum1 += a;
						}
					}
				}
			);

			for (int j = 0; j < 45; j++) {
				for (int i = 0; i < 100000; i++) {
					int n = i % 2;
					sum2 += n;
					if (i % 5 == 2)
						sum2a += n;
					queue.Enqueue(i % 5, n);
				}
				Thread.Sleep(1000);
			}

			TestContext.WriteLine("{0} == {1}", sum1, sum2);
			Assert.AreEqual(sum1, sum2);
			Assert.AreEqual(sum1a, sum2a);
		}

        [TestMethod]
        [Owner("gaolei")]
        public void TestQMSBUG_1()
        {
            DateTime time = DateTime.Parse("2009-12-24 23:59:59.600");
            int t = (int)time.TimeOfDay.TotalSeconds;
            Console.WriteLine(t);
        }
	}
}
