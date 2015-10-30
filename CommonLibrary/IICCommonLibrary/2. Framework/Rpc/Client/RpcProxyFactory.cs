using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Collections.Generic;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		获取Rpc客户端代理的工厂类
	/// </summary>
	public static class RpcProxyFactory
	{
		#region Private Static Members
		private static object _syncRoot = new object();
		private static Dictionary<string, RpcClientChannel> _channels = new Dictionary<string, RpcClientChannel>();
		private static Dictionary<ServerUri, RpcConnection> _connections = new Dictionary<ServerUri, RpcConnection>();
		#endregion

		#region Public static methods
		/// <summary>
		///		注册客户端Channel
		/// </summary>
		/// <param name="channel">具体的Channel实体</param>
		/// <remarks>一般来说，客户端Channel针对每种协议注册一个便可以</remarks>
		public static void RegisterClientChannel(RpcClientChannel channel)
		{
			lock (_syncRoot) {
				if (!_channels.ContainsKey(channel.Protocol)) {
					_channels.Add(channel.Protocol, channel);
				}
			}
		}

		/// <summary>
		///		获取一个连接
		/// </summary>
		/// <param name="serverUri"></param>
		/// <param name="serviceRole"></param>
		/// <returns></returns>
		public static RpcConnection GetConnection(ServerUri serverUri, string serviceRole)
		{
			RpcClientChannel channel = GetChannel(serverUri);

            RpcConnection conn;
            lock (_syncRoot) {
                if (!_connections.TryGetValue(serverUri, out conn)) {
					conn = channel.CreateConnection(serverUri, RpcConnectionMode.Simplex);
					// conn.ServiceRole = 
					_connections.Add(serverUri, conn);
				}
			}
			return conn;
		}
		
		/// <summary>
		///		获取一个Rpc客户端代理
		/// </summary>
		/// <typeparam name="T">Rpc的声明Interface</typeparam>
		/// <param name="uri">服务器的具体网络地址，如: sipc://192.168.1.100:5700</param>
		/// <returns>Rpc客户端代理了类</returns>
		public static RpcClientProxy GetProxy<T>(string uri)
		{
			ServerUri u = ServerUri.Parse(uri);
			return GetProxyInner<T>(u, null);
		}

		/// <summary>
		///		获取一个Rpc客户端代理
		/// </summary>
		/// <typeparam name="T">Rpc的声明Interface</typeparam>
		/// <param name="uri">服务器的具体网络地址实体类，如: sipc://192.168.1.100:5700</param>
		/// <returns>Rpc客户端代理类</returns>
		public static RpcClientProxy GetProxy<T>(ServerUri uri)
		{
			return GetProxyInner<T>(uri, null);
		}

		/// <summary>
		///		通过可路由Uri，获取一个Rpc客户端代理
		/// </summary>
		/// <typeparam name="T">Rpc的声明Interface</typeparam>
		/// <param name="uri">一个可用于路由的Uri实体类，例如IdUri或GroupUri</param>
		/// <returns>Rpc客户端代理类</returns>
		public static RpcClientProxy GetProxy<T>(ResolvableUri uri)
		{
			ServerUri solved = uri.Resolve(RouteMethod.Rpc);
			return GetProxyInner<T>(solved, uri);
		}
		#endregion

		#region Private or Internal Methods
		internal static RpcClientChannel GetChannel(ServerUri serverUri)
		{
			RpcClientChannel channel;
			if (!_channels.TryGetValue(serverUri.Protocol, out channel)) {
				throw new Exception(string.Format("{0} protocol:'{1}' not found", serverUri.ToString(), serverUri.Protocol));
			}
			return channel;
		}

		private static RpcClientProxy GetProxyInner<T>(ServerUri solvedUri, ResolvableUri contextUri)
		{
			RpcClientInterface intf = RpcClientInterfaceFactory<T>.GetOne();

			if (_channels.Count == 0)
				throw new Exception("You *MUST* Register at least 1 client channel at first");

			RpcClientChannel channel;
			if (!_channels.TryGetValue(solvedUri.Protocol, out channel)) {
				throw new Exception(string.Format("{0} protocol:'{1}' not found", solvedUri.ToString(), solvedUri.Protocol));
			}

			ResolvableUri r = contextUri as ResolvableUri;
			string role = r != null ? r.Service : "";
			RpcConnection conn = GetConnection(solvedUri, role);

			RpcClientProxy proxy = new RpcClientProxy(conn, intf, contextUri);
			proxy.Timeout = channel.Timeout;
			return proxy;
		}
		#endregion
	}
}
