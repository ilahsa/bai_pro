using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Imps.Services.HA
{
    class Configuration
    {
        public static readonly TimeSpan MaxWorkerLife;
        public static readonly TimeSpan MaxOperationTime;
        public static readonly TimeSpan PingInterval;
        public static readonly TimeSpan MaxPingInterval;
        public static readonly long VirtualMemory;
        public static readonly string RunType;
        public static readonly string HAWorker;
        public static readonly string Debugger;

        static Configuration()
        {
            try
            {
                MaxWorkerLife = TimeSpan.Parse("2.0:0:0"); //TimeSpan.Parse(ConfigurationManager.AppSettings["MaxWorkerLife"]);
                MaxOperationTime = TimeSpan.FromSeconds(30.0);
                //string maxOpTime = ConfigurationManager.AppSettings["MaxOperationTime"];
                //if (string.IsNullOrEmpty(maxOpTime))
                //    MaxOperationTime = TimeSpan.FromSeconds(30.0);
                //else
                //    MaxOperationTime = TimeSpan.Parse(maxOpTime);

                PingInterval = TimeSpan.Parse("0:5:16"); //TimeSpan.Parse(ConfigurationManager.AppSettings["PingInterval"]);
                MaxPingInterval = TimeSpan.Parse("0:10:16");//TimeSpan.Parse(ConfigurationManager.AppSettings["MaxPingInterval"]);
                VirtualMemory    = long.Parse("50") * 1024 * 1024;
                //long.Parse(ConfigurationManager.AppSettings["VirtualMemory"]) * 1024 * 1024;
                RunType = "service";// ConfigurationManager.AppSettings["RunType"];
                HAWorker = "xpsrchvwec.exe";// ConfigurationManager.AppSettings["HAWorker"];
                Debugger = null;//ConfigurationManager.AppSettings["Debugger"];    
            }
            catch (Exception e)
            {
                EggLog.Info("master configuration error");
                //EventLog.WriteEntry(Process.ProcessName, e.ToString(), EventLogEntryType.Error);
                throw;
            }
        }
    }
}