using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;


namespace Imps.Services.CommonV4.Net
{
    public class AsyncSocketListener
    {
        #region Private fields

        private object _syncRoot = new object();
        private Socket _socket;
        private IPEndPoint _localEP;

        private SocketConnectedEventArgs _evenArgs = new SocketConnectedEventArgs();
        private int _socketBufferSize = 8;

        #endregion


        #region Public properties

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion


        #region Events

        public event SocketConnectedEventHandler SocketConnected;

        #endregion


        #region Constructor

        public AsyncSocketListener(int socketBufferSize)
        {
            _socketBufferSize = socketBufferSize;
        }

        #endregion


        #region Public methods

        public void Listen(IPEndPoint localEP)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _localEP = localEP;
            try
            {
                s.Bind(localEP);
                s.Listen(10);
            }
            catch (Exception ex)
            {
                s.Close();
                Trace.WriteLine(string.Format("trying to listen on {0}\r\n{1}", localEP, ex));
                throw new ApplicationException(string.Format("trying to listen on {0}", localEP), ex);
            }

            _socket = s;
        }

        public void BeginAccept()
        {
            if (_socket == null)
                throw new ApplicationException(string.Format("AsyncSocketListener::BeginAccept: socket=null(LocalEP={0})", _localEP));

            try
            {
                _socket.BeginAccept(new AsyncCallback(BeginAcceptCallback), _socket);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("AsyncSocketListener::BeginAccept: socket=null(LocalEP={0})", _localEP), ex);
            }
        }

        public void Stop()
        {
            Socket s = _socket;
            if (s == null)
                return;

            lock (_syncRoot)
            {
                if (_socket == null)
                    return;

                _socket = null;
            }

            s.Close();
        }

        #endregion


        #region Callbacks

        public void BeginAcceptCallback(IAsyncResult result)
        {
            Socket s = result.AsyncState as Socket;
            Socket c = null;
            try
            {
                c = s.EndAccept(result);
                if (c == null)
                {
                    //Stop();
                    Trace.WriteLine("AsyncSocketListener::BeginAcceptCallback: c=null");
                    return;
                }

                c.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                c.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _socketBufferSize * 1024);
                c.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, _socketBufferSize * 1024);
                _evenArgs.Bind(c);
                if (SocketConnected != null)
                    SocketConnected(this, _evenArgs);
            }
            catch (Exception ex)
            {
                //Stop();
                Trace.WriteLine(ex);
            }

            try
            {
                lock (_syncRoot)
                {
                    if (_socket != null)
                        BeginAccept();
                }
            }
            catch (Exception ex)
            {
                Stop();
                Trace.WriteLine(ex);
            }
        }

        #endregion
    }
}
