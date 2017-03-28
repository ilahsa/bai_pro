using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Imps.Services.HA
{
    class Program
	{
        static void Main(string[] args)
		{
            bool runAsConsole = false;

            string magicCookie = string.Empty;
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.IsNullOrEmpty(args[i]))
                    continue;

                if (args[i][0] == '/' || args[i][0] == '-')
                {
                    if (args[i].Substring(1) == "console")
                        runAsConsole = true;
                }
                else
                {
                    magicCookie = args[i];
                }
            }

            if (string.IsNullOrEmpty(magicCookie))
            {
                runAsConsole = true;
                magicCookie = Guid.NewGuid().ToString("n");
            }

            try
            {
                (new WorkerController(runAsConsole)).Run(magicCookie);
            }
            catch (Exception ex)
            {
                if (runAsConsole)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
                else
                {
                    EggLog.Info("work run error "+ex.ToString());
                    //EventLog.WriteEntry(Configuration.Process.ProcessName, ex.ToString(), EventLogEntryType.Error);
                }
            }

            if (runAsConsole)
            {
                Console.WriteLine("press ENTER to exit...");
                Console.ReadLine();
            }
		}
	}
}