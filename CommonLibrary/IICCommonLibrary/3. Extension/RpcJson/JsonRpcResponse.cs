using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace Imps.Services.CommonV4
{
    public class JsonRpcResponse
    {

        public JsonRpcResponse(JsonRpcErrorCode rpcErrorCode, string responseBody, JsonRpcException exception,int originalErrorCode)
        {
            _rpcErrorCode = rpcErrorCode;
            _respBody = responseBody;
            _exception = exception;
            _originalErrorCode = originalErrorCode;
        }

        public JsonRpcResponse(JsonRpcErrorCode rpcErrorCode, string responseBody):this(rpcErrorCode,responseBody,null,0)
        {

        }

        public JsonRpcResponse(JsonRpcErrorCode rpcErrorCode,JsonRpcException exception,int orginalErrorCode):this(rpcErrorCode,null,exception,orginalErrorCode)
        {

        }


        public JsonRpcResponse(JsonRpcErrorCode rpcErrorCode,int originalErrorCode):this(rpcErrorCode,null,null,originalErrorCode)
        {
            _rpcErrorCode = rpcErrorCode;
            _respBody = null;
            _exception = null;
            _originalErrorCode = originalErrorCode;
        }


        public JsonRpcResponse(JsonRpcErrorCode rpcErrorCode):this
            (rpcErrorCode, 0)
        {

        }



        private JsonRpcErrorCode _rpcErrorCode;

        public JsonRpcErrorCode RpcErrorCode
        {
            get { return _rpcErrorCode; }
        }

        private int _originalErrorCode;

        public int OriginalErrorCode
        {
            get { return _originalErrorCode; }
        }
        private string _respBody;
        private JsonRpcException _exception;

        public string RespBody
        {
            get { return _respBody; }
        }

        public JsonRpcException Exception
        {
            get { return _exception; }
        }

        public override string ToString()
        {
            return string.Format("erroycode {0},respbody {1}", _rpcErrorCode, _respBody);
        }
    }
}
