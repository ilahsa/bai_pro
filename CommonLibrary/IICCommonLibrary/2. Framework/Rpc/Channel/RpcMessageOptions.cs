using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	/// <summary>
	/// RpcMessage的各种扩展选项，Rpc用于Rpc后面的各种扩展
	/// </summary>
	[Flags]
	public enum RpcMessageOptions: int
	{	
		/// <summary>
		/// 
		/// </summary>
		None				= 0,

		/// <summary> 批量方法 </summary>
		BatchMethod			= 0x0001,

		/// <summary> 文本错误提示 </summary>
		TextError			= 0x0002,

		/// <summary> 连接绑定 </summary>
		ConnectionBinding	= 0x0004,

		// ...
	}
}
