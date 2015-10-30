using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	//
	// 暂时去掉Dispose方法
	public abstract class RpcClientTransaction : IDisposable
	{
		//private bool _disposed = false;

		public string ServiceRole;

		public ServerUri ServerUri;

		public RpcRequest Request;

		public RpcResponse Response = null;

		public string ServiceUrl
		{
			get { return string.Format("{0}/{1}.{2}", ServerUri, Request.Service, Request.Method); }
		}

		public RpcClientTransaction(ServerUri uri, RpcRequest request)
		{
			ServerUri = uri;
			Request = request;
		}

		//~RpcClientTransaction()
		//{
		//    Dispose(false);
		//}		

		public void Dispose()
		{
		    Dispose(true);
		    //GC.SuppressFinalize(this);
			//_disposed = true;
		}

		protected virtual void Dispose(bool disposing)
		{
		    // if (disposing) {
		        // release all the resources no longer needed
		        // ...
		}
		//}
		
		public abstract void SendRequest(Action callback, int timeout);
	}
}
