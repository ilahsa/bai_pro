//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4
//{
//    [Obsolete("用Channel方式代替了", true)]
//    public class RpcInprocService 
//    {
//        RpcServiceBase _serviceBase;

//        public string ServiceName
//        {
//            get { return _serviceBase.ServiceName; }
//        }

//        public static RpcInprocService Create<T>(T service)
//        {
//            RpcServiceDecorator<T> s = new RpcServiceDecorator<T>(service);
//            return new RpcInprocService(s);
//        }

//        public RpcInprocService (RpcServiceBase service)
//        {
//            _serviceBase = service;			
//        }

//        public void BeginInvoke<TArgs>(RpcClientProxy proxy, string methodName, Action<RpcClientContext> callback, TArgs args)
//        {
//            RpcInprocTransaction inprocTrans = new RpcInprocTransaction(proxy, methodName, callback, _serviceBase);
//            inprocTrans.ClientContext.SendRequest<TArgs>(args);
//        }
//    }
//}
