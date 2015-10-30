using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
    /// <summary>
    ///		Rpc的双工客户端程序,允许保持长连接,并允许从服务器端进行反向调用
    /// </summary>
    public class RpcDuplexClient
    {
        #region Private member
        private int _timeout;
        private ServerUri _serverUri;
        private RpcConnection _connection;
        private RpcServiceDispather _dispatcher;
        private RpcClientChannel _channel;
        #endregion

        #region Public members
        /// <summary>是否连接上</summary>
        public bool Connected
        {
            get { return _connection.Connected; }
        }

        /// <summary>目标的服务Uri</summary>
        public ServerUri ServerUri
        {
            get { return _serverUri; }
        }

        /// <summary>Nexus</summary>
        public string TargetService
        {
            get { return ""; }
        }

        /// <summary>Nexus</summary>
        public string TargetComputer
        {
            get { return ""; }
        }

        /// <summary>超时</summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        ///		断开连接时触发, 连接失败不会触发
        /// </summary>
        public event Action<RpcDuplexClient> Disconnected;
        #endregion

        #region Constructor
        public RpcDuplexClient(ServerUri serverUri)
        {
            _serverUri = serverUri;

            _channel = RpcProxyFactory.GetChannel(serverUri);
            _timeout = _channel.Timeout;

            _connection = _channel.CreateConnection(serverUri, RpcConnectionMode.Duplex);
            _connection.Disconnected += new Action<RpcConnection>(
                (c) =>
                {
                    OnDisconnected();
                }
            );
            _connection.TransactionCreated += new Action<RpcConnection, RpcServerTransaction>(
                (c, tx) =>
                {
                    _dispatcher.ProcessTransaction(tx);
                }
            );

            _dispatcher = new RpcServiceDispather("duplex");
        }

        /// <summary>
        ///		异步尝试Connect到服务器端, 并在连接后直接注册
        /// </summary>
        /// <param name="callback">完成后的回调</param>
        public void BeginConnect(Action<Exception> callback)
        {
            _connection.BeginConnect(
                delegate(Exception ex)
                {
                    callback(ex);
                }
            );
        }

        /// <summary>
        ///		同步连接，默认超时时间
        /// </summary>
        public void Connect()
        {
            Connect(_timeout);
        }

        /// <summary>
        ///		连接，使用参数专递的超时时间
        /// </summary>
        /// <param name="connectionTimeout">超时时间</param>
        public void Connect(int connectionTimeout)
        {
            var a = new SyncInvoker<Exception>();
            BeginConnect(a.Callback);
            a.Wait(_timeout);
            if (a.Context != null)
            {
                throw a.Context;
            }
        }
        #endregion

        /// <summary>
        ///		获取一个客户端代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RpcClientProxy GetProxy<T>()
        {
            return GetProxy<T>(null);
        }

        /// <summary>
        ///		获取一个客户端代理, 带ContextUri
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RpcClientProxy GetProxy<T>(ResolvableUri uri)
        {
            var intf = RpcClientInterfaceFactory<T>.GetOne();
			return new RpcClientProxy(_connection, intf, uri);
        }

        /// <summary>
        ///		注册回调Service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rpcService"></param>
        public void RegisterService<T>(T rpcService)
        {
            _dispatcher.RegisterService(new RpcServiceDecorator<T>(rpcService));
        }

        /// <summary>
        ///		注册回调Service
        /// </summary>
        /// <param name="service"></param>
        public void RegisterRawService(RpcServiceBase service)
        {
            _dispatcher.RegisterService(service);
        }

        /// <summary>
        ///		接口支持服务: 创建Transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RpcClientTransaction CreateTransaction(RpcRequest request)
        {
            return _connection.CreateTransaction(request);
        }

        #region private methods
        protected void OnDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this);
            }
        }
        #endregion
    }
}
