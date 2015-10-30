using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	[Serializable]
	public class RpcException: Exception
	{
		private RpcErrorCode _code;
		private string _serviceUrl;
		private string _message;

		public RpcErrorCode RpcCode
		{
			get { return _code; }
		}

		public string RpcMessage
		{
			get { return _message; }
		}

		public string ServiceUrl
		{
			get { return _serviceUrl; }
		}

		public RpcException(RpcErrorCode code, string serviceUrl, string message, Exception ex)
			: base(string.Format("RpcException<{0}> on {1} '{2}'", code, serviceUrl, message), ex)
		{
			_code = code;
			_serviceUrl = serviceUrl;
			_message = message;
		}

		protected RpcException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string ToString()
		{
			return string.Format("RpcException<{0}> on \"{1}\" ({2}) \r\n{3}",
				_code, _serviceUrl, _message, InnerException);
		}
	}
}
