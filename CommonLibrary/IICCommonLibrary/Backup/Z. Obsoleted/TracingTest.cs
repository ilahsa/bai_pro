//using System;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Imps.Common.Sipc;
//using Imps.Services.CommonV4;
//using Imps.Services.CommonV4.Tracing;

//namespace UnitTest
//{
//    /// <summary>
//    /// Summary description for TracingTest
//    /// </summary>
//    [TestClass]
//    public class TracingTest
//    {
//        public TracingTest()
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
//            RpcSipcServerChannel channel = new RpcSipcServerChannel(IPAddress.Parse("0.0.0.0"), 9999);
//            RpcServiceManager.RegisterServerChannel(channel);
//            Sniffer = new TracingSnifferService();
//            RpcServiceManager.Start();

//            RpcSipcClientChannel c2 = new RpcSipcClientChannel();
//            RpcProxyFactory.RegisterClientChannel(c2);
//            RpcServiceManager.RegisterService<ITracingSniffer>(Sniffer);
//        }
//        //
//        // Use ClassCleanup to run code after all tests in a class have run
//        [ClassCleanup()]
//        public static void MyClassCleanup()
//        {
//        }
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
//        public void Test_TracingAppender()
//        {
//            tracing.Info("InfoTest");
//            tracing.Warn("InfoTest2");
//            tracing.WarnFmt("InfoTest3");
//            SystemLog.Error(LogEventID.TracingFailed, "Test");
//            TracingManager.FlushCache();
//        }

//        [TestMethod]
//        public void Test_DatabaseAppender()
//        {

//        }

//        [TestMethod]
//        public void TracingAntiRepeater_Test()
//        {
//            for (int i = 0; i < 200000; i++) {
//                SystemLog.Error(LogEventID.TracingFailed, "Est");
//            }
//            for (int i = 0; i < 200000; i++) {
//                SystemLog.Error(LogEventID.TracingFailed, "Est2");
//            }
//            TracingManager.FlushCache();
//        }

//        [TestMethod]
//        public void TracingAntiRepeater_Test2()
//        {
//            for (int i = 0; i < 200000; i++) {
//                tracing.Error("Es222t");
//            }
//            for (int i = 0; i < 200000; i++) {
//                tracing.Error("E222st2");
//            }
//            TracingManager.FlushCache();
//        }

//        [TestMethod]
//        public void TracingSniffer()
//        {
//            //TracingManager.AddSniffer("UnitTestTracing", "sipc://127.0.0.1:9999", TracingLevel.Info, string.Empty, string.Empty);

//            //tracing.Info("123123");
//            //tracing.Info("123232");

//            //Sniffer.WaitEvent.WaitOne();
//        }

//        static TracingSnifferService Sniffer;
//        ITracing tracing = TracingManager.GetTracing("UnitTestTracing");
//    }
//}
