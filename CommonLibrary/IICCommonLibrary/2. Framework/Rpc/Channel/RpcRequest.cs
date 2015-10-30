using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	///		Rpc请求实体类, 直接使用Transaction模式进行处理时可以直接使用
	/// </summary>
	public class RpcRequest
	{
		/// <summary>发起方机器名</summary>
		/// <remarks>1~32字节</remarks>
		public string FromComputer;

		/// <summary>发起方服务名</summary>
		/// <remarks>1~32字节</remarks>
		public string FromService;

		/// <summary>Rpc访问目标服务</summary>
		/// <remarks>1~32字节</remarks>
		public string Service;

		/// <summary>Rpc访问目标方法</summary>
		/// <remarks>1~32字节</remarks>
		public string Method;

		/// <summary>Rpc上下文Uri</summary>
		/// <remarks>0~256字节, 可以为空</remarks>
		/// <see cref="Imps.Services.CommonV4.ResolvableUri"/>
		public string ContextUri;

		/// <summary>Rpc消息选项</summary>
		/// <remarks>4字节整形</remarks>
		public RpcMessageOptions Options = RpcMessageOptions.None;

		/// <summary>请求数据</summary>
		/// <remarks>没有为空</remarks>
		public RpcBodyBuffer BodyBuffer = null;		// Body Buffer

		/// <summary>
		///		默认构造函数
		/// </summary>
		public RpcRequest()
		{
		}

		/// <summary>
		///		构造函数
		/// </summary>
		/// <param name="service"></param>
		/// <param name="method"></param>
		/// <param name="contextUri"></param>
		public RpcRequest(string service, string method, ResolvableUri contextUri)
		{
			FromComputer = ServiceEnvironment.ComputerName;
			FromService = ServiceEnvironment.ServiceName;
			Service = service;
			Method = method;
			ContextUri = ObjectHelper.ToString(contextUri);
		}

		public void SetBody<T>(T value)
		{
			BodyBuffer = new RpcBodyBuffer<T>(value);
		}

		public void SetBodyStream(Stream stream, int streamLen)
		{
			BodyBuffer = new RpcBodyBuffer(stream, streamLen);
		}

		public void SetBodyNull()
		{
			BodyBuffer = null;
		}

		public object BodyValue
		{
			get
			{ 
				if (BodyBuffer == null)
					return null;
				else
					return BodyBuffer.Value;
			}
		}

		public int WriteToStream(Stream stream)
		{
			return BodyBuffer.WriteToStream(stream);
		}

		public string ServiceAtComputer
		{
			get
			{
				return FromService + '@' + FromComputer;
			}
			set
			{
				if (!SplitTwo(value, '@', out FromService, out FromComputer))
					throw new FormatException("Unexcepted Service@Computer: " + value);
			}
		}

		public string ServiceDotMethod
		{
			get
			{
				return Service + '.' + Method;
			}
			set
			{
				if (!SplitTwo(value, '.', out Service, out Method))
					throw new FormatException("Unexcepted Service@Computer: " + value);
			}
		}

		public static bool SplitTwo(string str, char sperator, out string left, out string right)
		{
			int l = str.IndexOf(sperator);
			if (l < 0) {
				left = string.Empty;
				right = string.Empty;
				return false;
			}

			left = str.Substring(0, l);
			right = str.Substring(l + 1);
			return true;
		}
	}
}
