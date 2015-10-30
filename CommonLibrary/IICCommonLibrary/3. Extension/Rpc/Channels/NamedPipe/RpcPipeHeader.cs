using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct RpcPipeHeader
	{
		public static readonly int Size = Marshal.SizeOf(typeof(RpcPipeHeader));

		public const int MagicMark = 0x00BADBEE;

		[MarshalAs(UnmanagedType.I4)]
		public int Mark;

		[MarshalAs(UnmanagedType.I4)]
		public int ContextSize;

		[MarshalAs(UnmanagedType.I4)]
		public int BodySize;

		public static byte[] ToByteArray(RpcPipeHeader header)
		{
			byte[] buffer = new byte[Size];
			IntPtr structPtr = Marshal.AllocHGlobal(Size);
			Marshal.StructureToPtr(header, structPtr, true);
			Marshal.Copy(structPtr, buffer, 0, Size);
			Marshal.FreeHGlobal(structPtr);
			return buffer;  
		}

		public static RpcPipeHeader FromByteArray(byte[] buffer)
		{
			IntPtr structPtr = Marshal.AllocHGlobal(Size);
			Marshal.Copy(buffer, 0, structPtr, Size);
			object obj = Marshal.PtrToStructure(structPtr, typeof(RpcPipeHeader));
			Marshal.FreeHGlobal(structPtr);
			return (RpcPipeHeader)obj;
		}
	}
}
