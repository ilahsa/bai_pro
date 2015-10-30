using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Rpc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Rpc
{
    /// <summary>
    /// Summary description for RpcTest
    /// </summary>
    [TestClass]
    public class Rpc_TestAll
    {
        public const string RpcHttpUrl = "http://127.0.0.1:8899/";
        public const string RpcPipeUrl = "pipe://127.0.0.1:UnitTest/";
        public const string RpcInprocUrl = "inproc://";
        public const string RpcTcpUrl = "tcp://127.0.0.1:3900/";

        public static bool _inited;
        public string CurrentTestUrl;
        public string CurrentFailedUrl;

        public Rpc_TestAll()
        {
            //
            // TODO: Add constructor logic here
            //
            if (!_inited)
            {

                ServiceSettings.InitService("UnitTest-Common");


                RpcProxyFactory.RegisterClientChannel(new RpcHttpClientChannel());
                RpcProxyFactory.RegisterClientChannel(new RpcPipeClientChannel());
                RpcProxyFactory.RegisterClientChannel(RpcInprocClientChannel.Instance);
                RpcProxyFactory.RegisterClientChannel(new RpcTcpClientChannel());


                RpcServiceManager.RegisterServerChannel(new RpcHttpServerChannel(8899));
                RpcServiceManager.RegisterServerChannel(new RpcPipeServerChannel("UnitTest", 10));
                RpcServiceManager.RegisterServerChannel(RpcInprocServerChannel.Instance);
                RpcServiceManager.RegisterServerChannel(new RpcTcpServerChannel(3900));

                RpcServiceManager.Start();
                RpcServiceManager.RegisterService<IRpcTestService>(new RpcTestService());

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                _inited = true;
            }

            CurrentTestUrl = RpcTcpUrl;
            CurrentFailedUrl = "tcp://127.0.0.1:49494";
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled {0}", e.ExceptionObject);
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
        }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TracingManager.FlushCache();
        }
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
        [Owner("高磊")]
        public void MethodTestAdd()
        {
            TestMethod(CurrentTestUrl, "Add", new RpcClass<int, int>(100, 100), 200);
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestAddBulk()
        {
            TracingManager.Level = TracingLevel.Warn;
            int failed = 0;
            int succ = 0;
            Exception lastEx;
            int total = 1500000;
            for (int i = 0; i < total; i++)
            {
                if (i % 1000 == 0)
                    Thread.Sleep(10);

                RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(CurrentTestUrl);
                proxy.BeginInvoke(
                    "Add",
                    new RpcClass<int, int>(120, 120),
                    delegate(RpcClientContext ctx)
                    {
                        try
                        {
                            var s = ctx.EndInvoke<RpcClass<int>>();
                            if (s.Value != 240)
                                throw new Exception("Response Failed:" + s);

                            Interlocked.Increment(ref succ);
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref failed);
                            lastEx = ex;
                        }
                    }
                );
            }
            Thread.Sleep(5000);
            Assert.AreEqual(0, failed);
            Assert.AreEqual(total, succ);
            TracingManager.Level = TracingLevel.Info;
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestMirror()
        {
            TestMethod(CurrentTestUrl, "Mirror", new RpcClass<int>(100), new RpcClass<int>(100));
            TestMethod(CurrentTestUrl, "Mirror", new RpcClass<int>(), new RpcClass<int>());
            //TestMethod<RpcClass<int>, RpcClass<int>>(CurrentTestUrl, "Mirror", null, null);
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestNull()
        {
            // TestMethod<RpcClass<int>, RpcClass<int>>(CurrentTestUrl, "Null", new RpcClass<int>(0), null);
            TestMethod<RpcClass<int>, RpcClass<int>>(CurrentTestUrl, "Mirror", null, null);
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestArray()
        {
            byte[] buffer = new byte[4096];
            TestMethod2<byte[], byte[]>(CurrentTestUrl, "Array", buffer,
                delegate(byte[] r)
                {
                    Assert.AreEqual(r.Length, buffer.Length);
                }
            );
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestDefault()
        {
            var e = new RpcClass<int>();
            TestMethod<RpcClass<int>, RpcClass<int>>(CurrentTestUrl, "Default", e, e);
            TestMethod<RpcClass<int>, RpcClass<int>>(CurrentTestUrl, "Mirror", e, e);
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestException_ServerError()
        {
            TestException(CurrentTestUrl, "TestException", RpcErrorCode.ServerError, "TestException");
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestException_SendFailed()
        {
            TestException(CurrentFailedUrl, "Mirror", RpcErrorCode.SendFailed, null);
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestException_Timeout()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(CurrentTestUrl);
                proxy.Timeout = 3000;
                proxy.Invoke<string, string>("TestTimeout", null);

                Assert.Fail("Should Failed...");
            }
            catch (RpcException ex)
            {
                Assert.AreEqual(RpcErrorCode.TransactionTimeout, ex.RpcCode);
                Console.WriteLine(ex.ToString());

                int ms = (int)watch.ElapsedMilliseconds;
            }
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestBatch()
        {
            Exception _ex = null;

            for (int i = 0; i < 128; i++)
            {
                RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(CurrentTestUrl);
                proxy.BeginInvoke(
                    "TestBatch",
                    "Hello",
                    delegate(RpcClientContext ctx)
                    {
                        try
                        {
                            var s = ctx.EndInvoke<string>();
                            if (s != "Hello:OK")
                                throw new Exception("Response Failed:" + s);
                        }
                        catch (Exception ex)
                        {
                            _ex = ex;
                        }
                    }
                );
            }

            Thread.Sleep(5000);
            if (_ex != null)
                throw _ex;
        }

        [TestMethod]
        [Owner("高磊")]
        public void MethodTestAll()
        {
            MethodTestAdd();
            MethodTestDefault();
            MethodTestNull();
            MethodTestArray();
            MethodTestMirror();
            MethodTestException_SendFailed();
            MethodTestException_ServerError();
            MethodTestException_Timeout();
        }

        public int TestMethod2<TArgs, TResults>(string serverUrl, string method, TArgs args, Action<TResults> assertCallback)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(serverUrl);
            TResults r = proxy.Invoke<TArgs, TResults>(method, args, 30000);

            assertCallback(r);
            return (int)watch.ElapsedMilliseconds;
        }

        public int TestMethod<TArgs, TResults>(string serverUrl, string method, TArgs args, TResults exceptedRet)
        {
            return TestMethod2<TArgs, TResults>(serverUrl, method, args,
                (r) => Assert.AreEqual(exceptedRet, r));
        }

        public int TestException(string serverUrl, string param, RpcErrorCode exceptedCode, string errMsg)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                RpcClientProxy proxy = RpcProxyFactory.GetProxy<IRpcTestService>(serverUrl);
                proxy.Invoke<string, string>("TestException", param);

                Assert.Fail("Should Failed...");
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.ToString());

                Assert.AreEqual(exceptedCode, ex.RpcCode);

                if (!string.IsNullOrEmpty(errMsg))
                    Assert.IsTrue(ex.ToString().Contains(errMsg));
            }
            int ms = (int)watch.ElapsedMilliseconds;
            Console.WriteLine("Cost Ms: {0}", ms);
            return ms;
        }
    }
}
