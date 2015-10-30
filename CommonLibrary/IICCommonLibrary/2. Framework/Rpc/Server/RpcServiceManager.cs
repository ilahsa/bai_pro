using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		管理所有的RpcService
	/// </summary>
	public static class RpcServiceManager
	{
		#region Private 
		private static object _syncRoot = new object();
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcServiceManager));

		private static HybridDictionary<string, RpcServiceBase> _services = new HybridDictionary<string, RpcServiceBase>();
		private static List<RpcServerChannel> _channels = new List<RpcServerChannel>();
		private static RpcServiceDispather _dispatcher = new RpcServiceDispather("");

		static RpcServiceManager()
		{
			//var detectSrv = new RpcServiceDelegate("__DETECT__", DetectProc);
			//_services.Add(detectSrv.ServiceName, detectSrv);
		}
		#endregion

		#region Public
		/// <summary>
		///		注册服务信道
		/// </summary>
		/// <param name="channel">待注册的信道</param>
		/// <remarks>同一种信道如果监听不同的端口，可以注册多个</remarks>
		public static void RegisterServerChannel(RpcServerChannel channel)
		{
			lock (_syncRoot) {
				channel.TransactionCreated += new Action<RpcServerTransaction>(TransactionCreatedCallback);
				_channels.Add(channel);
			}
		}

		/// <summary>
		///		检测RpcService是否有注册
		/// </summary>
		/// <param name="serviceName">服务名</param>
		/// <returns>是否已注册</returns>
		public static bool HasService(string serviceName)
		{
			lock (_syncRoot) {
				return _services.ContainsKey(serviceName);
			}
		}

		/// <summary>
		///		注册派生自RpcSerivceBase的原生服务
		/// </summary>
		/// <param name="service">服务对象</param>
		/// <remarks>每个名字的服务对象仅能注册一次</remarks>
		public static void RegisterRawService(RpcServiceBase service)
		{
			lock (_syncRoot) {
				_dispatcher.RegisterService(service);
			}
		}

		/// <summary>
		///		注册继承自rpc interface的服务
		/// </summary>
		/// <typeparam name="T">interface的类型</typeparam>
		/// <param name="service">服务对象</param>
		/// <remarks>每个类型的服务对象仅能注册一次</remarks>
		public static void RegisterService<T>(T service)
		{
            // 适配器
			RpcServiceDecorator<T> realService = new RpcServiceDecorator<T>(service);
			_dispatcher.RegisterService(realService);
		}

		/// <summary>
		///		注册继承自rpc interface的服务，并用ServiceName覆盖服务名
		/// </summary>
		/// <typeparam name="T">interface的类型</typeparam>
		/// <param name="serviceName">服务名></param>
		/// <param name="service">服务对象</param>
		/// <remarks>每个类型的服务对象仅能注册一次</remarks>
		public static void RegisterService<T>(string serviceName, T service)
		{
			RpcServiceDecorator<T> realService = new RpcServiceDecorator<T>(service, serviceName);
			_dispatcher.RegisterService(realService);
		}

		/// <summary>
		///		启动所有的Channels，开始监听
		/// </summary>
		public static void Start()
		{
			foreach (var channel in _channels) {
				channel.Start();
				_tracing.WarnFmt("RpcChannel<{0}> Started on {1}", channel.Protocol, channel.ServerUrl);
			}
		}

		/// <summary>
		///		停止所有的Channels
		/// </summary>
		public static void Stop()
		{
			foreach (var channel in _channels) {
				channel.Stop();
				_tracing.WarnFmt("RpcChannel<{0}> Started on {1}", channel.Protocol, channel.ServerUrl);
			}
		}
		#endregion

		private static void TransactionCreatedCallback(RpcServerTransaction tx)
		{
			if (tx.Bypassed)
				return;
			_dispatcher.ProcessTransaction(tx);
		}
	}
}
