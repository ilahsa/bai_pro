using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace Imps.Services.HA
{
    static class Program
    {
        static void Main()
        {
            EggLog.Info("onlytest");

            if (string.Compare(Configuration.RunType, "console", true) == 0)
                RunAsConsole();
            else
                RunAsService();
        }

        private static void RunAsConsole()
        {
            MasterController monitorService = new MasterController(true);

            Console.Write("running master as console app, available commands: \r\n  start,stop\r\n> ");
            try
            {
                bool running = false;
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("> ");
                    Console.ResetColor();
                    string cmd = Console.ReadLine();
                    if (cmd == null)
                        break;

                    if (cmd == "start")
                    {
                        if (running)
                        {
                            Console.WriteLine("worker already running");
                        }
                        else
                        {
                            monitorService.Start();
                            running = true;
                        }
                    }
                    else if (cmd == "stop")
                    {
                        if (running)
                        {
                            monitorService.Stop();
                            running = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("worker not running");
                        }
                    }
                    else 
                    {
                        Console.WriteLine("unkown command");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();
            }

            Console.WriteLine("press ENTER to exit...");
            Console.ReadLine();
        }

        private static void RunAsService()
        {
            ServiceBase.Run(new ServiceBase[] { new MonitorService() });
        }
    }
}