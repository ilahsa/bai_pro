using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
    public class JsonRpcException:Exception
    {
		private string _serviceUrl;
		private string _message;

		public string JsonRpcExceptionMessage
		{
			get { return _message; }
		}

		public string ServiceUrl
		{
			get { return _serviceUrl; }
		}

		public JsonRpcException(string serviceUrl, string message, Exception ex)
			: base(string.Format("RpcException on {0} '{1}'", serviceUrl, message), ex)
		{
			_serviceUrl = serviceUrl;
			_message = message;
		}

		public override string ToString()
		{
			return string.Format("RpcException on \"{0}\" ({1}) \r\n{2}",
                _serviceUrl, _message, InnerException);
		}
    }
}
