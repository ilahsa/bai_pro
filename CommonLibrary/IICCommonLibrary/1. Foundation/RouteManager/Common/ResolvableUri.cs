using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public enum RouteMethod
	{
		Auto = 0,
		Sipc = 1,
		Rpc = 2,
		Database = 3,
		Http = 4,
		Remoting = 5,
		Sip = 6,
	};

	public abstract class ResolvableUri : BaseUri
	{
		private string _serviceName;
		private RouteMethod _method;

		public string Service
		{
			get { return _serviceName; }
			set { _serviceName = value; }
		}

		public RouteMethod Method
		{
			get { return _method; }
			set { _method = value; }
		}

		public ResolvableUri(string protocol, string serviceName)
			: base(protocol)
		{
			_serviceName = serviceName;
			_method = RouteMethod.Auto;
		}

		public abstract ServerUri Resolve(RouteMethod method);

		public override bool Equals(object obj)
		{
			ResolvableUri rval = obj as ResolvableUri;
			if (rval == null) {
				return false;
			} else {
				return this._serviceName == rval._serviceName &&
					this._method == rval._method &&
					this.Protocol == rval.Protocol;
			}
		}

		public override int GetHashCode()
		{
			return _serviceName.GetHashCode() ^
				Protocol.GetHashCode();
		}

		private static IUriParser _parser = null;

		public static IUriParser Parser
		{
			get { return _parser; }
			set { _parser = value; }
		}

		public static ResolvableUri Parse(string uri)
		{
			return _parser.ParseUri(uri);
		}

		public static ServerUri Reslove(string uri, RouteMethod m)
		{
			var r = Parse(uri);
			return r.Resolve(m);
		}

		public static void Parse(string uri, int startIndex, out string value, out string service, Action<string, string> paramFunc)
		{
			//
			// xxx:123@IBS;p=123;m=Auto;
			int at = uri.IndexOf('@', startIndex);
			value = uri.Substring(startIndex, at - startIndex);	// 123;

			int semicolon = uri.IndexOf(';', at);
			if (semicolon < 0) {
				service = uri.Substring(at + 1);
				return;
			}

			service = uri.Substring(at + 1, semicolon - at - 1);

			while (semicolon > 0) {
				string paramField;
				string paramValue;

				int nextEqual = uri.IndexOf('=', semicolon);

				if (nextEqual > 0) {
					paramField = uri.Substring(semicolon + 1, nextEqual - semicolon - 1);
				} else {
					break;
				}

				int nextSemicolon = uri.IndexOf(';', nextEqual);

				if (nextSemicolon > 0) {
					paramValue = uri.Substring(nextEqual + 1, nextSemicolon - nextEqual - 1);
				} else {
					paramValue = uri.Substring(nextEqual + 1);
				}

				paramFunc(paramField, paramValue);
				semicolon = nextSemicolon;
			}
		}
	}
}
