using System;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Imps.Services.HA;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		服务运行模式
	/// </summary>
	public enum ServiceRunMode
	{
		/// <summary>默认, 未赋值</summary>
		Unknown,

		/// <summary>Windows Service, 使用LocalConfigurator, 本地app.config</summary>
		LocalService,

		/// <summary>IIS Web App, 用LocalConfigurator, 本机app.config</summary>
		LocalWeb,

		/// <summary>Windows Service, 使用HAConfigurator, 全局配置, 集中管理</summary>
		HAService,

		/// <summary>IIS Web Application, 使用web.config, 使用appSettings["CenterUrl"], 访问HACente</summary>
		HAWeb,				

		//
		// NextVersion
		//LocalWinform,
		//LocalConsole,		// 使用LocalConfigurator, 使用app.config, Console启动
		//HAConsole,		// 使用HAConfigurator, 使用命令行参数通过Console进入
		//HATask,			// HACenter管理的TASK程序, 可以动态下载的插件
	}

	/// <summary>
	///		服务配置代理类
	/// </summary>
	public sealed class ServiceSettingsConfigProxy
	{
		#region Private fields
		private ServiceRunMode _runMode;
		private ServiceSettingsConfigSection _section = null;

		private string _serviceRoleName = string.Empty;
		private string _domain = string.Empty;
		private string _site = string.Empty;
		private int _poolId = 0;
		private List<int> _pools = null;
		#endregion

		#region Constructor & Public methods
		public ServiceSettingsConfigProxy(string serviceName)
			: this(serviceName, Environment.MachineName)
		{
		}

		public ServiceSettingsConfigProxy(string serviceName, string computerName)
		{
			_runMode = ServiceRunMode.Unknown;
			ServiceEnvironment.ServiceName = serviceName;
			ServiceEnvironment.ComputerNameForConfiguration = computerName;

			_section = IICConfigSection.CreateDefault<ServiceSettingsConfigSection>();
			_section.ServiceName = serviceName;
		}

		public void UpdateConfig(ServiceRunMode runMode, HAServiceSettings serviceSettings)
		{
			//
			// 不允许更新
			_runMode = runMode;
			_section = IICConfigurationManager.Configurator.GetConfigSecion<ServiceSettingsConfigSection>(
				"ServiceSettings", null);

			//
			// 优先使用本地配置
			_domain = _section.Domain;
			_site = _section.Site;
			int.TryParse(_section.PoolID, out _poolId);

			_pools = new List<int>();
			foreach (string a in _section.Pools.Split(',')) {
				int p;
				if (int.TryParse(a, out p)) {
					_pools.Add(p);
				}
			}

			//
			// HA模式下, 如果ServiceSettings没有配置, 会使用来自HA_Computer与HA_Deployment的配置覆盖
			// Pool, Site, Domain三项配置
			switch (_runMode) {
				case ServiceRunMode.LocalWeb:
				case ServiceRunMode.LocalService:
					if (string.IsNullOrEmpty(_serviceRoleName)) {
						_serviceRoleName = _section.ServiceName;
					}
					break;
				case ServiceRunMode.HAService:
				case ServiceRunMode.HAWeb:
					//
					// 在HA方式下运行的服务, 可以通过ServiceSettings配置节, 替换ServiceSettings的配置
					if (serviceSettings != null) {
						_serviceRoleName = serviceSettings.ServiceOriginName;

						if (string.IsNullOrEmpty(_domain)) {
							_domain = serviceSettings.Domain;
						}

						if (string.IsNullOrEmpty(_site)) {
							_site = serviceSettings.Site;
						}

						if (string.IsNullOrEmpty(_section.PoolID)) {
							_poolId = serviceSettings.PoolId;
						}
					}
					break;
				default:
					throw new NotSupportedException("Unexcepted RunMode:" + _runMode);
			}

		}
		#endregion

		#region Public Propertiess
		/// <summary>运行方式</summary>
		public ServiceRunMode RunMode
		{
			get { return _runMode; }
		}

		/// <summary>服务器机器名</summary>
		public string ComputerName
		{
			get { return ServiceEnvironment.ComputerName; }
		}

		/// <summary>服务名称</summary>
		public string ServiceName
		{
			get { return ServiceEnvironment.ServiceName; }
		}

		/// <summary>服务角色信息</summary>
		/// <remarks>如果服务的部署名可以喝服务本身角色名不一样比如: 部署名:IBSV4, 角色名, IBS</remarks>
		public string ServiceRoleName
		{
			get { return _serviceRoleName; }
		}

		/// <summary>服务所在域, 现在没用</summary>
		public string Domain
		{
			get { return _domain; }
		}

		/// <summary>服务所在PoolId, 针对分Pool服务</summary>
		public int PoolID
		{
			get { return _poolId; }
		}

		/// <summary>服务所在Site</summary>
		public string SiteName
		{
			get { return _site; }
		}

		/// <summary>是否Debug模式</summary>
		public bool Debug
		{
			get { return _section.Debug; }
		}

		/// <summary>Sipc服务开放的端口</summary>
		public int SipcServerPort
		{
			get { return _section.SipcServerPort; }
		}

		/// <summary>Http服务开放的端口</summary>
		public int HttpServerPort
		{
			get { return _section.HttpServerPort; }
		}

		/// <summary>Http服务的前缀, 一般会用HttpListener</summary>
		public string HttpServicePrefix
		{
			get { return _section.HttpServicePrefix; }
		}

		/// <summary>Rpc服务开放的端口</summary>
		public int RpcServerPort
		{
			get { return _section.RpcServerPort; }
		}

		/// <summary>Remoting服务开放端口</summary>
		public int RemotingServerPort
		{
			get { return _section.RemotingServerPort; }
		}

        /// <summary>RpcOverTcp服务开放的端口</summary>
        public int RpcOverTcpServerPort
        {
            get { return _section.RpcOverTcpServerPort; }
        }

		/// <summary>最大工作线程数</summary>
		public int MaxWorkerThread
		{
			get { return _section.MaxWorkerThread; }
		}

		/// <summary>最小工作线程数</summary>
		public int MinWorkerThread
		{
			get { return _section.MinWorkerThread; }
		}

		/// <summary>工作线程超期时间</summary>
		public int WorkerThreadTimeout
		{
			get { return _section.WorkerThreadTimeout; }
		}

		/// <summary>服务器实际地址</summary>
		public string ServerAddress
		{
			get { return _section.ServerAddress; }
		}

		/// <summary>服务管理的Pool列表</summary>
		public List<int> Pools
		{
			get { return _pools; }
		}
		#endregion

		#region Obsolete
		[Obsolete("请用ServiceEnvironment.ProcessInfo代替", true)]
		public string ProcessInfo
		{
			get { throw new NotSupportedException(); }
		}

		[Obsolete("请用ServiceEnvironment.WorkPath代替", true)]
		public string BasePath
		{
			get { throw new NotSupportedException(); }
		}
		#endregion
	}
}
