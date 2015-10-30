using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc客户端通道的抽象基类
	/// </summary>
	public abstract class RpcClientChannel
	{
		#region Protected
		/// <summary>协议: 如"http"</summary>
		protected string p_protocol;

		/// <summary>通道设置</summary>
		protected RpcChannelSettings p_channelSettings = null;
		
		/// <summary>构造函数</summary>
		/// <param name="protocol">协议</param>
		protected RpcClientChannel(string protocol)
		{
			p_protocol = protocol;
		}
		#endregion

		#region Public 
		/// <summary>协议</summary>
		public string Protocol 
		{
			get { return p_protocol; }
		}

		/// <summary>通道超时: ms毫秒</summary>
		public int Timeout
		{
			get { return p_channelSettings.Timeout; }
			set { p_channelSettings.Timeout = value; }
		}

		/// <summary>ChannelSettings 的默认配置，在没有检查到DetectorService时使用该配置</summary>
		public RpcChannelSettings DefaultSettings 
		{
			get { return p_channelSettings; }
		}

		///// <summary>创建Transaction</summary>
		public virtual RpcClientTransaction CreateTransaction(ServerUri serverUri, RpcRequest request)
		{
			throw new NotFiniteNumberException();
		}

		/// <summary>创建连接，单工连接或者双工连接，适用于高级的Channel</summary>
		public virtual RpcConnection CreateConnection(ServerUri serverUri, RpcConnectionMode mode)
		{
			if (mode == RpcConnectionMode.Simplex) {
				return new RpcSimplexConnection(this, serverUri);
			} else {
				throw new NotSupportedException();
			}
		}
		#endregion
	}
}