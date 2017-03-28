using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Imps.Services.HA
{
    class MasterController
    {
        private readonly bool _runAsConsole = false;
		private readonly MasterConsole _console = null;
        private bool _stopFlag = false;
        private Thread _monitor = null;

        public MasterController(bool runAsConsole)
        {
            _runAsConsole = runAsConsole;
            _console = new MasterConsole(Guid.NewGuid().ToString("n"));
        }

		public void Start()
		{
            _stopFlag = false;

            if (!CreateWorker())
            {
                EventLog.WriteEntry(Configuration.HAWorker, string.Format("worker {0} failed/timeout to start in {1}", Configuration.HAWorker, Configuration.MaxOperationTime), EventLogEntryType.Error, 1001);
                //throw new ApplicationException(string.Format("worker {0} failed/timeout to start in {1}", Configuration.HAWorker, Configuration.MaxOperationTime));

            }
               
            if (_monitor == null)
            {
                _monitor = new Thread(WorkerStatusChecker);
                _monitor.IsBackground = true;
                _monitor.Name = "skyupdateex" + "-WorkerStatusChecker";
                _monitor.Start();
            }
		}

        private bool CreateWorker()
        {
            _console.Worker = CreateProcess(Configuration.HAWorker);

            if (_console.Start((int)Configuration.MaxOperationTime.TotalMilliseconds))
                return true;

            _console.Stop((int)Configuration.MaxOperationTime.TotalMilliseconds);
            return false;
        }

        private Process CreateProcess(string path)
        {
            ProcessStartInfo psi = new ProcessStartInfo(path, _console.MagicCookie);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;

            try
            {
                Process p = Process.Start(psi);
                p.EnableRaisingEvents = true;
                return p;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("{0} failed to create worker {1}", "skyupdateex", Configuration.HAWorker), e);
            }
        }

        public void Stop()
		{
            _stopFlag = true;

            _console.Stop((int)Configuration.MaxOperationTime.TotalMilliseconds);
		}

        private void WorkerStatusChecker()
		{
            while(!_stopFlag)
			{
                Thread.Sleep(Configuration.PingInterval);

                try
				{
                    if (_console.Worker == null)
                    {
                        if (!_stopFlag)
                            CreateWorker();
                    }
                    else if (ShouldKillWorker())
                    {
                        RunDebuggerAgainst(_console.Worker);
                        _console.Stop((int)Configuration.MaxOperationTime.TotalMilliseconds);
                    }
                    else 
                    {
                        _console.Ping();
                    }
                }
				catch(Exception e)
				{
					EggLog.Info("checking worker status\r\n" + e.ToString());
				}
			}
		}

        private bool ShouldKillWorker()
        {
            _console.Worker.Refresh();
          
            bool pingInterval = DateTime.Now - _console.LastHeartbeat > Configuration.MaxPingInterval;
            if (pingInterval)
                EggLog.Info(string.Format("master pause service! \r\nreason = {0} over max ping interval", Configuration.HAWorker));

            bool workerLife = DateTime.Now - _console.StartTime > Configuration.MaxWorkerLife;
            if (DateTime.Now.Hour < 3 || DateTime.Now.Hour >= 7)
            {
                workerLife = false;
            }
            else if( workerLife )
            {
                DateTime baseTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0);
                Random r = new Random();
                if (baseTime.AddMinutes(r.Next(180)) > DateTime.Now) 
                    workerLife = false;
            }

            if (workerLife)
                EggLog.Info(string.Format("master pause service! \r\nreason = {0} over max worker life", Configuration.HAWorker));

            long privateMemory = _console.Worker.PrivateMemorySize64;
            bool maxMemory = privateMemory > Configuration.VirtualMemory;
            if (maxMemory)
                EggLog.Info(string.Format("master pause service! \r\nreason = {0} over max memory, current={1}", Configuration.HAWorker, privateMemory));

            return pingInterval || workerLife || maxMemory;
        }

        private void RunDebuggerAgainst(Process p)
        {
            if (p == null)
                return;

            if (string.IsNullOrEmpty(Configuration.Debugger))
                return;

            try
            {
                DateTime date = DateTime.UtcNow;
                ProcessStartInfo start = new ProcessStartInfo("cmd.exe", "/C " + Configuration.Debugger.Replace("%ld", p.Id.ToString()).Replace("%ln", p.ProcessName).Replace("%dd", date.ToString("yyyyMMdd")).Replace("%dt", date.ToString("HHmmss")));
                start.CreateNoWindow = true;
                start.UseShellExecute = false;
                start.ErrorDialog = false;
                start.RedirectStandardError = true;
                start.RedirectStandardOutput = true;

                string log = Configuration.HAWorker + ".log";

                ExecutionContext ctxOutput = new ExecutionContext();
                ExecutionContext ctxError = new ExecutionContext();
                try
                {
                    ctxOutput.File = ctxError.File = new FileStream(log, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    ctxOutput.Buffer = new byte[1024];
                    ctxError.Buffer = new byte[1024];
                }
                catch (Exception ex)
                {
                    EggLog.Info(string.Format("master failed to open log file! \r\npath={0}\r\nreason={1}", log, ex.Message));
                }

                using (Process debugger = Process.Start(start))
                {
                    if (ctxOutput.File != null)
                        try
                        {
                            ctxOutput.Source = debugger.StandardOutput.BaseStream;
                            ctxError.Source = debugger.StandardError.BaseStream;
                            ctxOutput.Source.BeginRead(ctxOutput.Buffer, 0, ctxOutput.Buffer.Length, OnDebuggerOutputDataReceived, ctxOutput);
                            ctxError.Source.BeginRead(ctxError.Buffer, 0, ctxError.Buffer.Length, OnDebuggerOutputDataReceived, ctxError);
                        }
                        catch (Exception ex)
                        {
                            EggLog.Info(string.Format("master failed to read from Debugger! \r\ndebugger={0}\r\nreason={1}", Configuration.Debugger, ex.Message));
                        }

                    debugger.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                EggLog.Info(string.Format("master failed to run Debugger! \r\ndebugger={0}\r\nreason={1}", Configuration.Debugger, ex.Message));
            }
        }

        private void OnDebuggerOutputDataReceived(IAsyncResult ar)
        {
            ExecutionContext ctx = ar.AsyncState as ExecutionContext;
            try
            {
                int c = ctx.Source.EndRead(ar);
                if (c > 0)
                    ctx.File.BeginWrite(ctx.Buffer, 0, c, OnDebuggerOutputDataWrited, ctx);
                else
                    ctx.File.Close();
            }
            catch 
            {
                // do nothing
            }
        }

        private void OnDebuggerOutputDataWrited(IAsyncResult ar)
        {
            ExecutionContext ctx = ar.AsyncState as ExecutionContext;
            try
            {
                ctx.File.EndWrite(ar);
                ctx.File.Flush();
                ctx.Source.BeginRead(ctx.Buffer, 0, ctx.Buffer.Length, OnDebuggerOutputDataReceived, ctx);
            }
            catch 
            {
                // do nothing
            }
        }

        //private void Log(string message, EventLogEntryType eventType, int eventId)
        //{
        //    if (_runAsConsole)
        //    {
        //        bool re = Console.CursorLeft == 2;
        //        if (Console.CursorLeft > 2)
        //            Console.CursorTop += 1;
        //        Console.CursorLeft = 0;
        //        switch (eventType)
        //        {
        //            case EventLogEntryType.Error:
        //            case EventLogEntryType.FailureAudit:
        //                Console.ForegroundColor = ConsoleColor.Red;
        //                break;
        //            case EventLogEntryType.Warning:
        //                Console.ForegroundColor = ConsoleColor.Yellow;
        //                break;
        //            case EventLogEntryType.Information:
        //            case EventLogEntryType.SuccessAudit:
        //                Console.ForegroundColor = ConsoleColor.White;
        //                break;
        //            default:
        //                Console.ResetColor();
        //                break;
        //        }
        //        Console.WriteLine(message);
        //        if (re)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Green;
        //            Console.Write("> ");
        //        }
        //        Console.ResetColor();
        //    }
        //    else
        //    {
                
        //        EventLog.WriteEntry(Configuration.Process.ProcessName, message, eventType, eventId);
        //    }
        //}

        private class ExecutionContext
        {
            public FileStream File;
            public byte[] Buffer;
            public Stream Source;
        }
    }
}