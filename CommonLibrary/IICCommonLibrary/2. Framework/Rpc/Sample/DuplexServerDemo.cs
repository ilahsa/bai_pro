using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc.Sample
{
	[RpcService("DuplexDemoService")]
	interface IDuplexDemoService
	{
		[RpcServiceMethod(ArgsType=typeof(DuplexDemoRegisterArgs), ResultType=null)]
		void Register(RpcServerContext ctx);

		[RpcServiceMethod(ArgsType=typeof(int), ResultType=typeof(int))]
		void KeepAlive(RpcServerContext ctx);
	}

	[RpcService("DuplexDemoCallbackService")]
	interface IDuplexDemoCallbackService
	{
		[RpcServiceMethod(ArgsType=typeof(string), ResultType=null)]
		void CallCommand(RpcServerContext ctx);
	}

	[ProtoContract]
	public class DuplexDemoRegisterArgs
	{
		[ProtoMember(1)]
		public string UserName;

		[ProtoMember(2)]
		public string Passwd;
	}

	class DuplexDemoSession
	{
		public string Id;

		public DateTime LastTime;

		public RpcConnection Connection;
	}

	class DuplexDemoService: IDuplexDemoService
	{
		private object _syncRoot = new object();
		private Dictionary<string, DuplexDemoSession> _agents = new Dictionary<string,DuplexDemoSession>();

		public void Register(RpcServerContext ctx)
		{
			//
			// 已经连接过的就返回失败 
			if (ctx.Connection.Contexts["session"] != null) {
				ctx.ReturnError(RpcErrorCode.ConnectionBroken, null);
				return;
			}

			var args = ctx.GetArgs<DuplexDemoRegisterArgs>();
			if (args.UserName == "foo" && args.Passwd == "bar") {
				//
				// 注册成功， 建立session对象
				var session = new DuplexDemoSession() {
					Id = Guid.NewGuid().ToString(),
					LastTime = DateTime.Now,
					Connection = ctx.Connection,
				};
				ctx.Connection.Contexts["session"] = session;
				lock (_syncRoot) {
					_agents.Add(ctx.From, session);
				}
				ctx.Return();
			} else {
				ctx.ReturnError(RpcErrorCode.ConnectionFailed, new Exception("UserNotFound"));
			}
		}

		public void KeepAlive(RpcServerContext ctx)
		{
			int seconds = ctx.GetArgs<int>();
			DuplexDemoSession session = (DuplexDemoSession)ctx.Connection.Contexts["session"];
			session.LastTime = DateTime.Now.AddSeconds(seconds);
			ctx.Return(0);	// 没意义
		}


		public void SendCommand(string from, string command)
		{
			lock (_syncRoot) {
				DuplexDemoSession session = _agents[from];
				RpcClientProxy proxy = session.Connection.GetCallbackProxy<IDuplexDemoCallbackService>();
				proxy.Invoke<string, string>("CallCommand", command);
			}
		}
	}



	public class DuplexDemoServer
	{
		static void Initialize()
		{
			RpcTcpServerChannel channel = new RpcTcpServerChannel(8888);
			RpcDuplexServer server = new RpcDuplexServer(channel);

			//
			// 如果Session没有建立成功，如果发起非注册的信令，直接断开连接
			server.BeforeTransactionCreated += new Action<RpcServerTransaction>(
				delegate (RpcServerTransaction tx) {
					if (tx.Connection.Contexts["session"] == null) {
						if (tx.Request.ServiceDotMethod != "DuplexDemoService.Register") {
							tx.Connection.Disconnect();
						}
					}
				}
			);
		}
	}


	class DuplexDemoCallbackService: IDuplexDemoCallbackService
	{
		public void Start(RpcServerContext ctx)
		{
			throw new NotImplementedException();
		}

		public void Stop(RpcServerContext ctx)
		{
			throw new NotImplementedException();
		}

		public void CallCommand(RpcServerContext ctx)
		{
			throw new NotImplementedException();
		}
	}

	public class DuplexDemoClient
	{
		static void Initialize()
		{
			RpcTcpClientChannel channel = new RpcTcpClientChannel();
			RpcDuplexClient client = new RpcDuplexClient(new TcpUri("127.0.0.1", 8888));
			client.RegisterService<IDuplexDemoCallbackService>(new DuplexDemoCallbackService());
			client.Connect(10000);
			var proxy = client.GetProxy<IDuplexDemoService>();
			proxy.Invoke<DuplexDemoRegisterArgs, RpcNull>(
				"Regiser",
				new DuplexDemoRegisterArgs() {
					UserName = "223",
					Passwd = "12",
				}
			);
		}
	}
}
