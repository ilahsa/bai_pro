using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace enda_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread[] tds = new Thread[30];
            for (var i = 0; i < tds.Length; i++) {
                tds[i] = new Thread(new ThreadStart(TheadWork));
                tds[i].IsBackground = true;
                tds[i].Name = "td_" + i;
            }
            Console.WriteLine("start batch test ");
            foreach (var td in tds) {
                td.Start();
            }
            Console.ReadLine();
        }

        static void TheadWork() {
            while (true) {
                try
                {
                    var str = @"{""type"":""type1"",""str1"":10,""str2"":20,""str"":100}";
                    byte[] inBy = Encoding.UTF8.GetBytes(str);
                    byte[] outBy;
                    List<KeyValuePair<string, string>> respHeaders = null;
                    Console.WriteLine("send:" + str);
                    HttpUtils.HttpRequest("POST", "http://127.0.0.1:80/", "", null, inBy, out respHeaders, out outBy);
                    Console.WriteLine("rev:" + Encoding.UTF8.GetString(outBy));
                    System.Threading.Thread.Sleep(1000 * 10);
                }
                catch (Exception ex) {
                    Console.WriteLine("timeout?:" + ex.Message);
                }

            }
        }
    }
}
