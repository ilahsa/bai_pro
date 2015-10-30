using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTest.Rpc;
using Imps.Services.CommonV4;

namespace IICCommonlibrary_NUnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServiceSettings.InitService("IICCommonlibraryTestr");
            //Rpc_TestAll rpc = new Rpc_TestAll();
            //rpc.MethodTestBatch();
            HttpListenerServer httpServer = new HttpListenerServer("http://127.0.0.1:9097/");
            httpServer.Start();
            Console.ReadLine();
            //
        }
    }
}
