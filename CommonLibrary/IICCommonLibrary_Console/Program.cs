using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace IICCommonLibrary_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceSettings.InitService("IICCommon");
            ITracing tracing = TracingManager.GetTracing(typeof(Program));
            tracing.Info("test");
            LinkedNode<string> linkedNode =new LinkedNode<string>();
            linkedNode.Value = "1";
            linkedNode.AddNext("2");
            linkedNode.AddNext("3");
            foreach (string str in linkedNode.GetValues())
            {
                Console.WriteLine(str);
            }
            Console.ReadLine();
        }
    }
}
