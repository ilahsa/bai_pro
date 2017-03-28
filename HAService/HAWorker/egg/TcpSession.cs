using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Imps.Services.HA;
using System.IO;

namespace HAWorker.egg
{
    public class TcpSession
    {
        private string _serverAddr;
        private Socket _socket;
        public TcpSession(string serverAddr) {
            _serverAddr = serverAddr;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Connect() {
            try
            {
                if (_socket == null) {
                    throw new Exception("socket is null");
                }

                if (_socket.Connected)
                {
                    return true;
                }

                _socket.Connect(IPAddress.Parse(_serverAddr), 80);
                if (_socket.Connected)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex) {
                EggLog.Info("socket connect error "+ex.Message);
                return false;
            }
            
        }


        public byte[] Send(byte[] msg) {
            try
            {
                if (!Connect()) {
                    Connect();
                }
                _socket.Send(msg);

                byte[] buffer = new byte[4];
                _socket.Receive(buffer, 4, SocketFlags.None);
                Int32 dataLen = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[dataLen];

                _socket.Receive(buffer, dataLen, SocketFlags.None);
            

                return buffer;
            }
            catch (Exception ex) {
                EggLog.Info("send occur error " + ex.Message);
            }
            return null;
        }

        public void SafeClose()
        {
            if (_socket == null)
                return;

            if (!_socket.Connected)
                return;

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            try
            {
                _socket.Close();
            }
            catch
            {
            }
        }
    }
}
