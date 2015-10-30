using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[IICConfigSection("ServiceSettings", IsRequired = false)]
	public class ServiceSettingsConfigSection: IICConfigSection
	{
		[IICConfigField("ServiceName", DefaultValue = "")]
		public string ServiceName;

		[IICConfigField("Domain", DefaultValue = "")]
		public string Domain;

		[IICConfigField("PoolID", DefaultValue = "")]
		public string PoolID;

		[IICConfigField("Pools", DefaultValue = "")]
		public string Pools;

		[IICConfigField("Site", DefaultValue = "")]
		public string Site;

		[IICConfigField("ServerAddress", DefaultValue = "0.0.0.0")]
		public string ServerAddress;

		[IICConfigField("Debug", DefaultValue = false)]
		public bool Debug;

		[IICConfigField("SipcServerPort", DefaultValue = 0)]
		public int SipcServerPort;

		[IICConfigField("HttpServerPort", DefaultValue = 0)]
		public int HttpServerPort;

		[IICConfigField("HttpServicePrefix", DefaultValue = "")]
		public string HttpServicePrefix;

		[IICConfigField("RpcServerPort", DefaultValue = 0)]
		public int RpcServerPort;

		[IICConfigField("RemotingServerPort", DefaultValue = 0)]
		public int RemotingServerPort;

        [IICConfigField("RpcOverTcpServerPort", DefaultValue = 0)]
        public int RpcOverTcpServerPort;

		[IICConfigField("MaxWorkerThread", DefaultValue= -1)]
		public int MaxWorkerThread;

		[IICConfigField("MaxWorkerThread", DefaultValue = -1)]
		public int MinWorkerThread;

		[IICConfigField("WorkerThreadTimeout", DefaultValue = -1)]
		public int WorkerThreadTimeout;
	}
}
