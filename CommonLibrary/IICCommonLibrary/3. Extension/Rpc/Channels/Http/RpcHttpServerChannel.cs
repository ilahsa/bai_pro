using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpServerChannel : RpcServerChannel
	{
		#region Private Member
		private HttpListener _listener;
		#endregion

		#region Constructors
		public RpcHttpServerChannel(string prefix)
			: base("http", "http://*/" + prefix)
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add(prefix);
		}

		public RpcHttpServerChannel(int port)
			: this(string.Format("http://*:{0}/", port))
		{
			p_channelSettings = new RpcChannelSettings() {
				MaxBodySize = 512 * 1024 * 1024,
				SupportModes = RpcChannelSupportModes.None,
				Version = "4.1",
			};
		}
		#endregion

		#region Overrided Abstract Methods
		protected override void DoStart()
		{
			_listener.Start();
			_listener.BeginGetContext(new AsyncCallback(ListenerCallback), this);
		}

		protected override void DoStop()
		{
			_listener.Stop();
		}
		#endregion

		#region Private Listener Logic
		private static void ListenerCallback(IAsyncResult result)
		{
			try {
				RpcHttpServerChannel channel = (RpcHttpServerChannel)result.AsyncState;

				// Call EndGetContext to complete the asynchronous operation.
				HttpListenerContext context = channel._listener.EndGetContext(result);
				channel._listener.BeginGetContext(new AsyncCallback(ListenerCallback), channel);
				ProcessRequest(channel, context);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "RpcHttpChannel.ListenerCallback Failed");
			}
		}
			
		private static void ProcessRequest(RpcHttpServerChannel channel, HttpListenerContext httpContext)
		{
			try {
				RpcHttpServerTransaction tx = new RpcHttpServerTransaction(channel, httpContext);
				channel.OnTransactionCreated(tx);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "RpcServiceStartFailed");
			}
		}
		#endregion
	}
}