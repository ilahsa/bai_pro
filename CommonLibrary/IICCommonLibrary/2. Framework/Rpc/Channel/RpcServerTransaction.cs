using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		服务器端事务的封装
	/// </summary>
	public abstract class RpcServerTransaction
	{
		private RpcRequest _request;
		private RpcConnection _connection;
		private RpcServerChannel _channel;
		
		/// <summary>
		///		RPC请求
		/// </summary>
		public RpcRequest Request
		{ 
			get { return _request; }
		}

		/// <summary>
		///		允许其他事件优先处理RpcServerTransaction, 
		///		并在RpcServiceManager的TransactionCreated事件中跳过请求的处理
		/// </summary>
		public bool Bypassed
		{
			get;
			set;
		}

		/// <summary>
		///		连接对象
		/// </summary>
		public RpcConnection Connection 
		{
			get { return _connection; }
		}

		public RpcServerChannel Channel 
		{
			get { return _channel; }
		}

		protected void SetRequest(RpcRequest request)
		{
			_request = request;
		}

		protected RpcServerContext CreateContext()
		{
			return new RpcServerContext(this);
		}

		public RpcServerTransaction(RpcServerChannel channel, RpcConnection conn, RpcRequest request)
		{
			_connection = conn;
			_channel = channel;
			_request = request;
		}

		public string ServiceUrl
		{
			get { return string.Format("{0}/{1}.{2}", Channel.ServerUrl, Request.Service, Request.Method); }
		}

		public abstract void SendResponse(RpcResponse response);
	}
}
