using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Imps.Services.CommonV4
{
    public class JsonRpcRequest
    {
/// <summary>发起方机器名</summary>
		/// <remarks>1~32字节</remarks>
		public string FromComputer;

		/// <summary>发起方服务名</summary>
		/// <remarks>1~32字节</remarks>
		public string FromService;

		/// <summary>Rpc访问目标服务</summary>
		/// <remarks>1~32字节</remarks>
		public string ServiceUri;


		/// <summary>请求数据</summary>
		/// <remarks>没有为空</remarks>
		public string ReqBody = null;		// Body Buffer


        public NameValueCollection Header;
		/// <summary>
		///		默认构造函数
		/// </summary>
		public JsonRpcRequest()
		{
		}

		/// <summary>
		///		构造函数
		/// </summary>
		/// <param name="service"></param>
		/// <param name="method"></param>
		/// <param name="contextUri"></param>
        public JsonRpcRequest(string serviceUri,string reqBody)
		{
			FromComputer = ServiceEnvironment.ComputerName;
			FromService = ServiceEnvironment.ServiceName;
            ServiceUri = serviceUri;
            ReqBody = reqBody;
		}

        public void AddHeader(string key,string value)
        {
            if(Header==null)
                Header = new NameValueCollection();

            Header.Add(key, value);
        }

        public void SetReqBody(string reqBody)
        {
            ReqBody = reqBody;
        }

        public override string ToString()
        {
            StringBuilder strB = new StringBuilder();
            if (Header != null)
            {
                foreach (KeyValuePair<string, string> it in Header)
                {
                    strB.Append(string.Format("{0}:{1}\r\n", it.Key, it.Value));

                }
            }
            return string.Format("request header is {0},\r\n request body is {1}",strB.ToString(),ReqBody);
        }
    }
}
