using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpServerTransaction: RpcServerTransaction
	{
		private HttpListenerContext _httpContext;

		public RpcHttpServerTransaction(RpcHttpServerChannel channel, HttpListenerContext ctx)
			: base(channel, null, ParseRequest(ctx))
		{
			_httpContext = ctx;
		}

		private static RpcRequest ParseRequest(HttpListenerContext ctx)
		{
			HttpListenerRequest webRequest = ctx.Request;

			var req = new RpcRequest() {
				ServiceAtComputer = webRequest.Headers["From"],
				ServiceDotMethod = ExtractServiceDotMethod(webRequest),
				ContextUri = ObjectHelper.ToString(webRequest.Headers["ToUri"])
			};

			if (webRequest.Headers["Null"] != "true") {
				if (webRequest.ContentLength64 == 0) {
					req.BodyBuffer = RpcBodyBuffer.EmptyBody;
				} else {
					req.BodyBuffer = new RpcBodyBuffer(webRequest.InputStream, (int)webRequest.ContentLength64);
				}
			} else {
				req.BodyBuffer = null;
			}
			return req;
		}

		public override void SendResponse(RpcResponse resp)
		{
			HttpListenerResponse webResp = _httpContext.Response;
			webResp.StatusCode = 200;
			webResp.ContentType = "multipart/byteranges";

			if (resp.ErrorCode != RpcErrorCode.OK) {
				webResp.Headers.Add("Warning", resp.ErrorCode.ToString());
			}

			if (resp.BodyBuffer == null) {
				webResp.Headers.Add("Null", "true");
				webResp.ContentLength64 = 0;
			} else {
				byte[] buffer = resp.BodyBuffer.GetByteArray();
				if (buffer.Length > 0) {
					webResp.ContentLength64 = buffer.Length;
					webResp.OutputStream.Write(buffer, 0, buffer.Length);
					webResp.OutputStream.Close();
				} else {
					webResp.ContentLength64 = 0;
				}
			}
			webResp.Close();
		}

		public static String ExtractServiceDotMethod(HttpListenerRequest request)
		{
			string url = request.Url.ToString();
			int l = url.LastIndexOf("/");
			String s = url.Substring(l + 1);

			String stl = s.ToLower();
			if (stl == "rpc.aspx" || stl == "rpc.do") {
				String service = request.QueryString["service"];
				String method = request.QueryString["method"];
				return service + "." + method;				
			} else {
				return s;
			}
		}
	}
}
