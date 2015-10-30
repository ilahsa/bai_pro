using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Rpc
{
    /// <summary>
    /// Summary description for RpcChannels_Test
    /// </summary>
    [TestClass]
    public class RpcChannels_Test
    {
        public RpcChannels_Test()
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
        }
    
        #endregion

        [TestMethod]
        [Owner("高磊")]
        public void ChannelTest_Http()
        {
            //
            // TODO: Add test logic	here
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcHttpUrl;
            test.MethodTestAll();
        }

        [TestMethod]
        [Owner("高磊")]
        public void ChannelTest_Inproc()
        {
            //
            // TODO: Add test logic	here
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcInprocUrl;
            test.MethodTestAll();
        }

        [TestMethod]
        [Owner("高磊")]
        public void ChannelTest_Pipe()
        {
            //
            // TODO: Add test logic	here
            //
            //
            // TODO: Add test logic	here
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcPipeUrl;
            test.MethodTestAll();
        }

        [TestMethod]
        [Owner("高磊")]
        public void ChannelTest_Tcp()
        {
            //
            // TODO: Add test logic	here
            //
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcTcpUrl;
            test.MethodTestAll();
        }
    }
}
