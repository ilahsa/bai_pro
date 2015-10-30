/*
 * RpcConnection
 *	新增加的抽象基类，用于在完整能力的RpcChannel中增加以下能力
 *	
 *	- 连接管理和重用
 *	- 实现Duplex连接，可在Rpc层次上实现回调
 *	
 * GaoLei
 * 2010-06-11
 */
using System;
using System.Collections;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc的连接类型
	/// </summary>
	public enum RpcConnectionMode
	{
		/// <summary></summary>
		Unknown,

		/// <summary>单工方式，仅支持客户端到服务器的调用</summary>
		Simplex,

		/// <summary>双工方式，支持双向调用</summary>
		Duplex,
	}

	/// <summary>
	///		Rpc的连接方向
	/// </summary>
	public enum RpcConnectionDirection
	{
		/// <summary>客户端</summary>
		Client,

		/// <summary>服务器</summary>
		Server,
	}

	/// <summary>
	///		用于Rpc长连接的管理，长连接被可以是双向: Duplex的或单向: Simplex的
	/// </summary>
	public abstract class RpcConnection
	{
		#region Private fields
		//private string _remoteService;
		//private string _remoteComputer;
		private RpcConnectionMode _mode;
		private RpcConnectionDirection _direction;
		private Hashtable _contexts = Hashtable.Synchronized(new Hashtable());
		#endregion

		#region Public properties
		/// <summary>连接类型</summary>
		public RpcConnectionMode Mode
		{
			get { return _mode; }
		}
		/// <summary>是否连接上</summary>
		public abstract bool Connected 
		{
			get;
		}

		/// <summary>连接方向</summary>
		public RpcConnectionDirection Direction
		{
			get { return _direction; }
		}

		/// <summary>对端Uri</summary>
		public abstract ServerUri RemoteUri
		{
			get;
		}

		/// <summary>远程服务角色名</summary>
		public string RemoteService
		{
			get;
			set;
		}

		/// <summary>自定义连接上下文，用于保存状态</summary>
		public Hashtable Contexts
		{
			get { return _contexts; }
		}

		/// <summary>连接断开时触发，主动连接不会触发</summary>
		public event Action<RpcConnection> Disconnected;

		/// <summary>收到请求, Transaction创建时触发此事件, 服务端</summary>
		/// <remarks>一般情况, 触发此事件后, 会继续触发Channel.OnTransactionCreated, 所以本事件不要用于业务处理</remarks>
		public event Action<RpcConnection, RpcServerTransaction> TransactionCreated;
		#endregion

		#region Protected Methods
		///<summary>触发Transaction创建事件</summary>
		protected void OnTransactionCreated(RpcServerTransaction tx)
		{
			if (TransactionCreated != null) {
				TransactionCreated(this, tx);
			}
		}

		/// <summary>触发断开连接事件</summary>
		protected void OnDisconnected()
		{
			if (Disconnected != null) {
				Disconnected(this);
			}
		}
		#endregion

		#region Abstract Methods
		/// <summary>主动断开连接</summary>
		public abstract void Disconnect();

		/// <summary>主动创建连接，Client</summary>
		/// <param name="callback">如果callback无异常，表示连接上</param>
		public abstract void BeginConnect(Action<Exception> callback);

		/// <summary>创建一个RpcTransaction</summary>
		/// <param name="request">请求实体类</param>
		/// <returns>Trans</returns>
		public abstract RpcClientTransaction CreateTransaction(RpcRequest request);
		#endregion

		#region Public Methods
		/// <summary>
		///		创建一个RpcConnection
		/// </summary>
		/// <param name="mode">模式</param>
		/// <param name="direction">方向</param>
		/// <remarks>被动连接(Server)在连接创建时建立，主动连接(Client)在BeginConnect后连接</remarks>
		public RpcConnection(RpcConnectionMode mode, RpcConnectionDirection direction)
		{
			_mode = mode;
			_direction = direction;
		}

		/// <summary>
		///		同步创建连接
		/// </summary>
		/// <param name="timeout"></param>
		public void Connect(int timeout)
		{
			var a = new SyncInvoker<Exception>();
			BeginConnect(a.Callback);
			if (!a.Wait(timeout)) {
				throw new RpcException(RpcErrorCode.ConnectionTimeout, RemoteUri.ToString(), "Connection timeout", null);
			}
		}

		public RpcClientProxy GetCallbackProxy<T>()
		{
			RpcClientProxy proxy = new RpcClientProxy(this, RpcClientInterfaceFactory<T>.GetOne(), null);
			return proxy;
		}

		public RpcClientProxy GetProxy<T>()
		{
			RpcClientProxy proxy = new RpcClientProxy(this, RpcClientInterfaceFactory<T>.GetOne(), null);
			return proxy;
		}
		#endregion
	}
}

