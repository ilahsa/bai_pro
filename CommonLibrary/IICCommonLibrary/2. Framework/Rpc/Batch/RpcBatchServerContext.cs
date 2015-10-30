using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public class RpcBatchServerContext
	{	
		private int _count;
		private RpcBatchRequest[] _requests;
		private RpcBatchResponse[] _responses;
		private RpcServerContext _innerCtx;

		public string From
		{
			get { return _innerCtx.From; }
		}

		public string FromService
		{
			get { return _innerCtx.FromService; }
		}

		public string FromComputer
		{
			get { return _innerCtx.FromComputer; }
		}

		public string ServiceName
		{
			get { return _innerCtx.ServiceName; }
		}

		public string MethodName
		{
			get { return _innerCtx.MethodName; }
		}

		internal RpcBatchServerContext(RpcServerContext ctx)
		{
			_innerCtx = ctx;
			_requests = ctx.GetArgs<RpcBatchRequest[]>();
			_count = _requests.Length;
			_responses = new RpcBatchResponse[_count];
		}

		public int ArgsCount
		{
			get { return _requests.Length; }
		}

		public T GetArgs<T>(int index)
		{
			if (_requests[index].HasBody) {
				return ProtoBufSerializer.FromByteArray<T>(_requests[index].RequestData);
			} else {
				return default(T);
			}
		}

		public T GetContextUri<T>(int index) where T: ResolvableUri
		{
			var u = _requests[index].ContextUri;
			if (!string.IsNullOrEmpty(u)) {
				return (T)ResolvableUri.Parse(u);
			} else {
				return null;
			}
		}

		public void SetResults<T>(int i, T value)
		{
			_responses[i] = new RpcBatchResponse() {
				ErrorCode = RpcErrorCode.OK,
				ResponseData = ProtoBufSerializer.ToByteArray(value),
			};
		}

		public void SetResults(int i, RpcErrorCode code, Exception ex)
		{
			_responses[i] = new RpcBatchResponse() {
				ErrorCode = code,
				ResponseData = ex != null ? Encoding.UTF8.GetBytes(ex.ToString()) : null
			};
		}

		public void SetResults(int i, Exception ex)
		{
			_responses[i] = new RpcBatchResponse() {
				ErrorCode = RpcErrorCode.ServerError,
				ResponseData = ex != null ? Encoding.UTF8.GetBytes(ex.ToString()) : null
			};
		}

		public void Return()
		{
			_innerCtx.Return(_responses);
		}

		public void ReturnError(RpcErrorCode errCode, Exception ex)
		{
			_innerCtx.ReturnError(errCode, ex);
		}

		public void ThrowException(Exception ex)
		{
			ReturnError(RpcErrorCode.ServerError, ex);
		}

		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcServerContext));
		private static RpcServerPerfCounter _perfCounters = IICPerformanceCounterFactory.GetCounters<RpcServerPerfCounter>();
	}
}
