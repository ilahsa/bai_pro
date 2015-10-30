//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    [Obsolete("用Channel方式代替了", true)]
//    public class RpcInprocTransaction: IRpcServerTransaction, IRpcClientTransaction
//    {
//        private Action _callback;
//        private RpcClientContext _clientContext;
//        private RpcServerContext _serverContext;
//        private object _args;
//        private object _results;
//        private string _serviceUrl;
//        private Exception _exception;
//        private RpcServiceBase _service;

//        public RpcInprocTransaction(RpcClientProxy proxy, string methodName, Action<RpcClientContext> callback, RpcServiceBase service)
//        {
//            _serviceUrl = string.Format("{0}.{1}", proxy.ServiceUrl, methodName);
//            _clientContext = new RpcClientContext(proxy, methodName, callback, this, false);
//            _serverContext = new RpcServerContext(proxy.From, proxy.To, proxy.ServiceName, methodName, proxy.ServiceUrl, this);
//            _service = service;
//        }

//        public RpcClientContext ClientContext
//        {
//            get { return _clientContext; }
//        }

//        public RpcServerContext ServerContext
//        {
//            get { return _serverContext; }
//        }

//        #region IRpcClientTransaction Members
//        public void SendRequest(Action callback)
//        {
//            _callback = callback;
//            _service.OnTransactionStart(_serverContext);
//        }

//        public void SendRequest<T>(T args, Action callback)
//        {
//            _args = args;
//            _callback = callback;
//            _service.OnTransactionStart(_serverContext);
//        }

//        public void ReceiveResponse()
//        {
//            CheckError();
//        }

//        public T ReceiveResponse<T>()
//        {
//            CheckError();
//            return (T)_results;
//        }

//        private void CheckError()
//        {
//            if (_exception != null)
//                throw _exception;
//        }

//        #endregion

//        #region IRpcServerTransaction Members

//        public RpcServerContext CreateContext()
//        {
//            // Never Call
//            throw new NotSupportedException();
//        }

//        public T ReceiveRequest<T>()
//        {
//            return (T)_args;
//        }

//        public void SendResponse()
//        {
//            _results = null;
//            _callback();
//        }

//        public void SendResponse<T>(T results)
//        {
//            _results = results;
//            _callback();
//        }

//        public void SendResponse(RpcErrorCode retCode, Exception ex)
//        {
//            _exception = new RpcException("RpcInprocTransaction.SendResponse", _serviceUrl, retCode, ex);
//            _callback();
//        }

//        #endregion
//    }
//}
