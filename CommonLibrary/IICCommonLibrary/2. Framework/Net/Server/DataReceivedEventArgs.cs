using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Net
{
    [Serializable]
    public class DataReceivedEventArgs : EventArgs
    {
        private byte[] _buffer;
        private int _size;

        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public int Size
        {
            get { return _size; }
        }

        public DataReceivedEventArgs(byte[] buffer)
        {
            _buffer = buffer;
        }

        internal void SetSize(int size)
        {
            _size = size;
        }

    }

    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
