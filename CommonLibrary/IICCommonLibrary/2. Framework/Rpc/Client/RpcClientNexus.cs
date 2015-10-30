//using System;
//using System.Threading;
//using System.Collections.Generic;
//using System.Text;

//using Imps.Services.CommonV4;

//namespace Imps.Services.CommonV4.Rpc
//{
//    interface IRpcClientNexus
//    {
//        ServerUri ServerUri { get; }

//        string TargetService { get; }

//        string TargetComputer { get; }

//        // RpcChannelSettings ChannelSettings { get; }

//        RpcClientTransaction CreateTransaction(RpcRequest request);
//    }

//    // 这套面向连接的东西非常的傻逼

//    class RpcSimplexClient : IRpcClientNexus
//    {
//        private object _syncRoot = new object();
//        private ServerUri _serverUri;
//        private RpcConnection _connection;
//        private RpcClientChannel _channel;

//        private string _targetService;
//        private string _targetComputer;
//        private RpcChannelSettings _channelSettings;

//        public ServerUri ServerUri
//        {
//            get { return _serverUri; }
//        }

//        public string TargetService
//        {
//            get { return _targetService; }
//        }

//        public string TargetComputer
//        {
//            get { return _targetComputer; }
//        }

//        public RpcChannelSettings ChannelSettings
//        {
//            get { return _channelSettings; }
//        }

//        public RpcSimplexClient(RpcClientChannel channel, ServerUri uri, string serviceRole)
//        {
//            _channel = channel;
//            _serverUri = uri;
//            _targetService = serviceRole;
//            _targetComputer = "";
//            _channelSettings = channel.DefaultSettings;
			

//            if ((_channelSettings.SupportModes & RpcChannelSupportModes.Connection) > 0) {
//                _connection = channel.CreateConnection(uri, RpcConnectionMode.Simplex);
//            }

//            // var intf = RpcClientInterfaceFactory<IRpcDetectorService>.GetOne();
//            //RpcClientProxy proxy = new RpcClientProxy(this, intf, null);
//            //proxy.BeginInvoke(
//            //    "GetProtocolInfo",
//            //    delegate (RpcClientContext ctx) {
//            //    //    try {
//            //    //        _protocolInfo = ctx.EndInvoke<RpcProtocolInfo>();
//            //    //        _serviceRole = _protocolInfo.ServiceName;
//            //    //        _maxBodySize = _protocolInfo.MaxBodySize;

//            //    //        if (_protocolInfo.AutoBatch)
//            //    //            InitBatchClient();
//            //    //    } catch (RpcException rex) {
//            //    //        if (rex.RpcCode == RpcErrorCode.ServiceNotFound) {
//            //    //            _protocolInfo = null;
//            //    //            _autoBatch = false;
//            //    //            _maxBodySize = channel.MaxBodySize;
//            //    //        }
//            //    //    } catch (Exception ex) {
//            //    //        SystemLog.Error(LogEventID.RpcFailed, ex, "RpcProxy Detect Failed");
//            //    //    }
//            //    }
//            //);
//        }

//        public RpcClientTransaction CreateTransaction(RpcRequest request)
//        {
//            RpcClientTransaction trans;
//            if (_connection != null) {
//                trans = _connection.CreateTransaction(request);
//            } else {
//                trans = _channel.CreateTransaction(ServerUri, request);
//            }
//            trans.ServiceRole = TargetService;
//            return trans;
//        }

//        private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcSimplexClient));
//    }
//}
