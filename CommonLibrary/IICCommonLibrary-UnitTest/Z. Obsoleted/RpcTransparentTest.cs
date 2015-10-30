//using System;
//using System.Net;
//using System.Threading;
//using Imps.Common.Sipc;
//using Imps.Services.CommonV4;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using UnitTest;

//namespace ConsoleTest {
//    [RpcService("IMathRpc", IsTransparent = true)]
//    public interface IMathRpc {
//        void Sum(RpcClass<int, int> args, RpcResult2<int> result);
//        void Sum2(int a, int b, RpcResult2<int> result);
//    }

//    public class MathImpServer : IMathRpc {
//        public void Sum(RpcClass<int, int> args, RpcResult2<int> result) {
//            result.Context.ReturnError(RpcErrorCode.SendFailed, new Exception("xxxxx"));
//        }

//        public void Sum2(int a, int b, RpcResult2<int> result) {
//            result.Callback(a + b);
//        }
//    }
//    [TestClass]
//    public class RpcTransparentTest {
//        private static TestContext _context;

//        [TestMethod]
//        public void TestCatchException() {
//            var proxy = RpcProxyFactory
//                .GetTransparentProxy<IMathRpc>("sipc://127.0.0.1:8000");

//            Exception exception = null;
//            AutoResetEvent wait = new AutoResetEvent(false);

//            RpcResult2<int> result2
//                = (Action<int>)(ret => _context.WriteLine("invoke sum return:{0}", ret));
//            result2.ExceptionHandler = (ex => { exception = ex; wait.Set(); });
//            proxy.Sum(new RpcClass<int, int>(2, 3), result2);
//            wait.WaitOne();
//            Assert.IsNotNull(exception, "ex is null");
//            _context.WriteLine("invoke sum return err:{0}", exception);

//        }

//        [TestMethod]
//        public void TestTypedRpcInvoke() {
//            var proxy = RpcProxyFactory
//                .GetTransparentProxy<IMathRpc>("sipc://127.0.0.1:8000");
//            int ret = -1;
//            AutoResetEvent wait = new AutoResetEvent(false);
//            proxy.Sum2(7, 8, (Action<int>)(i => { ret = i; wait.Set(); }));
//            wait.WaitOne();
//            Assert.AreEqual(ret, 15, "invoke sum2 return not 15");
//            _context.WriteLine("invoke sum2 7+8={0}", ret);

//        }

//        [ClassInitialize()]
//        public static void MyClassInitialize(TestContext testContext) {
//            ServiceSettings.InitService("UnitTest-Common");
//            StartServer();
//            StartClient();
//            _context = testContext;
//        }


//        static void StartServer() {
//            var channel = new RpcSipcServerChannel(IPAddress.Parse("0.0.0.0"), 8000);
//            RpcServiceManager.RegisterServerChannel(channel);
//            RpcServiceManager.RegisterTransparentService<IMathRpc>(new MathImpServer());
//            RpcServiceManager.Start();
//        }
//        static void StartClient() {
//            var sipcProvider = new SipcConnectionProvider("127.0.0.1:8001",
//                ConnectionUsage.Multiplex);
//            var stack = new SipcStack(sipcProvider, "epid");
//            var clientChannel = new RpcSipcClientChannel(stack);
//            RpcProxyFactory.RegisterClientChannel(clientChannel);
//        }
//    }
//}
