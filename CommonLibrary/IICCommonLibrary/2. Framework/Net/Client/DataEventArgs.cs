using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Net
{
    public class DataEventArgs : EventArgs
    {
        public byte[] Data { get; set; }

        public int Offset { get; set; }

        public int Length { get; set; }
    }
}
