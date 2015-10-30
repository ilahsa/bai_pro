using System;
using System.Diagnostics;

namespace Imps.Services.CommonV4.Rpc
{
	[IICPerformanceCounters("rpc:TcpChannel Transport", CategoryType = PerformanceCounterCategoryType.MultiInstance)]
	class RpcTcpTransportCounter
	{
		[IICPerformanceCounter("Connections", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter Connections = null;

		[IICPerformanceCounter("Connections Pending.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ConnectionsPending = null;

		[IICPerformanceCounter("Connections Recycled.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ConnectionsRecycled = null;

		[IICPerformanceCounter("Send /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter SendPerSec = null;

		[IICPerformanceCounter("Send Message /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter SendMessagePerSec = null;

		[IICPerformanceCounter("Send Bytes /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter SendBytesPerSec = null;

		[IICPerformanceCounter("Send Pendings.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter SendPending = null;

		[IICPerformanceCounter("Receive /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter ReceivePerSec = null;

		[IICPerformanceCounter("Receive Message /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter ReceiveMessagePerSec = null;

		[IICPerformanceCounter("Receive Bytes /sec.", PerformanceCounterType.RateOfCountsPerSecond32)]
		public IICPerformanceCounter ReceiveBytesPerSec = null;

		[IICPerformanceCounter("Total Send.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter SendTotal = null;

		[IICPerformanceCounter("Total Send Message.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter SendMessageTotal = null;

		[IICPerformanceCounter("Total Send Bytes.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter SendBytesTotal = null;

		[IICPerformanceCounter("Total Send Failed.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter SendFailed = null;

		[IICPerformanceCounter("Total Receive.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter ReceiveTotal = null;

		[IICPerformanceCounter("Total Receice Message.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter ReceiveMessageTotal = null;

		[IICPerformanceCounter("Total Receive Bytes.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter ReceiveBytesTotal = null;

		[IICPerformanceCounter("Corrupted Transmisions.", PerformanceCounterType.NumberOfItems64)]
		public IICPerformanceCounter CorruptedTransmissions = null;
	}

	[IICPerformanceCounters("rpc:TcpChannel Buffer", CategoryType = PerformanceCounterCategoryType.MultiInstance)]
	class RpcTcpBufferCounter
	{
		[IICPerformanceCounter("Receive Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter ReceiveTotal = null;

		[IICPerformanceCounter("Receiving.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter Receiving = null;

		[IICPerformanceCounter("P1 Send Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter P1SendTotal = null;

		[IICPerformanceCounter("P1 Sending.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter P1Sending = null;

		[IICPerformanceCounter("P2 Send Total.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter P2SendTotal = null;

		[IICPerformanceCounter("P2 Sending.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter P2Sending = null;

		[IICPerformanceCounter("Unpooled Sending.", PerformanceCounterType.NumberOfItems32)]
		public IICPerformanceCounter UnpooledSending = null;

		[IICPerformanceCounter("Hit Ratio.", PerformanceCounterType.RawFraction)]
		public IICPerformanceCounter HitRatio = null;
	}
}
