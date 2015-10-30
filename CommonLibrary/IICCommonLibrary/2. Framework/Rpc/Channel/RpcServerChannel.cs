using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc Server通道的抽象基类: 要实现一个Server通道的话，请继承此类
	/// </summary>
	public abstract class RpcServerChannel
	{
		#region Protected members
		/// <summary>内部同步对象</summary>
		protected object p_syncRoot;

		/// <summary>通道是否启动（监听）</summary>
		protected bool p_started;

		/// <summary>协议</summary>
		protected string p_protocol;

		/// <summary>服务监听的Uri</summary>
		protected string p_serverUri;

		/// <summary>默认通道的配置</summary>
		protected RpcChannelSettings p_channelSettings;
		#endregion

		#region Public properties
		/// <summary>服务器端Url</summary>
		public string ServerUrl
		{
			get { return p_serverUri; }
		}

		/// <summary>协议</summary>
		public string Protocol
		{
			get { return p_protocol; }
		}

		/// <summary>是否启动监听</summary>
		public bool Started
		{
			get { return p_started; }
		}

		/// <summary>通道配置</summary>
		public RpcChannelSettings ChannelSettings 
		{
			get { return p_channelSettings; }
		}
		#endregion

		#region Public methods & events
		/// <summary>Server事务创建时触发</summary>
		public event Action<RpcServerTransaction> TransactionCreated = null;

		/// <summary>连接创建时触发</summary>
		public event Action<RpcConnection> ConnectionCreated = null;

		/// <summary>启动监听</summary>
		public void Start()
		{
			if (!p_started) {
				lock (p_syncRoot) {
					if (!p_started) {
						DoStart();
						p_started = true;
					}
				}
			}
		}

		/// <summary>中止监听</summary>
		public void Stop()
		{
			if (p_started) {
				lock (p_syncRoot) {
					if (p_started) {
						DoStop();
						p_started = false;
					}
				}
			}
		}
		#endregion

		#region Protected methods
		/// <summary>
		///		开始监听的具体实现
		/// </summary>
		protected abstract void DoStart();

		/// <summary>
		///		关闭监听的具体实现
		/// </summary>
		protected abstract void DoStop();

		/// <summary>
		/// 基础构造函数
		/// </summary>
		/// <param name="protocol">协议: 如"http"."tcp"</param>
		/// <param name="serverUri">监听的url，如tcp://192.168.1.100:8000/</param>
		protected RpcServerChannel(string protocol, string serverUri)
		{
			p_started = false;
			p_protocol = protocol;
			p_channelSettings = null;
			p_serverUri = serverUri;
			p_syncRoot = new object();
		}

		/// <summary>
		///		安全触发Transaction创建事件
		/// </summary>
		/// <param name="trans"></param>
		protected void OnTransactionCreated(RpcServerTransaction trans)
		{
			if (TransactionCreated != null) {
				TransactionCreated(trans);
			}
		}

		/// <summary>
		///		安全触发Connection创建事件
		/// </summary>
		/// <param name="conn"></param>
		protected void OnConnectionCreated(RpcConnection conn)
		{
			if (ConnectionCreated != null) {
				ConnectionCreated(conn);
			}
		}
		#endregion
	}
}
