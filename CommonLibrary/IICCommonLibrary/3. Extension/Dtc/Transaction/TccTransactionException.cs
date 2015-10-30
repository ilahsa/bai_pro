using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public class TccTransactionException<TContext>: Exception where TContext: ITccContext
	{
		public TccTransaction<TContext> Transaction;

		public TccWorkUnit<TContext> FailedWork;

		public Exception Error;

		public TccTransactionException(TccTransaction<TContext> trans, string message, Exception ex)
			: base(message)
		{
			Transaction = trans;
			Error = ex;
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("TccTransaction Failed while <{0}> '{1} in works:\r\n", Transaction.State, Message);
			str.AppendFormat("Context {0}:\r\n", Transaction.Context);
			foreach (var w in Transaction.Works) {
				str.AppendFormat("\tWork[{0}]:{1} {2}", w.WorkName, w.WorkState, w.Error);
			}
			return str.ToString();
		}
	}
}
