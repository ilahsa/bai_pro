using System;
using System.Collections.Generic;
using System.Text;
//using System.Diagnostics;
using System.Threading;

namespace Imps.Services.HA
{
    class WorkerController
    {
        private readonly bool _runAsConsole;

        private IHAComponent _component;
        private WorkerConsole _console;
        private ManualResetEvent _waitForExit;
        private WorkerStatus _status;

        public WorkerController(bool runAsConsole)
        {
            _runAsConsole = runAsConsole;
        }

        internal bool Run(string magicCookie)
        {
            if (string.IsNullOrEmpty(magicCookie))
            {
                Log("ipc not available");// EventLogEntryType.Error, 1000);
                return false;
            }

            if (_component != null)
            {
                Log("worker already initialized");//, EventLogEntryType.Error, 1000);
                return false;
            }

            //object obj = null;
            //try
            //{
            //    obj = Activator.CreateInstance(Type.GetType(Configuration.Component, true));

            //}
            //catch (Exception ex)
            //{
            //    Log("failed to create " + Configuration.Component + ":\r\n" + ex, EventLogEntryType.Error, 1000);
            //    return false;
            //}

            //if (obj == null)
            //{
            //    Log("failed to create " + Configuration.Component, EventLogEntryType.Error, 1000);
            //    return false;
            //}

            _component = new EggComponent();  //obj as IHAComponent;

            _waitForExit = new ManualResetEvent(false);

            if (_runAsConsole)
            {
                MasterConsole master = new MasterConsole(magicCookie);
                master.Worker = System.Diagnostics.Process.GetCurrentProcess();

                Thread t = new Thread(RunAsConsoleApp);
                t.IsBackground = true;
                t.Name = "HA_Worker_Runas_Console_App";
                t.Start(master);
            }

            _console = new WorkerConsole(magicCookie);
            _console.PingCommandReceived   += OnPingCommandReceived;
            _console.StartCommandReceived  += OnStartCommandReceived;
            _console.PauseCommandReceived  += OnPauseCommandReceived;
            _console.ResumeCommandReceived += OnResumeCommandReceived;
            _console.StopCommandReceived   += OnStopCommandReceived;

            _status = WorkerStatus.Ready;
            _console.Initialized();

            _waitForExit.WaitOne();
            return true;
        }

        private void OnPingCommandReceived(object sender, EventArgs e)
        {
            if ((_status == WorkerStatus.Running) != _component.IsRunning)
            {
                _waitForExit.Set();
                return;
            }

            int tcWK;
            int tcIO;
            ThreadPool.GetAvailableThreads(out tcWK, out tcIO);

            if (tcWK == 0 || tcIO < 500)
            {
                Log(string.Format("thread count overflow! \r\nWorkerThread={0}, IOThread={1} : remain", tcWK, tcIO));//, EventLogEntryType.Warning, 1609);
            }

            ThreadPool.QueueUserWorkItem(delegate(object state) {
                _console.PingResponse();
            });
        }

        private void OnStartCommandReceived(object sender, EventArgs e)
        {
            try
            {
                _component.Start(sender, e);
                _status = WorkerStatus.Running;
                _console.StartResponse(true);
            }
            catch(Exception ex)
            {
                Log(string.Format("can't start component! \r\n{0}", ex));//, EventLogEntryType.Error, 1000);
                _console.StartResponse(false);
                _waitForExit.Set();
            }
        }

        private void OnPauseCommandReceived(object sender, EventArgs e)
        {
            if (_status != WorkerStatus.Running)
                return;

            try
            {
                _component.Pause(sender, e);
            }
            catch (Exception ex)
            {
                Log(string.Format("can't pause component! \r\n{0}", ex)); //, EventLogEntryType.Error, 1002);
            }

            _status = WorkerStatus.Paused;
        }

        private void OnResumeCommandReceived(object sender, EventArgs e)
        {
            if (_status != WorkerStatus.Paused)
                return;
            
            try
            {
                _component.Resume(sender, e);
                _status = WorkerStatus.Running;
            }
            catch (Exception ex)
            {
                Log(string.Format("can't resume component! \r\n{0}", ex));//, EventLogEntryType.Error, 1003);
                _waitForExit.Set();
            }
        }

        private void OnStopCommandReceived(object sender, EventArgs e)
        {
            try
            {
                _component.Stop(sender, e);
            }
            catch (Exception ex)
            {
                Log(string.Format("can't stop component! \r\n{0}", ex));//, EventLogEntryType.Error, 1001);
            }

            _status = WorkerStatus.Stopped;
            _waitForExit.Set();
        }

        private void RunAsConsoleApp(object state)
        {
            MasterConsole master = state as MasterConsole;
            if (master == null)
            {
                Console.Error.WriteLine("failed to start worker as console app");
                _waitForExit.Set();
                return;
            }

            Console.SetOut(new ConsoleWriter(Console.Out));

            Console.Error.WriteLine("running worker as console app, available commands:\r\n  start,stop,pause,resume");
            try
            {
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Error.Write("> ");
                    Console.ResetColor();
                    string cmd = Console.ReadLine();
                    if (cmd == null)
                        break;

                    switch (cmd)
                    {
                        case "start":
                            if (_status != WorkerStatus.Ready)
                            {
                                Console.Error.WriteLine("worker not ready yet");
                            }
                            else if (!master.Start(Timeout.Infinite))
                            {
                                goto quit;
                            }
                            break;
                        case "stop":
                            if (_status != WorkerStatus.Running && _status != WorkerStatus.Paused)
                            {
                                Console.Error.WriteLine("worker not started yet");
                                break;
                            }
                            master.Stop(Timeout.Infinite);
                            goto quit;
                        case "pause":
                            if (_status != WorkerStatus.Running)
                                Console.Error.WriteLine("worker not running");
                            else
                                master.Pause();
                            break;
                        case "resume":
                            if (_status != WorkerStatus.Paused)
                                Console.Error.WriteLine("worker not paused");
                            else
                                master.Resume();
                            break;
                        default:
                            Console.Error.WriteLine("unkown command");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex);
                Console.ResetColor();
            }

        quit: 
            if (_status == WorkerStatus.Running || _status == WorkerStatus.Paused)
                master.Stop(Timeout.Infinite);
            _waitForExit.Set();
        }


        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            //if (_runAsConsole)
            //{
            //    bool re = Console.CursorLeft == 2;
            //    if (Console.CursorLeft > 2)
            //        Console.CursorTop += 1;
            //    Console.CursorLeft = 0;

            //    switch (eventType)
            //    {
            //        case EventLogEntryType.Error:
            //        case EventLogEntryType.FailureAudit:
            //            Console.ForegroundColor = ConsoleColor.Red;
            //            break;
            //        case EventLogEntryType.Warning:
            //            Console.ForegroundColor = ConsoleColor.Yellow;
            //            break;
            //        case EventLogEntryType.Information:
            //        case EventLogEntryType.SuccessAudit:
            //            Console.ForegroundColor = ConsoleColor.White;
            //            break;
            //        default:
            //            Console.ResetColor();
            //            break;
            //    }
            //    Console.Error.WriteLine(message);
            //    Console.ResetColor();

            //    if (re)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.Error.Write("> ");
            //        Console.ResetColor();
            //    }
            //}
            //else
            //{
            //    EventLog.WriteEntry(Configuration.Process.ProcessName, message, eventType, eventId);
            //}
        }
    }
}