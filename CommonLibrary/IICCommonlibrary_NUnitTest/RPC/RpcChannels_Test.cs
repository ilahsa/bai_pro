using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace UnitTest.Rpc
{
    /// <summary>
    /// Summary description for RpcChannels_Test
    /// </summary>
    [TestFixture]
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



          [Test]
        public void ChannelTest_Http()
        {
            //
            // TODO: Add test logic	here
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcHttpUrl;
            test.MethodTestAll();
        }

          [Test]
        public void ChannelTest_Inproc()
        {
            //
            // TODO: Add test logic	here
            Rpc_TestAll test = new Rpc_TestAll();
            test.CurrentTestUrl = Rpc_TestAll.RpcInprocUrl;
            test.MethodTestAll();
        }

        [Test]
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

        [Test]
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
