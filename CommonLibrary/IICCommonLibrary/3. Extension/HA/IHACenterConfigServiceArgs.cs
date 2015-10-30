using System;
using System.Collections.Generic;
using Google.ProtoBuf;
using Imps.Services.CommonV4;
using Imps.Services.CommonV4.Configuration;

namespace Imps.Services.HA
{
    [ProtoContract]
    public class HAGetConfigArgs
    {
        [ProtoMember(1)]
        public string ServiceName;

        [ProtoMember(2)]
        public string ComputerName;

        [ProtoMember(3)]
        public string Path;

        [ProtoMember(4)]
        public string Key;
    }

    [ProtoContract]
    public class HAGetConfigVersionArgs
    {
        [ProtoMember(1)]
        public string ServiceName;

        [ProtoMember(2)]
        public string ComputerName;

        [ProtoMember(3)]
        public IICConfigType ConfigType;

        [ProtoMember(4)]
        public string ConfigPath;

        [ProtoMember(5)]
        public string ConfigKey;
    }

    [ProtoContract]
    public class HAGetTableVersionArgs
    {
        [ProtoMember(1)]
        public string ServiceName;

        [ProtoMember(2)]
        public string ComputerName;

        [ProtoMember(3)]
        public RpcList<string> TableNames;
    }

    [ProtoContract]
    public class UpdateSoftBalanceTableArgs
    {
        [ProtoMember(1)]
        public string PolicyName;

        [ProtoMember(2)]
        public string ComputerName;

        [ProtoMember(3)]
        public int Weight;
    }
}
