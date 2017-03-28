using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Imps.Services.HA
{
    public class MasterConsole : ConsoleBase
    {
        private string _magicCookie;
        public string MagicCookie
        {
            get { return _magicCookie; }
        }

        private Process _worker;
        public Process Worker
        {
            get { return _worker; }
            set 
            {
                Process worker = _worker;
                if (object.ReferenceEquals(worker, value))
                    return;

                if (value != null)
                {
                    _startTime = DateTime.Now;
                    _lastHeartbeat = _startTime.AddSeconds(60.0);
                    value.Exited += OnWorkerExited;
                }

                _worker = value;

                if (worker != null)
                    worker.Dispose();
            }
        }

        private DateTime _lastHeartbeat;
        public DateTime LastHeartbeat
        {
            get { return _lastHeartbeat; }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
        }

        private DateTime _stopTime;
        public DateTime StopTime
        {
            get { return _stopTime; }
            set { _stopTime = value; }
        }

        private WorkerStatus _status;
        public WorkerStatus Status
        {
            get { return _status; }
        }

        private ConsoleCommandEvent[] _startResponse;
        private ConsoleCommandEvent _workerInitialize;

        public MasterConsole(string magicCookie)
            : base(magicCookie, true, 
                new ConsoleCommand[] {
                    ConsoleCommand.StartCommand, 
                    ConsoleCommand.PauseCommand, 
                    ConsoleCommand.ResumeCommand, 
                    ConsoleCommand.StopCommand, 
                    ConsoleCommand.PingCommand
                }, new ConsoleCommand[] {
                    ConsoleCommand.PingResponse,
                }
            )
        {
            _magicCookie = magicCookie;

            _startTime = DateTime.Now;
            _lastHeartbeat = _startTime.AddSeconds(60.0);

            _startResponse = new ConsoleCommandEvent[2] {
                new ConsoleCommandEvent(magicCookie, ConsoleCommand.StartSuccess, true), 
                new ConsoleCommandEvent(magicCookie, ConsoleCommand.StartFailure, true)
            };
            _workerInitialize = new ConsoleCommandEvent(magicCookie, ConsoleCommand.Initialized, true);
        }

        public bool Ping()
        {
            return WriteCommand(ConsoleCommand.PingCommand);
        }

        public bool Start(int timeout)
        {
            if (_worker == null)
                return false;

            if (!_workerInitialize.WaitOne(timeout))
                return false;

            _status = WorkerStatus.Ready;
            if (!WriteCommand(ConsoleCommand.StartCommand))
                return false;

            if (ConsoleCommandEvent.WaitAny(_startResponse, timeout) != ConsoleCommand.StartSuccess)
                return false;

            _status = WorkerStatus.Running;
            return true;
        }

        public bool Pause()
        {
            return WriteCommand(ConsoleCommand.PauseCommand);
        }

        public bool Resume()
        {
            return WriteCommand(ConsoleCommand.ResumeCommand);
        }

        public bool Stop(int timeout)
        {
            Process worker = _worker;
            if (worker == null)
                return true;

            _worker = null;
            _status = WorkerStatus.Stopped;
            WriteCommand(ConsoleCommand.StopCommand);

            if (worker.Id == Process.GetCurrentProcess().Id)
                return true;

            try
            {
                if (!worker.WaitForExit(timeout))
                {
                    worker.Kill();
                    worker.WaitForExit();
                }
                worker.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return true;
        }

        public bool IsHeartBeatExpire(TimeSpan interval)
        {
            return DateTime.Now - _lastHeartbeat > interval;
        }

        public bool IsWorkerLifeExpire(TimeSpan lifetime)
        {
            DateTime now = DateTime.Now;

            if (now.Hour < 3 || now.Hour >= 7)
                return false;

            if (now - _startTime <= lifetime)
                return false;

            return now.Date.AddHours(3).AddMinutes((new Random()).Next(180)) <= now;
        }

        protected override void OnCommandReceived(ConsoleCommand cmd)
        {
            _lastHeartbeat = DateTime.Now;
        }

        private void OnWorkerExited(object sender, EventArgs e)
        {
            Process worker = _worker;
            Process p = sender as Process;
            if (worker == null || worker.Id == p.Id)
            {
                _worker = null;
                _status = WorkerStatus.Stopped;
            }
        }
    }
}