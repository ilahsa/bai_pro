//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    public class RpcDuplexCallbackNexus: IRpcClientNexus
//    {
//        private string _targetService;
//        private string _targetComputer;
//        private ServerUri _serverUri;
//        private RpcConnection _connection;

//        public RpcDuplexCallbackNexus(ServerUri serverUri, string targetService, string targetComputer, RpcConnection conn)
//        {
//            _serverUri = serverUri;
//            _targetService = targetService;
//            _targetComputer = targetComputer;
//            _connection = conn;
//        }

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

//        public RpcClientTransaction CreateTransaction(RpcRequest request)
//        {
//            return _connection.CreateTransaction(request);
//        }
//    }
//}
