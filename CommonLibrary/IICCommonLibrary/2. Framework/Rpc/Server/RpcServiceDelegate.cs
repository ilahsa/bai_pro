//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4
//{
//    public class RpcServiceDelegate: RpcServiceBase
//    {
//        private Action<RpcServerContext> _serviceProc;

//        public RpcServiceDelegate(string serviceName, Action<RpcServerContext> proc)
//            : base(serviceName)
//        {
//            _serviceProc = proc;
//        }

//        public override void OnTransactionStart(RpcServerContext context)
//        {
//            _serviceProc(context);
//        }
//    }
//}
