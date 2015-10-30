//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;

//using Imps.Common.Sipc;
//using Imps.Services.CommonV4;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace UnitTest
//{
//    /// <summary>
//    /// Summary description for HashTest
//    /// </summary>
//    [TestClass]
//    public class HashTest
//    {
//        public HashTest()
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
//            ServiceSettings.InitService("IBS");

//            //HashManager.Initialize();
//            //HashManager.RegisterLocalHashService(HashPolicyName.PRSByUserIdRpc, "192.168.110.228");

//            //RpcSipcClientChannel channel = new RpcSipcClientChannel();
//            //RpcProxyFactory.RegisterClientChannel(channel);

//            //RpcRouteHelper.RegisterLocalPoolService<IRpcTestService>(new RpcTestService());

//            // RpcProxyFactory.RegisterInprocService<IRpcUnitTest>(new RpcServerTest());
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
//        public void HashInprocTest()
//        {
//            //
//            // TODO: Add test logic	here
//            //
//            HashDirector<int> director = HashManager.GetDirector<int>(HashPolicyName.PRSByUserIdRpc);
//            Console.WriteLine(director.GetValue(101));
//            Console.WriteLine(director.GetValue(201));

//            string url = director.GetValue(201);

//            RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(url);

//            string url2 = RouteHelper.GetAddress(RouteType.RpcOverSipc, "IBS", 1);

//            proxy = RpcProxyFactory.GetProxy<IRpcTestService>(url2);
//        }
//    }
//}
