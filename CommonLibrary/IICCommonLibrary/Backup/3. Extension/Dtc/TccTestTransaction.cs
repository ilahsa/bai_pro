using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Dtc;

namespace UnitTest.Dtc
{
	class TccTestTransaction: TccTransaction<TccTestContext>
	{
		public TccTestTransaction(TccTestContext context, TccCoordinator<TccTestContext> coordinator)
			: base(context, coordinator)
		{
		}

		protected override void Prepare()
		{
			//AddWorkUnit(new TccTestRpcWork("RpcWork1"));
			AddWorkUnit(new TccWorkUnitSample("Work2"));
			AddWorkUnit(new TccWorkUnitSample("Work3"), 0);
			AddWorkUnit(new TccWorkUnitSample("Work4"), 0);
			AddWorkUnit(new TccWorkUnitSample("Work5"), 1, 2);
			// AddWorkUnit(new TccTestRpcWork("RpcWork4"), 1, 2);
		}
	}
}
