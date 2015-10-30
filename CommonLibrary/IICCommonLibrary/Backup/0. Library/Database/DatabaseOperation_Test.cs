//using System;
//using System.Diagnostics;
//using System.Text;
//using System.Threading;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Imps.Services.CommonV4.Tracing;
//using Imps.Services.CommonV4;
//using Imps.Services.CommonV4.DbAccess;

//namespace UnitTest
//{
//    /// <summary>
//    /// Summary description for DbAccess_Test
//    /// </summary>
//    [TestClass]
//    public class DbAccess_Test
//    {
//        public DbAccess_Test()
//        {
//            //
//            // TODO: Add constructor logic here
//            //
//        }

//        private TestContext testContextInstance;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        //
//        // You can use the following additional attributes as you write your tests:
//        //
//        // Use ClassInitialize to run code before running the first test in the class
//        [ClassInitialize()]
//        public static void MyClassInitialize(TestContext testContext)
//        {
//            ServiceSettings.InitService("UnitTest-Common");
//        }
//        //
//        // Use ClassCleanup to run code after all tests in a class have run
//        // [ClassCleanup()]
//        // public static void MyClassCleanup() { }
//        //
//        // Use TestInitialize to run code before running each test 
//        // [TestInitialize()]
//        // public void MyTestInitialize() { }
//        //
//        // Use TestCleanup to run code after each test has run
//        // [TestCleanup()]
//        // public void MyTestCleanup() { }
//        //
//        #endregion

//        [TestMethod]
//        public void Mysql_TestBulkBinaryInsert()
//        {
//            Database db = DatabaseManager.GetDatabase("TESTBatchInsert");


//            List<TestTable> val = new List<TestTable>();
//            for (int i = 0; i < 78; i++) {
//                TestTable t = new TestTable();
//                t.TestStr = string.Format("HelloWorld:{0}", i);
//                t.TestBinary = StrUtils.FromHexString("0019238418273409178234");
//                t.TestTime = DateTime.Now;
//                val.Add(t);
//            }

//            db.BulkInsert<TestTable>("TestTable", val);

//            //
//            // TODO: Add test logic	here
//            //
//        }


//        [TestMethod]
//        public void Mysql_TestBulkInsert()
//        {
//            // MysqlDatabase db = (MysqlDatabase)DatabaseManager.GetDatabase("TESTDB").Operation;

//            int n = 1500;
//            int batch = 1500;
//            int m = n / batch;

//            Stopwatch watch2 = new Stopwatch();
//            watch2.Start();
//            string str = BuildTestStr();
//            string str2 = BuildTestStr2();
			
//            for (int j = 0; j < m; j++) {

//                List<TracingEvent> val = new List<TracingEvent>();
//                for (int i = 0; i < batch; i++) {
//                    TracingEvent t = new TracingEvent();
//                    t.ComputerName = "Hello:" + i;
//                    t.ServiceName = "123";
//                    t.ThreadInfo = "11";
//                    int l = i % 65;
//                    if (l < 65)
//                        t.Message = str.Substring((i % 65) * 1000, 1000);
//                    else
//                        t.Message = str.Substring(l * 1000);

//                    t.ProcessInfo = "e30eo";
//                    t.Repeat = 0;
//                    t.Time = DateTime.Now;
//                    t.Level = TracingLevel.Error;
//                    t.ComputerName = "123";

//                    if (l < 65) {
//                        t.Error = str2.Substring((i % 65) * 2000, 2000);
//                    } else {
//                        t.Error = str2.Substring(l * 2000);
//                    }
//                    val.Add(t);
//                }

//                Stopwatch watch = new Stopwatch();
//                watch.Reset();
//                watch.Start();
//                db.BulkInsert<TracingEvent>("CMN_ServerTrace", val);
//                TestContext.WriteLine("Cost {0} with batch {1} avg={2}", 
//                    watch.ElapsedMilliseconds, batch, batch * 1000 / watch.ElapsedMilliseconds);
//            }
//            TestContext.WriteLine("Cost {0} with {1} avg={2} per second", 
//                watch2.ElapsedMilliseconds, n, n * 1000 / watch2.ElapsedMilliseconds);

//            //
//            // TODO: Add test logic	here
//            //
//        }

//        private string BuildTestStr()
//        {
//            StringBuilder str = new StringBuilder();
//            for (int i = 0; i < 65536; i++) {
//                char c = (char)i;
//                str.Append(c);
//            }
//            return str.ToString(); ;
//        }

//        private string BuildTestStr2()
//        {
//            StringBuilder str = new StringBuilder();
//            for (int i = 0; i < 65536; i++) {
//                char c = (char)i;
//                str.Append('\\');
//                str.Append(c);
//            }
//            return str.ToString(); ;
//        }

//        [TestMethod]
//        public void Mysql_ErrorTest()
//        {
//            Database db = DatabaseManager.GetDatabase("TESTDB");

//            for (int i = 0; i < 500; i++) {
//                try {
//                    using (DataReader reader = db.SpExecuteReader("USP_Test", null)) {
//                        reader.Read();
//                    }
//                } catch (Exception ex) {
//                    if (ex.Message.Contains("Timeout")) {
//                        TestContext.WriteLine("Run {0} Times", i);
//                        TestContext.WriteLine(ex.ToString());
//                        throw;
//                    }
//                }
//            }
//        }

//        [TestMethod]
//        public void Mysql_InsertTest()
//        {
//            Database db = DatabaseManager.GetDatabase("TESTDB");

//            string[] parms = { "@Param2" };

//            string v = "?";
//            using (DataReader reader = db.SpExecuteReader("USP_Test2", parms, v)) {
//                reader.Read();

//                string v2 = reader.GetString(0);
//                Assert.AreEqual(v2, v);
//            }
//        }
//    }

//    class TestTable
//    {
//        [TableField("TestStr")]
//        public string TestStr;

//        [TableField("TestBinary")]
//        public byte[] TestBinary;

//        [TableField("TestTime")]
//        public DateTime TestTime;
//    }
//}
