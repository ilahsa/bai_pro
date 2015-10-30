using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Generic;

namespace Imps.Services.CommonV4.Net
{
    public enum SocketConnectionDirection
    {
        Incoming = 0,
        Outgoing
    }

    public class AsyncSocketConnection
    {
        #region Const values

        //private const int RecvBufferSize = 1024 * 4;

        #endregion


        #region Private fields

        private object _syncRoot = new object();

        private Socket _socket;
        private IPEndPoint _remoteEP;
        private SocketConnectionDirection _direction;

        private byte[] _recvBuffer;

        private DataReceivedEventArgs _evenArgs;

        #endregion


        #region Public properties

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public Socket InnerSocket
        {
            get { return _socket; }
        }

        public SocketConnectionDirection Direction
        {
            get { return _direction; }
        }

        public bool IsConnected
        {
            get
            {
                lock (_syncRoot)
                {
                    return _socket != null && _socket.Connected;
                }
            }
        }

        #endregion


        #region Events

        public event DataReceivedEventHandler DataReceived;
        public event EventHandler Disconnected;

        #endregion


        #region Constructor

        public AsyncSocketConnection(int bufferSize)
        {
            _recvBuffer = new byte[bufferSize * 1024];
            _evenArgs = new DataReceivedEventArgs(_recvBuffer);
        }

        public AsyncSocketConnection(Socket socket, int bufferSize)
        {
            if (socket == null)
                throw new ApplicationException("socket is null");

            _recvBuffer = new byte[bufferSize * 1024];
            _evenArgs = new DataReceivedEventArgs(_recvBuffer);
            _direction = SocketConnectionDirection.Incoming;
            _socket = socket;
            _remoteEP = socket.RemoteEndPoint as IPEndPoint;
        }

        #endregion


        #region Public methods

        public void Connect(IPEndPoint remoteEP, int bufferSize)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                s.Connect(remoteEP);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, bufferSize * 1024);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, bufferSize * 1024);
            }
            catch (Exception ex)
            {
                s.Close();
                Trace.WriteLine(string.Format("failed to connect to {0}\r\n{1}", remoteEP, ex));
                throw new ApplicationException(string.Format("failed to connect to {0}", remoteEP), ex);
            }
            _direction = SocketConnectionDirection.Outgoing;
            _socket = s;
            _remoteEP = remoteEP;
        }

        public void Connect(IPEndPoint localEP, IPEndPoint remoteEP, int bufferSize)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                s.Bind(localEP);
            }
            catch (Exception ex)
            {
                s.Close();
                Trace.WriteLine(string.Format("failed to bind to {0}\r\n{1}", localEP, ex));
                throw new ApplicationException(string.Format("failed to bind to {0}", localEP), ex);
            }
            try
            {
                s.Connect(remoteEP);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, bufferSize * 1024);
                s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, bufferSize * 1024);
            }
            catch (Exception ex)
            {
                s.Close();
                Trace.WriteLine(string.Format("failed to connect to {0}\r\n{1}", remoteEP, ex));
                throw new ApplicationException(string.Format("failed to connect to {0}", remoteEP), ex);
            }

            _direction = SocketConnectionDirection.Outgoing;
            _socket = s;
            _remoteEP = remoteEP;
        }

        public void BeginReceive()
        {
            if (_socket == null)
                throw new ApplicationException(string.Format("AsyncSocketConnection::BeginReceive: socket=null(RemoteEP={0})", _remoteEP));

            try
            {
                _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), _socket);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("AsyncSocketConnection::BeginReceive: RemoteEP={0}", _remoteEP), ex);
            }
        }

        public void Send(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            if (_socket == null)
                throw new ApplicationException(string.Format("AsyncSocketConnection::BeginSend: socket=null(RemoteEP={0})", _remoteEP));

            try
            {
                _socket.BeginSend(buffer, offset, size, flags, new AsyncCallback(BeginSendCallback), _socket);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("AsyncSocketConnection::BeginSend: RemoteEP={0}", _remoteEP), ex);
            }
        }

        public void Close()
        {
            Close(false);
        }

        private void Close(bool force)
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

            s.SafeClose();
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }
        #endregion


        #region Callbacks

        private void BeginReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket s = result.AsyncState as Socket;
                if (!s.Connected)
                    return;

                int size = s.EndReceive(result);
                if (size < 1)
                {
                    Close(true);
                    return;
                }
                
                _evenArgs.SetSize(size);

                lock (_syncRoot)
                {
                    if (DataReceived != null)
                        DataReceived(this, _evenArgs);
                    if (_socket != null)
                        BeginReceive();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
                Close(true);
            }
            catch (Exception ex)
            {
                Close(true);
                Trace.WriteLine(string.Format("receiving data from {0}\r\n{1}", _remoteEP, ex));
            }
        }

        private void BeginSendCallback(IAsyncResult result)
        {
            try
            {
                Socket s = result.AsyncState as Socket;
                s.EndSend(result);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
                Close(true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("sending data to {0}\r\n{1}", _remoteEP, ex));
            }
        }

        #endregion
    }
}

