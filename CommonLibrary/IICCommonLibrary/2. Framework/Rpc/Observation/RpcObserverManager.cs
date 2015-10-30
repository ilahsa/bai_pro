using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Observation;

namespace Imps.Services.CommonV4.Rpc
{
	static class RpcObserverManager
	{
		static object _syncServer = new object();
		static object _syncClient = new object();

		static Dictionary<ComboClass<string, string, string, string>, RpcServerObserverItem> _serverDic;
		static Dictionary<ComboClass<string, string, string>, RpcClientObserverItem> _clientDic;

		static RpcObserverManager()
		{
			_clientDic = new Dictionary<ComboClass<string, string, string>, RpcClientObserverItem>();
			_serverDic = new Dictionary<ComboClass<string, string, string, string>, RpcServerObserverItem>();

			ObserverManager.RegisterObserver("RpcClient", ObserveClient, ClearClient);
			ObserverManager.RegisterObserver("RpcServer", ObserveServer, ClearServer);
		}

		public static RpcClientObserverItem GetClientItem(string serverUri, string service, string method, string serviceRole)
		{
			RpcClientObserverItem ret;
			var key = new ComboClass<string, string, string>(serverUri, service, method);

			lock (_syncClient) {
				if (!_clientDic.TryGetValue(key, out ret)) {
					ret = new RpcClientObserverItem(serverUri, service, method, serviceRole);
					_clientDic.Add(key, ret);
				}
			}
			return ret;
		}

		public static RpcServerObserverItem GetServerItem(string service, string method, string fromService, string fromComputer)
		{
			RpcServerObserverItem ret;
			var key = new ComboClass<string, string, string, string>(service, method, fromService, fromComputer);

			lock (_syncServer) {
				if (!_serverDic.TryGetValue(key, out ret)) {
					ret = new RpcServerObserverItem(service, method, fromService, fromComputer);
					_serverDic.Add(key, ret);
				}
			}
			return ret;
		}

		static List<ObserverItem> ObserveClient()
		{
			var ret = new List<ObserverItem>();
			lock (_syncClient) {
				foreach (var k in _clientDic) {
					ret.Add(k.Value);
				}
			}
			return ret;
		}

		static void ClearClient()
		{
			lock (_syncClient) {
				_clientDic.Clear();
			}
		}

		static List<ObserverItem> ObserveServer()
		{
			var ret = new List<ObserverItem>();
			lock (_syncServer) {
				foreach (var k in _serverDic) {
					ret.Add(k.Value);
				}
			}
			return ret;			
		}

		static void ClearServer()
		{
			lock (_syncServer) {
				_serverDic.Clear();
			}
		}
	}

}
