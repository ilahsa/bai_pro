using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Net
{
    public interface IBufferSetter
    {
        void SetBuffer(ArraySegment<byte> bufferSegment);
    }
}
