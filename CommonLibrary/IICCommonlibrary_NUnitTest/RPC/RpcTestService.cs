using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Imps.Services.CommonV4;

namespace UnitTest.Rpc
{
	[RpcService("RpcUnitTest", EnableCounters = RpcPerformanceCounterMode.Both)]
	public interface IRpcTestService
	{
		[RpcServiceMethod(ArgsType = typeof(RpcClass<int, int>), ResultType = typeof(int))]
		void Add(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(RpcClass<int>), ResultType = typeof(RpcClass<int>))]
		void Mirror(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(RpcClass<int>), ResultType = typeof(RpcClass<int>))]
		void Null(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(RpcClass<int>), ResultType = typeof(RpcClass<int>))]
		void Default(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(byte[]), ResultType = typeof(byte[]))]
		void Array(RpcServerContext context);

		[RpcServiceMethod(ArgsType = typeof(string), ResultType = typeof(string))]
		void TestException(RpcServerContext context);

		[RpcServiceMethod(ArgsType = null, ResultType = null)]
		void TestTimeout(RpcServerContext context);

		[RpcServiceBatchMethod(32, 2000, ArgsType = typeof(string), ResultType = typeof(string))]
		void TestBatch(RpcBatchServerContext context);
	}

	public class RpcTestService: IRpcTestService
	{
		public RpcTestService()
		{
		}

		public void Add(RpcServerContext context)
		{
			RpcClass<int, int> c = context.GetArgs<RpcClass<int, int>>();
			// Console.WriteLine("Rpc.Test1 {0} {1}", c.Value1, c.Value2);
			context.Return(c.Value1 + c.Value2);
		}

		public void Mirror(RpcServerContext context)
		{
			var e = context.GetArgs<RpcClass<int>>();
			context.Return(e);
		}

		public void Null(RpcServerContext context)
		{
			context.Return<RpcClass<int>>(null);
		}

		public void Default(RpcServerContext context)
		{
			RpcClass<int> a = new RpcClass<int>();
			context.Return(a);
		}

		public void Array(RpcServerContext context)
		{
			byte[] a = context.GetArgs<byte[]>();
			context.Return(a);
		}

		public void TestException(RpcServerContext context)
		{
			context.ThrowException(new ApplicationException("TestException"));
		}

		public void TestTimeout(RpcServerContext context)
		{
			Thread.Sleep(60000);
			context.Return();
		}

		public void TestBatch(RpcBatchServerContext ctx)
		{
			for (int i = 0; i < ctx.ArgsCount; i++) {
				string s = ctx.GetArgs<string>(i);
				// IdUri id = ctx.GetContextUri<IdUri>(i);

				// Run Something
				ctx.SetResults(i, s + ":OK");
			}
			ctx.Return();
		}
	}
}
