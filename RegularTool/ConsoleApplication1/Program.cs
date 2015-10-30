using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var totalLine = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                var path = args[0];
                var regex = args[1];
                var path1 = args[2];
                var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (path.IndexOf("/")<0) {
                    path = Path.Combine(currentPath, path);
                }
                if (path1.IndexOf("/") < 0)
                {
                    path1 = Path.Combine(currentPath, path1);
                }

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine("file not found");
                    return;
                }

                Regex reg = new Regex(regex);
                var strs = File.ReadLines(path);
               
                using (StreamWriter writer = new StreamWriter(path1, true, Encoding.UTF8)) {
                    foreach (string str in strs)
                    {
                        if (reg.IsMatch(str))
                        {
                            totalLine++;
                            writer.WriteLine(str);
                        }
                    }
                }
               

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            sw.Stop();
            Console.WriteLine("millisecond " + sw.ElapsedMilliseconds);
            Console.WriteLine("total line " + totalLine);
            Console.WriteLine("finished");

            Console.ReadLine();
        }
    }
}
