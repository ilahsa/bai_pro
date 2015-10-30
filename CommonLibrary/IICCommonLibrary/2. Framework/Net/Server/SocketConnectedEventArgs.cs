using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Imps.Services.CommonV4.Net
{
    [Serializable]
    public class SocketConnectedEventArgs : EventArgs
    {
        private Socket _socket;

        public Socket Socket
        {
            get { return _socket; }
        }

        public SocketConnectedEventArgs()
        {
        }

        internal void Bind(Socket socket)
        {
            _socket = socket;
        }
    }

    public delegate void SocketConnectedEventHandler(object sender, SocketConnectedEventArgs e);
}
