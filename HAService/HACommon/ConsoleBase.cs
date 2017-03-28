using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Imps.Services.HA
{
    public abstract class ConsoleBase
    {
        private ConsoleCommandEvent[] _send;
        private ConsoleCommandEvent[] _recv;

        private Thread _thread;
        private bool _stopFlag;

        protected abstract void OnCommandReceived(ConsoleCommand cmd);

        protected ConsoleBase(string magicCookie, bool create, ConsoleCommand[] send, ConsoleCommand[] recv)
        {
            if ((send == null || send.Length < 1) && (recv == null || recv.Length < 1))
                throw new ArgumentNullException("send && recv");

            try
            {
                _send = new ConsoleCommandEvent[send == null ? 0 : send.Length];
                for (int i = 0; i < _send.Length; ++i)
                    _send[i] = new ConsoleCommandEvent(magicCookie, send[i], create);

                _recv = new ConsoleCommandEvent[recv == null ? 0 : recv.Length];
                for (int i = 0; i < _recv.Length; ++i)
                    _recv[i] = new ConsoleCommandEvent(magicCookie, recv[i], create);
            }
            catch (WaitHandleCannotBeOpenedException ex)
            {
                EventLog.WriteEntry(Process.GetCurrentProcess().ProcessName, ex.ToString(), EventLogEntryType.Error);
                throw;
            }

            _thread = new Thread(ThreadProc);
            _thread.Name = "HA-IPC-Proc";
            _thread.IsBackground = true;
            _thread.Start();
        }

        public bool WriteCommand(ConsoleCommand cmd)
        {
            try
            {
                for (int i = 0; i < _send.Length; ++i)
                {
                    if (_send[i].Command != cmd)
                        continue;

                    _send[i].Set();
                    return true;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            return false;
        }

        public void Close()
        {
            _stopFlag = true;

            if (_thread == null) return;

            if (_thread.IsAlive)
            {
                if (!_thread.Join(10000)) 
                    _thread.Abort();
            }
            _thread = null;
        }

        private void ThreadProc()
        {
            try
            {
                while (!_stopFlag)
                {
                    try
                    {
                        ConsoleCommand cmd = ConsoleCommandEvent.WaitAny(_recv);
                        if (cmd == ConsoleCommand.INVALID)
                        {
                            Trace.WriteLine("failed to receive command");
                        }
                        else
                        {
                            OnCommandReceived(cmd);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}