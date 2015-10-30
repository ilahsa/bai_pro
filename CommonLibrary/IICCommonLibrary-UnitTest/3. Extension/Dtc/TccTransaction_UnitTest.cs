//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;


//using Imps.Services.CommonV4;
//using Imps.Services.CommonV4.Rpc;
//using Imps.Services.CommonV4.Dtc;
//using Imps.Services.CommonV4.Observation;

//namespace UnitTest.Dtc
//{
//    /// <summary>
//    /// Summary description for DtcTransaction_UnitTest
//    /// </summary>
//    [TestClass]
//    public class TccTransaction_UnitTest
//    {
//        static TccCoordinator<TccTestContext> _coordinator = new TccCoordinator<TccTestContext>("Test");
//        public TccTransaction_UnitTest()
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
//            ServiceSettings.InitService("UnitTest");
//            SipcStack.Initialize();

//            RpcServiceManager.RegisterServerChannel(RpcInprocServerChannel.Instance);
//            RpcProxyFactory.RegisterClientChannel(RpcInprocClientChannel.Instance);
//            TccRpcHostService.Initialize();
//            TccRpcHostService.RegisterWorkUnit(new TccTestRpcHost());
//            RpcServiceManager.Start();
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
//        [Owner("高磊")]
//        public void TccTransaction_Hello()
//        {
//            //
//            // TODO: Add test logic	here
//            TccTestTransaction trans = new TccTestTransaction(new TccTestContext() {
//                Sid = 100,
//                UserId = 0,
//            }, _coordinator);
//            try {
//                trans.Execute();
//            } catch (Exception ex) {
//                Console.WriteLine(ex.ToString());
//                throw;
//            }

//            foreach (var s in ObserverManager.EnumObserverName()) {
//                ObserverDataTable table = ObserverManager.Observe(s);
//                Console.WriteLine(s);
//                Console.WriteLine("".PadRight(72, '='));
//                if (table == null)
//                    continue;

//                foreach (var c in table.Columns) {
//                    Console.Write(c);
//                    Console.Write("\t");
//                }
//                Console.WriteLine("".PadRight(72, '-'));
//                foreach (var r in table.Rows) {
//                    foreach (var i in r.Value) {
//                        Console.Write(i);
//                        Console.Write("\t");
//                    }
//                    Console.WriteLine();
//                }
//            }
//        }

//        [TestMethod]
//        [Owner("高磊")]
//        public void TccTransaction_Exception()
//        {
//            //
//            // TODO: Add test logic	here
//            TccTestTransaction trans = new TccTestTransaction(new TccTestContext() {
//                Sid = 100,
//                UserId = 0,
//                FailedWork = "Work2",
//                FailedAction = TccAction.Try,
//            }, _coordinator);
//            try {
//                trans.Execute();
//            } catch (TccTransactionException<TccTestContext> ex) {
//                Console.WriteLine(ex.ToString());
//            }
//        }

//        [TestMethod]
//        [Owner("高磊")]
//        public void TccTransaction_RpcException()
//        {
//            //
//            // TODO: Add test logic	here
//            TccTestTransaction trans = new TccTestTransaction(new TccTestContext() {
//                Sid = 100,
//                UserId = 0,
//                FailedWork = "RpcWork1",
//                FailedAction = TccAction.Try,
//            }, _coordinator);
//            try {
//                trans.Execute();
//            } catch (TccTransactionException<TccTestContext> ex) {
//                Console.WriteLine(ex.ToString());
//            }
//        }


//        [TestMethod]
//        [Owner("李春雷")]
//        public void TccTransaction_TccPersister()
//        {
//            Database db = DatabaseManager.GetDatabase(IICDbType.SqlServer2005,"server=LICHL\\SQLEXPRESS;database=TCCDB;uid=sa;pwd=sa");
//            TccDatabasePersister persister = new TccDatabasePersister("PersisterTrans", db, TccPersisterMode.All);
//            _coordinator.Persister = persister;
//            //
//            // TODO: Add test logic	here
//            TccTestTransaction trans = new TccTestTransaction(new TccTestContext()
//            {
//                Sid = 100,
//                UserId = 0,
//            }, _coordinator);
//            try
//            {
//                trans.Execute();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.ToString());
//                throw;
//            }

//            foreach (var s in ObserverManager.EnumObserverName())
//            {
//                ObserverDataTable table = ObserverManager.Observe(s);
//                Console.WriteLine(s);
//                Console.WriteLine("".PadRight(72, '='));
//                if (table == null)
//                    continue;

//                foreach (var c in table.Columns)
//                {
//                    Console.Write(c);
//                    Console.Write("\t");
//                }
//                Console.WriteLine("".PadRight(72, '-'));
//                foreach (var r in table.Rows)
//                {
//                    foreach (var i in r.Value)
//                    {
//                        Console.Write(i);
//                        Console.Write("\t");
//                    }
//                    Console.WriteLine();
//                }
//            }
//        }

//        [TestMethod]
//        [Owner("李春雷")]
//        public void TccTransaction_TccPersisterException()
//        {
//            Database db = DatabaseManager.GetDatabase(IICDbType.SqlServer2005, "server=LICHL\\SQLEXPRESS;database=TCCDB;uid=sa;pwd=sa");
//            TccDatabasePersister persister = new TccDatabasePersister("PersisterTrans", db, TccPersisterMode.All);
//            _coordinator.Persister = persister;
//            //
//            // TODO: Add test logic	here
//            TccTestTransaction trans = new TccTestTransaction(new TccTestContext()
//            {
//                Sid = 100,
//                UserId = 0,
//                FailedWork = "Work2",
//                FailedAction = TccAction.Try,
//            }, _coordinator);
//            try
//            {
//                trans.Execute();
//            }
//            catch (TccTransactionException<TccTestContext> ex)
//            {
//                Console.WriteLine(ex.ToString());
//            }
//        }
//    }
//}
