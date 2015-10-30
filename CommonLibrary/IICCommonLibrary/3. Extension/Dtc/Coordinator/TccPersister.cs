using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Imps.Services.CommonV4.Dtc
{
	public enum TccPersisterMode
	{
		None = 0,		        // 不序列化
		CloseFailed,	        // 序列化关闭失败的: CancelFailed, ConfirmFailed
        AllFailed,		        // 序列化所有失败: !=Confirmed 都认为是失败
        ActiveAndAllFailed,     // 正在运行的和所有失败的
		All,		            // 所有都序列化
	}

	public abstract class TccPersister
	{
		protected string _transName;
        //public string TransName { get; set; }
        protected TccPersisterMode _mode;

		public TccPersister(string transName,TccPersisterMode mode)
		{
            _transName = transName;
            _mode = mode;
		}

		public abstract IList<TccTransactionData> LoadFailedTransaction();

		public abstract void NewTransaction(TccTransactionData data);

		public abstract void UpdateTransaction(TccTransactionData data);

		public abstract void CloseTransaction(TccTransactionData data);

	}

   
}
