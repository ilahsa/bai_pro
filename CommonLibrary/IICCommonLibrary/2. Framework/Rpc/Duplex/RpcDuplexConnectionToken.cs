using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcDuplexConnectionToken
	{
		private RpcDuplexServer _duplexServer;
		//private RpcConnection _connection;
		private string _clientService;
		//private string _clientComputer;
		//private string _clientId;

		public RpcDuplexConnectionToken(RpcDuplexServer server)
		{
			_duplexServer = server;
			_clientService = null;
		}

		public bool Registerd
		{
			get { throw new NotImplementedException(); }
		}

		public string ClientService
		{
			get { return _clientService; }
		}

		public void ProcessRegister(RpcServerContext ctx)
		{
			_duplexServer.ProcessRegister(this, ctx);
		}

		public void ProcessKeepalive(RpcServerContext ctx)
		{
			_duplexServer.ProcessKeepAlive(this, ctx);
		}
	}
}
