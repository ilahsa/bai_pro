using System;
using System.Collections.Generic;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Rpc
{
    /// <summary>
    ///		Rpc通道支持模式
    /// </summary>
    [Flags]
    public enum RpcChannelSupportModes
    {
        /// <summary>空</summary>
        None = 0,

        /// <summary>长连接模式</summary>
        Connection = 1,

        /// <summary>双工长连接</summary>
        DuplexConnection = 2,

        /// <summary>文本错误</summary>
        TextError = 4,
    }

    /// <summary>
    ///		Rpc的通道配置
    /// </summary>
    [ProtoContract]
    public class RpcChannelSettings
    {
        /// <summary>版本号,一般不看</summary>
        [ProtoMember(1)]
        public string Version;

        /// <summary>通道支持的特性</summary>
        [ProtoMember(2)]
        public RpcChannelSupportModes SupportModes;

        /// <summary>最大的包体长度</summary>
        [ProtoMember(3)]
        public int MaxBodySize;

        /// <summary>默认的超时时间</summary>
        [ProtoMember(4)]
        public int Timeout = 120 * 1000;

        /// <summary>连接模式的保持连接事件</summary>
        [ProtoMember(5)]
        public int ConnectionKeepaliveSeconds;

        [ProtoMember(6)]
        public int ConcurrentConnection = 1;
    }
}
