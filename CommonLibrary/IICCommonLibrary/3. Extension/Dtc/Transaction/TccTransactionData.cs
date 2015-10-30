using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Services.CommonV4.Dtc
{
	[ProtoContract]
	public class TccTransactionData
	{
		[ProtoMember(1)]
		public string TxId;

		[ProtoMember(2)]
		public string ServiceAtComputer;	// "UMS@P01-LCS-01"

		[ProtoMember(3)]
		public string SchemaName;		// TransactionName

        [ProtoMember(4)]
        public DateTime BeginTime;
		
		[ProtoMember(5)]
		public TccTransactionState TxState;

		[ProtoMember(6)]
		public TccWorkState[] WorkStates;

		[ProtoMember(7)]
		public string ContextKey;

		[ProtoMember(8)]
		public byte[] ContextData;

		[ProtoMember(9)]
		public string Error;

	}
}
