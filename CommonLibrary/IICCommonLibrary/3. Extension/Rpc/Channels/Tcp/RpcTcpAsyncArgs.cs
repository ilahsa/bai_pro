using System;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public enum RpcTcpAsyncArgsType
	{
		Accept,
		Recv,
		Send1,
		Send2,
	}

	public class RpcTcpAsyncArgs: SocketAsyncEventArgs, IObjectPoolCacheable
	{
		private byte[] _buffer;
		private int _index;
		private RpcTcpAsyncArgsType _type;
		private Action<SocketAsyncEventArgs> _callback;

		public RpcTcpAsyncArgs(int size, RpcTcpAsyncArgsType type)
			: base()
		{
			if (size != 0) {
				_buffer = new byte[size];
				SetBuffer(_buffer, 0, _buffer.Length);
			}
			_type = type;
		}

		public int CachedIndex
		{
			get { return _index; }
			set { _index = value; }
		}

		public Action<SocketAsyncEventArgs> Callback
		{
			get { return _callback; }
			set { _callback = value; }
		}

		public RpcTcpAsyncArgsType ArgsType
		{
			get { return _type; }
		}

		public void FetchPrepare()
		{
			AcceptSocket = null;
			DisconnectReuseSocket = false;
			RemoteEndPoint = null;
			SendPacketsElements = null;
			SendPacketsSendSize = 0;
			SocketError = SocketError.Success;
			SocketFlags = SocketFlags.None;
			UserToken = null;
			Callback = null;

			_SetSocket(this, null);
			if (_buffer != null)
				SetBuffer(0, _buffer.Length);
		}

		public void Release()
		{
			this.Dispose();
		}

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			if (Callback != null) {
				try {
					Callback(e);
				} catch (Exception ex) {
					_tracing.ErrorFmt(ex, "Callback Failed", this);
				}
			}
		}

		//public override string ToString()
		//{
		//    return String.Format("RpcTcpAsyncArgs - I:{0} ", CachedIndex, StrUtils.ToHexString(_buffer));
		//}

		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpAsyncArgs));

		private static Action<SocketAsyncEventArgs, Socket> _SetSocket = GetSetSocketMethod();
		public static Action<SocketAsyncEventArgs, Socket> GetSetSocketMethod()
		{
			FieldInfo m_CurrentSocket = typeof(SocketAsyncEventArgs).GetField("m_CurrentSocket", BindingFlags.Instance | BindingFlags.NonPublic);

			DynamicMethod method = new DynamicMethod("SetSocket", typeof(void), new Type[] { typeof(SocketAsyncEventArgs), typeof(Socket) }, typeof(RpcTcpAsyncArgs), true);

			ILGenerator il = method.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stfld, m_CurrentSocket);
			il.Emit(OpCodes.Ret);

			return (Action<SocketAsyncEventArgs, Socket>)method.CreateDelegate(typeof(Action<SocketAsyncEventArgs, Socket>));
		}
	}
}
