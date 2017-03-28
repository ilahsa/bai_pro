using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Imps.Services.HA
{
    public enum ConsoleCommand
    {
        Initialized,

        StartCommand,
        StartSuccess,
        StartFailure,

        PauseCommand,
        ResumeCommand,
        StopCommand,

        PingCommand,
        PingResponse,

        INVALID
    }

    public sealed class ConsoleCommandEvent
    {
        private EventWaitHandle _event;

        private ConsoleCommand _command;
        public ConsoleCommand Command
        {
            get { return _command; }
        }

        public ConsoleCommandEvent(string magicCookie, ConsoleCommand command, bool create)
        {
            if (command == ConsoleCommand.INVALID)
                throw new ArgumentException("cannot be INVALID command");

            _command = command;
            string name = magicCookie + command.ToString();
            if (create)
                _event = new EventWaitHandle(false, EventResetMode.AutoReset, name);
            else
                _event = EventWaitHandle.OpenExisting(name);
        }

        public bool Set()
        {
            return _event.Set();
        }

        public bool Reset()
        {
            return _event.Reset();
        }

        public bool WaitOne()
        {
            return _event.WaitOne();
        }

        public bool WaitOne(int timeout)
        {
            return _event.WaitOne(timeout, false);
        }

        public static ConsoleCommand WaitAny(ConsoleCommandEvent[] commands)
        {
            WaitHandle[] handles = GetWaitHandle(commands);
            int i = WaitHandle.WaitAny(handles);
            if (i < 0 || i >= handles.Length)
                return ConsoleCommand.INVALID;
            return commands[i].Command;
        }

        public static ConsoleCommand WaitAny(ConsoleCommandEvent[] commands, int timeout)
        {
            WaitHandle[] handles = GetWaitHandle(commands);
            int i = WaitHandle.WaitAny(handles, timeout, false);
            if (i < 0 || i >= handles.Length)
                return ConsoleCommand.INVALID;
            return commands[i].Command;
        }

        private static WaitHandle[] GetWaitHandle(ConsoleCommandEvent[] commands)
        {
            WaitHandle[] handles = new WaitHandle[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                handles[i] = commands[i]._event;
            }
            return handles;
        }
    }
}