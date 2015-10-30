using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		所有Rpc服务的基类
	/// </summary>
	public abstract class RpcServiceBase
	{
		protected string p_serviceName;

        public RpcServiceBase(string serviceName)
        {
            p_serviceName = serviceName;
        }

		public string ServiceName
		{
			get { return p_serviceName; }
		}

		public abstract void OnTransactionStart(RpcServerContext context);
	}
}
