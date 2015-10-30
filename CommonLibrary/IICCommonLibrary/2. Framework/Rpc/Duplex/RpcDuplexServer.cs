using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcDuplexServer
	{
		#region private fields
		private object _syncRoot = new object();
		private RpcServerChannel _channel;
		private RpcServiceDispather _dispatcher;
		#endregion

		#region public & constructors
		public event Action<RpcServerTransaction> BeforeTransactionCreated;
		public event Action<RpcConnection> BeforeConnectionCreated;

		public RpcDuplexServer(RpcServerChannel channel)
		{
			channel.ConnectionCreated += new Action<RpcConnection>(ConnectionCreated);
			channel.TransactionCreated += new Action<RpcServerTransaction>(TransactionCreated);
			_channel = channel;

			_dispatcher = new RpcServiceDispather("duplex");
		}

		public void RegisterRawService(RpcServiceBase service)
		{
			_dispatcher.RegisterService(service);
		}

		public void RegisterService<IService>(IService service)
		{
			var srv = new RpcServiceDecorator<IService>(service);
			_dispatcher.RegisterService(srv);
		}
		#endregion


		#region private methods
		private void TransactionCreated(RpcServerTransaction tx)
		{
			if (BeforeTransactionCreated != null) {
				BeforeTransactionCreated(tx);
			}
			_dispatcher.ProcessTransaction(tx);
		}

		private void ConnectionCreated(RpcConnection conn)
		{
			//var token = new RpcDuplexConnectionToken(this);
			//conn.Contexts["duplex"] = token;
			if (BeforeConnectionCreated != null) {
				BeforeConnectionCreated(conn);
			}
		}
		#endregion
	}
}
