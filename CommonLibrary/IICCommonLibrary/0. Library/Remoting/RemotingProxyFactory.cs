using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;

namespace Imps.Services.CommonV4
{
    public static class RemotingProxyFactory
    {
        private static bool _startTcp = false;
        private static bool _startHttp = false;
        private static bool _startIpc = false;

        private static IChannel _tcpChannel = null;
        private static IChannel _ipcChannel = null;
        private static IChannel _httpChannel = null;

        private static object _syncRoot = new object();
        private static Dictionary<Type, ComboClass<string, string>> _registeredServices = new Dictionary<Type, ComboClass<string, string>>();
        private static Dictionary<string, RemotingCachedObjectPool> _cachedObjects = new Dictionary<string, RemotingCachedObjectPool>();

        public const int DefaultTimeout = 30000;

        public static T GetObject<T>(string url)
        {
            RemotingCachedObjectPool pool;
            lock (_syncRoot)
            {
                if (!_cachedObjects.TryGetValue(url, out pool))
                {
                    pool = new RemotingCachedObjectPool(url, 8, typeof(T), 90);
                    _cachedObjects.Add(url, pool);
                }
            }
            return pool.GetProxyObject<T>();
        }

        public static void RegisterObject<T>(string serviceName, string url)
        {
            lock (_syncRoot)
            {
                _registeredServices.Add(typeof(T), new ComboClass<string, string>(serviceName, url));
            }
        }

        private static void CheckUriAndRegisterChannel(string uri)
        {
            if (!_startTcp && uri.ToLower().StartsWith("tcp"))
            {
                Hashtable props = new Hashtable();
                props["name"] = "tcp_rem";
                props["timeout"] = DefaultTimeout;

                _tcpChannel = new TcpChannel(props, null, null);
                _startTcp = true;

                ChannelServices.RegisterChannel(_tcpChannel, false);
            }
            else if (!_startHttp && uri.ToLower().StartsWith("http"))
            {
                Hashtable props = new Hashtable();
                props["name"] = "http_rem";
                props["timeout"] = DefaultTimeout;

                _httpChannel = new HttpChannel(props, null, null);
                _startHttp = true;
                ChannelServices.RegisterChannel(_httpChannel, false);
            }
            else if (!_startIpc && uri.ToLower().StartsWith("ipc"))
            {
                Hashtable props = new Hashtable();
                props["name"] = "ipc_rem";
                props["priority"] = "20";
                props["authorizedGroup"] = "Everyone";
                props["connectionTimeout"] = DefaultTimeout;

                _ipcChannel = new IpcClientChannel(props, null);
                _startIpc = true;
                ChannelServices.RegisterChannel(_ipcChannel, false);
            }
        }
    }
}
