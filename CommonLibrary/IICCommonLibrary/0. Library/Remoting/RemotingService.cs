using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;
using System.Security.AccessControl;

namespace Imps.Services.CommonV4
{
    public enum RemotingProtocol
    {
        Tcp,
        Http,
        Ipc,
    }

    public class RemotingService
    {
        public static void RegisterChannel(RemotingProtocol protocol, int port)
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IChannel channel;
            switch (protocol)
            {
                case RemotingProtocol.Tcp:
                    {
                        Hashtable props = new Hashtable();
                        props["port"] = port;
                        props["name"] = "tcp_remoting";
                        props["timeout"] = 8000;
                        channel = new TcpServerChannel(props, provider, null);
                    }
                    break;
                case RemotingProtocol.Ipc:
                    {
                        Hashtable props = new Hashtable();

                        props["name"] = "ipc_remoting";
                        props["priority"] = "20";
                        props["portName"] = "localhost:" + port.ToString();
                        props["authorizedGroup"] = "Everyone";

                        IpcServerChannel ipcchannel = new IpcServerChannel(props, provider, null);
                        ipcchannel.IsSecured = false;
                        channel = ipcchannel;
                    }
                    break;
                case RemotingProtocol.Http:
                    {
                        Hashtable props = new Hashtable();
                        props["port"] = port;
                        props["name"] = "http_remoting";
                        channel = new HttpChannel(props, null, provider);
                    }
                    break;
                default:
                    throw new Exception("Unknown Protocol: " + protocol);
            }

            _channels.Add(channel);
            ChannelServices.RegisterChannel(channel, false);

            if (_firstRun)
            {
                lock (_syncRoot)
                {
                    if (_firstRun)
                    {
                        LifetimeServices.LeaseTime = new TimeSpan(0, 30, 0);
                        LifetimeServices.RenewOnCallTime = new TimeSpan(0, 20, 0);
                        _firstRun = false;
                    }
                }

                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                RemotingConfiguration.CustomErrorsEnabled(false);
            }
        }

        public static void RegisterService<T>(string uri, WellKnownObjectMode mode) where T : MarshalByRefObject
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(T), uri, mode);
        }

        public static void Start()
        {
            foreach (IChannel channel in _channels)
            {
                if (channel is TcpChannel)
                {
                    (channel as TcpChannel).StartListening(null);
                }
                else if (channel is IpcChannel)
                {
                    (channel as IpcChannel).StartListening(null);
                }
                else if (channel is HttpChannel)
                {
                    (channel as HttpChannel).StartListening(null);
                }
            }
        }

        public static void Stop()
        {
            foreach (IChannel channel in _channels)
            {
                if (channel is TcpChannel)
                {
                    (channel as TcpChannel).StopListening(null);
                }
                else if (channel is IpcChannel)
                {
                    (channel as IpcChannel).StopListening(null);
                }
                else if (channel is HttpChannel)
                {
                    (channel as HttpChannel).StopListening(null);
                }
            }
        }

        private static bool _firstRun = true;
        private static object _syncRoot = new object();
        private static List<IChannel> _channels = new List<IChannel>();
    }
}
