﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

/*
required int32 FromId = 1;          // 交换后使用缩写的from代替service@computer

required int32 ServiceId = 2;       // 交换后用id代替service.method

required int32 Sequence = 3;		// 事务序号, 客户端生成, 活动事务唯一，可复用

requided int32 BodyLength = 4;		// 0代表传空，1代表全默认，len-1为实际长度

optional int32 Option = 5;          // 事务可选项

optional string ContextUri = 6;

optional string FromComputer = 7;

optional string FromService = 8;               

optional string Service = 9;

optional string Method = 10;
 */
// Generated from: rpc.proto
using Google.ProtoBuf;
namespace Imps.Services.CommonV4.Rpc.ProtoContract
{
  [ProtoContract]
  public partial class RpcRequestHeader : IExtensible
  {
	[ProtoMember(1, IsRequired = true)]
	public int FromId;

	[ProtoMember(2, IsRequired = true)]
	public int ServiceId;

	[ProtoMember(3, IsRequired = true)]
	public int Sequence;

    [ProtoMember(4, IsRequired = true)]
    public int BodyLength;

    [ProtoMember(5)]
    public int Option;

	[ProtoMember(6)]
    public string ContextUri;

    [ProtoMember(7)]
    public string FromComputer;

    [ProtoMember(8)]
    public string FromService;

    [ProtoMember(9)]
    public string Service;

    [ProtoMember(10)]
    public string Method;

    private IExtension extensionObject;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
      { return Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [ProtoContract]
  public partial class RpcResponseHeader : IExtensible
  {
    public RpcResponseHeader() {}

	[ProtoMember(1, IsRequired = true)]
    public int Sequence;

	[ProtoMember(2, IsRequired = true)]
    public int ResponseCode;

    [ProtoMember(3, IsRequired = true)]
    public int BodyLength;

    [ProtoMember(4, IsRequired = false)]
    public int Option;

    [ProtoMember(5)]
    public int ServiceId;

	[ProtoMember(6)]
    public int FromId;

    private IExtension extensionObject;
    IExtension IExtensible.GetExtensionObject(bool createIfMissing)
      { return Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
}