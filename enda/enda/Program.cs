using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enda
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = System.Configuration.ConfigurationSettings.AppSettings["port"];
            Console.WriteLine("http server start on "+port);
            var listener = new NHttpListener(int.Parse(port));
            listener.Start();
            Console.ReadLine();
        }
    }
}
