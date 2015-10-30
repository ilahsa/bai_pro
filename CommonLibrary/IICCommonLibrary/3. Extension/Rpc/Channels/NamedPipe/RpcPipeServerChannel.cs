using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc服务器通道: NamedPipe
	/// </summary>
	public class RpcPipeServerChannel : RpcServerChannel, IDisposable
	{
		private string _pipeName;
		private NamedPipeServerStream[] _pipeServers;

		public RpcPipeServerChannel(string pipeName, int instanceCount)
			: base("pipe", string.Format("pipe://.:{0}", pipeName))
		{
			_pipeName = pipeName;
			_pipeServers = new NamedPipeServerStream[instanceCount];

			for (int i = 0; i < instanceCount; i++) {
				PipeSecurity security = new PipeSecurity();
				security.AddAccessRule(new PipeAccessRule(new NTAccount("Everyone"), PipeAccessRights.FullControl, AccessControlType.Allow));

				_pipeServers[i] = new NamedPipeServerStream(pipeName, PipeDirection.InOut,
					instanceCount, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0,
					security);
			}

			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.None,
				Version = "4.1",
			};
		}

		protected override void DoStart()
		{
			foreach (NamedPipeServerStream stream in _pipeServers) {
				stream.BeginWaitForConnection(new AsyncCallback(ConnectionCallback), stream);
			}
		}

		protected override void DoStop()
		{
			return;
		}

		private void ConnectionCallback(IAsyncResult ar)
		{
			try {
				NamedPipeServerStream stream = (NamedPipeServerStream)ar.AsyncState;
				stream.EndWaitForConnection(ar);

				RpcPipeServerTransaction trans = new RpcPipeServerTransaction(this, stream);
				OnTransactionCreated(trans);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "RpcPipeServerChannel.ConnectionCallback Failed");
			}
		}

		internal void RecycleServerStream(NamedPipeServerStream stream)
		{
			stream.BeginWaitForConnection(new AsyncCallback(ConnectionCallback), stream);
		}

		/// <summary>
		/// 关闭当前流
		/// </summary>
		public void Close()
		{
			foreach (NamedPipeServerStream stream in _pipeServers) {
				stream.Close();
			}
		}

		/// <summary>
		/// 释放资源
		/// </summary>
		public void Dispose()
		{
			Close();
		}
	}
}
