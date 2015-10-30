using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcBatchClientTransaction: RpcClientTransaction
	{
		private Action _callback;
		private RpcClientBatchManager _manager;
        private int _requestIndex;

        public int RequestIndex
        {
            get { return _requestIndex; }
        }

		public RpcBatchClientTransaction(ServerUri uri, RpcRequest request, RpcClientBatchManager manager)
			: base(uri, request)
		{	
			_manager = manager;
		}

        /// <summary>
        /// SGW专用，其他慎用
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="request"></param>
        /// <param name="manager"></param>
        public RpcBatchClientTransaction(ServerUri uri, RpcBatchRequest request, RpcClientBatchManager manager)
            : base(uri, null)
        {
            _manager = manager;
            _batchRequest = request;
        }

		public override void SendRequest(Action callback, int timeout)
		{
			_callback = callback;
			_manager.EnqueueTransaction(this);
		}
       /// <summary>
       /// SGW使用，其他慎用
       /// </summary>
       /// <param name="callback"></param>
       /// <param name="timeout"></param>
       /// <param name="requestIndex"></param>
        public void SendRequest(Action callback, int timeout,int requestIndex)
        {
            _requestIndex = requestIndex;
            SendRequest(callback, timeout);
        }

		public void OnTransactionEnd()
		{
			_callback();
		}

		
        private RpcBatchRequest _batchRequest = null;
        public RpcBatchRequest BatchRequest
        {
            get
            {
                if (_batchRequest == null)
                {
                    return new RpcBatchRequest()
                    {
                        ContextUri = Request.ContextUri,
                        HasBody = Request.BodyBuffer != null,
                        RequestData = Request.BodyBuffer != null ? Request.BodyBuffer.GetByteArray() : null
                    };
                }
                else
                    return _batchRequest;
            }
        }

        private RpcBatchResponse _rpcBatchResponse;
        public RpcBatchResponse BatchResponse
        {
            set
            {
                _rpcBatchResponse = value;
                if (value.ErrorCode == RpcErrorCode.OK)
                {
                    Response = RpcResponse.Create(value.ResponseData);
                }
                else
                {
                    string error = Encoding.UTF8.GetString(value.ResponseData);
                    Response = RpcResponse.Create(value.ErrorCode, new Exception(error));
                }
            }
            get
            {
                return _rpcBatchResponse;
            }
        }
	}
}
