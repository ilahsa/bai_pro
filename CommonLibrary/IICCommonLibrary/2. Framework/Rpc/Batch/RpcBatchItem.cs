using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class RpcBatchItem<T>
	{
		private RpcBatchServerContext _ctx;
		private int _idx;

		private T _args;
		private ResolvableUri _uri;

		internal RpcBatchItem(RpcBatchServerContext ctx, int i)
		{
			_ctx = ctx;
			_idx = i;
			_args = ctx.GetArgs<T>(_idx);
			_uri = ctx.GetContextUri<ResolvableUri>(_idx);
		}

		public T Args
		{
			get { return _args; }
		}

		public ResolvableUri ContextUri
		{
			get { return _uri; }
		}

		public void SetResults<TResults>(TResults ret)
		{
			_ctx.SetResults<TResults>(_idx, ret);
		}

		public void SetResults(RpcErrorCode code, Exception ex)
		{
			_ctx.SetResults(_idx, code, ex);
		}

		public void SetResults(Exception ex)
		{
			_ctx.SetResults(_idx, ex);
		}
	}
}
