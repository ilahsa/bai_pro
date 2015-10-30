//using System;
//using System.Threading;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;

//using Imps.Common.Sipc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Imps.Services.CommonV4;

//namespace UnitTest
//{
//    /// <summary>
//    /// Summary description for QualityMonitorTest
//    /// </summary>
//    [TestClass]
//    public class QualityMonitorTest
//    {
//        public QualityMonitorTest()
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
//            ServiceSettings.InitService("PRS");

//            SipcStack.Initialize();
//            RpcSipcClientChannel channel = new RpcSipcClientChannel();
//            RpcProxyFactory.RegisterClientChannel(channel);
			
//            QualityMonitor.Initialize();
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
//        public void QualityMonitorTest_TestPRS()
//        {
//            //
//            // TODO: Add test logic	here
//            //
//            while (true) {
//                for (int i = 0; i < 20000; i++) {
//                    QualityMonitor.Increment(QMAction_PRS.OnlineUser, i % 5, 0, "CN." + (i % 30), IICClientType.PC, "1.1", 0);
//                }
//                Thread.Sleep(5000);
//                for (int i = 0; i < 10000; i++) {
//                    QualityMonitor.Decrement(QMAction_PRS.OnlineUser, i % 5, 0, "CN." + (i % 30), IICClientType.PC, "1.1", 0);
//                }
//                Thread.Sleep(5000);
//            }
//        }

//        [TestMethod]
//        public void QualitMonitorTest_Size()
//        {
//            QMData data = new QMData();
//            data.ServiceName = "PRS";
//            data.ActionId = 1;
//            data.ComputerName = "P01-LCPROXY-01";
//            data.Keys = new List<QMKey>();
			
//            for (int i = 0; i < 1000; i++) {
//                data.Keys.Add(new QMKey() {
//                    PhysicalPoolId = 10,
//                    CarrierId = 1,
//                    ClientType = IICClientType.PC,
//                    ClientVersion = "4.0.1.1134",
//                    Region = "CN.bj.10",
//                    StatusCode = 300
//                });
//            }

//            Console.WriteLine(ProtoBufSerializer.ToByteArray<QMData>(data).Length);
//        }
//    }
//}
