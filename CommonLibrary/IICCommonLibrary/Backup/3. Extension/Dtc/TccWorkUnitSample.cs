using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4.Dtc;
using Imps.Services.CommonV4;

namespace UnitTest.Dtc
{
    [Serializable]
	class TccTestContext:ITccContext
	{
		public int UserId = 0;
		public int Sid = 0;
        [NonSerialized]
		public string FailedWork = null;
        [NonSerialized]
		public TccAction FailedAction = TccAction.None;
        [NonSerialized]
		public string RpcTestMark = string.Empty;

        #region ITccContext Members

        public string Key
        {
            get { return UserId.ToString(); }
        }

        public byte[] Serialize()
        {

            return BinarySerializer.ToByteArray<TccTestContext>(this);

        }

        public TccTestContext Deserialize<TccTestContext>(byte[] bytes)
        {
            return BinarySerializer.FromByteArray<TccTestContext>(bytes);
        }

        #endregion
    }

	class TccWorkUnitSample: TccWorkUnit<TccTestContext>
	{
		public TccWorkUnitSample(string unitName)
			: base(unitName)
		{
		}

		protected override void Try(TccWorkUnitContext<TccTestContext> ctx)
		{
			if (ctx.Value.FailedWork == this.WorkName && ctx.Value.FailedAction == TccAction.Try) {
				throw new Exception("Try Failed");
			}
			ctx.Return();
		}

		protected override void Confirm(TccWorkUnitContext<TccTestContext> ctx)
		{
			if (ctx.Value.FailedWork == this.WorkName && ctx.Value.FailedAction == TccAction.Confirm) {
				throw new Exception("Confirm Failed");
			}
			ctx.Return();
		}

		protected override void Cancel(TccWorkUnitContext<TccTestContext> ctx)
		{
			if (ctx.Value.FailedWork == this.WorkName && ctx.Value.FailedAction == TccAction.Cancel) {
				throw new Exception("Cancel Failed");
			}
			ctx.Return();
		}
	}
}
