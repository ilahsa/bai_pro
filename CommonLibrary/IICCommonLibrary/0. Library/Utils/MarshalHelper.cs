using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
    public static class MarshalHelper
    {
        public static byte[] StructToBuffer<T>(T st)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(st, structPtr, true);
            Marshal.Copy(structPtr, buffer, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return buffer;
        }

        public static T BufferToStruct<T>(byte[] buffer)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, typeof(T));
            Marshal.FreeHGlobal(structPtr);
            return (T)obj;
        }

        public static T StructFromStream<T>(Stream stream)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            stream.Read(buffer, 0, buffer.Length);
            return BufferToStruct<T>(buffer);
        }
    }
}
