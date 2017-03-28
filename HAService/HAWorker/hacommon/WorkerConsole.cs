using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Diagnostics;

namespace Imps.Services.HA
{
    public class WorkerConsole : ConsoleBase
    {
        private DateTime _lastPing;
        public DateTime LastPing
        {
            get { return _lastPing; }
        }

        public event EventHandler PingCommandReceived;
        public event EventHandler StartCommandReceived;
        public event EventHandler PauseCommandReceived;
        public event EventHandler ResumeCommandReceived;
        public event EventHandler StopCommandReceived;

        public WorkerConsole(string magicCookie)
            : base(magicCookie, false,
                new ConsoleCommand[] {
                    ConsoleCommand.Initialized,
                    ConsoleCommand.StartSuccess,
                    ConsoleCommand.StartFailure,
                    ConsoleCommand.PingResponse
                }, new ConsoleCommand[] {
                    ConsoleCommand.StartCommand,
                    ConsoleCommand.PauseCommand,
                    ConsoleCommand.ResumeCommand,
                    ConsoleCommand.StopCommand,
                    ConsoleCommand.PingCommand
                }
            )
        {
            _lastPing = DateTime.Now.AddSeconds(10.0);
        }

        public bool Initialized()
        {
            return WriteCommand(ConsoleCommand.Initialized);
        }

        public bool StartResponse(bool success)
        {
            return WriteCommand(success ? ConsoleCommand.StartSuccess : ConsoleCommand.StartFailure);
        }

        public bool PingResponse()
        {
            return WriteCommand(ConsoleCommand.PingResponse);
        }

        protected override void OnCommandReceived(ConsoleCommand cmd)
        {
            switch (cmd)
            {
                case ConsoleCommand.PingCommand:
                    _lastPing = DateTime.Now;
                    if (PingCommandReceived != null)
                        PingCommandReceived(this, EventArgs.Empty);
                    break;

                case ConsoleCommand.StartCommand:
                    if (StartCommandReceived != null)
                        StartCommandReceived(this, EventArgs.Empty);
                    break;

                case ConsoleCommand.PauseCommand:
                    if (PauseCommandReceived != null)
                        PauseCommandReceived(this, EventArgs.Empty);
                    break;

                case ConsoleCommand.ResumeCommand:
                    if (ResumeCommandReceived != null)
                        ResumeCommandReceived(this, EventArgs.Empty);
                    break;

                case ConsoleCommand.StopCommand:
                    if (StopCommandReceived != null)
                        StopCommandReceived(this, EventArgs.Empty);
                    break;

                default:

                    //Trace.WriteLine(string.Format("Unknown Command : {0}", cmd));
                    break;
            }
        }
    }
}