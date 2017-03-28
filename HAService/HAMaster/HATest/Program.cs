using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HAWorker.egg;
using System.Collections;

namespace HATest
{

    class Program
    {
        static void Main(string[] args)
        {
            //TestJson();
            /// TestHandshake();
            SpecialPath();
            Console.ReadLine();
        }

        static void SpecialPath() {
            string pathnames = "windir;temp;programdata;";
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("  {0} = {1}", de.Key, de.Value);
            }
        }
        static void TestHandshake() {
            Constv.Instance.Init();
            TcpSession session = null;
            string newKey = null;
            Process.HandShake(out session, out newKey);

            Console.WriteLine(newKey + "");
        }
        static void TestJson() {
            var respStr = @"{""rc"":0,""payroll"":{ ""key"":""OAMBYKWBVSZDWYLX""}}";
            Response<Payroll_HandkShake> resp = JsonSerializer.JString2Object<Response<Payroll_HandkShake>>(respStr);
            Console.WriteLine(resp.payroll.key);
        }
    }

   
}