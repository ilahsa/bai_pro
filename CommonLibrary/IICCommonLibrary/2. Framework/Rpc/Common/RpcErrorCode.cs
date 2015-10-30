using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///		Rpc返回码
	/// </summary>
	public enum RpcErrorCode: int
	{
		/// <summary>成功</summary>
		OK						= 200,

		/// <summary>在客户端发送阶段产生错误</summary>
		SendFailed				= -1,

		/// <summary>客户端通道过于拥挤，无法发送</summary>
		SendPending				= -2,

		/// <summary>Transaction超时, 在规定时间内未收到服务器应答</summary>
		TransactionTimeout		= -3,

		/// <summary>客户端事务忒忙，无法创建</summary>
		TransactionBusy			= -4,

		/// <summary>建立连接超时，在某些连接模式的Channel中会抛出</summary>
		ConnectionTimeout		= -5,

		/// <summary>连接断开失效，在连接模式的Channel中抛出</summary>
		ConnectionBroken		= -6,

		/// <summary>连接堵塞, 当对某一台服务器创建了太多无法使用的连接时抛出</summary>
		ConnectionPending		= -7,

		/// <summary>服务未找到, 已到达Server端，由Rpc分发器返回</summary>
		ServiceNotFound			= 404,

		/// <summary>方法未找到, 已到达Server端，由Rpc分发器返回</summary>
		MethodNotFound			= 405,

		/// <summary>内部错误, Server端返回, 可能为应用抛出</summary>
		ServerError				= 500,

		/// <summary>服务器忙, 保护性错误</summary>
		ServerBusy				= 503,

		/// <summary>请求格式错误，可能为序列化错误，或内部报文格式错误</summary>
		InvaildRequestArgs		= 512,

		/// <summary>应答格式错误，可能为序列化错误，或内部报文格式错误</summary>
		InvaildResponseArgs		= 513,

		/// <summary>连接状态维护失败，由服务器返回错误</summary>
		ConnectionFailed		= 481,

		/// <summary>服务器转发，或处理超时</summary>
		/// <remarks>TransacionTimeout为客户端未收到应答，ServerTimeout由服务器明确给出超时应答，在SGW或转发类服务中出现</remarks>
		ServerTimeout			= 504,

		/// <summary>服务器转发错误</summary>
		ServerTransferFailed	= 600,

		/// <summary>未知错误</summary>
		Unknown					= 699,
	}
}
