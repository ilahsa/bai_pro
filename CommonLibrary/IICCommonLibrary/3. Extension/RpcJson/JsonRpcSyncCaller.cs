using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Imps.Services.CommonV4
{
    public static class JsonRpcSyncCaller
    {
        /// <summary>
        /// 异步方法
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <param name="callback"></param>
        public static void BeginInvoke(string serviceUri, string module, string method, string reqBody, Action<JsonRpcResponse> callback)
        {
            JsonRpcHttpClientTransaction proxy = new JsonRpcHttpClientTransaction();
            JsonRpcRequest rpcReq = new JsonRpcRequest(serviceUri, reqBody);
            rpcReq.AddHeader("UU-REQUEST-MODULE", module);
            rpcReq.AddHeader("UU-REQUEST-ACTION", method);
            rpcReq.AddHeader("UU-AUTH-TYPE", "2");
            proxy.SendRequest(rpcReq, (rpcResponse) =>
            {
                callback(rpcResponse);
            });
        }

        public static JsonRpcResponse Invoke(string serviceUri, string module, string method, string reqBody)
        {
            JsonRpcResponse resp = null;
            JsonRpcHttpClientTransaction proxy = new JsonRpcHttpClientTransaction();


            SyncInvoker sync = new SyncInvoker();
            JsonRpcRequest rpcReq = new JsonRpcRequest(serviceUri, reqBody);
            rpcReq.AddHeader("UU-REQUEST-MODULE", module);
            rpcReq.AddHeader("UU-REQUEST-ACTION", method);
            rpcReq.AddHeader("UU-AUTH-TYPE", "2");
            proxy.SendRequest(rpcReq, (rpcResponse) =>
            {
                resp = rpcResponse;
                sync.Callback();
            });

            sync.Wait(1000 * 80);
            return resp;
        }
    }
}
