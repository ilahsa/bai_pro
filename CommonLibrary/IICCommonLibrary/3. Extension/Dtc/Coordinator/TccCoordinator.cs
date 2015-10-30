using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
    //public enum TccPersisterLevel
    //{
    //    None,
    //    Cancelled,
    //    Failed,
    //    All,
    //}

	public sealed class TccCoordinator<TContext> where TContext: ITccContext
	{
		private object _syncRoot = new object();
		private Thread _monitorThread;
		private ITracing _tracing;
		private TccPersister _persister = null;
		private Dictionary<string, TccTransaction<TContext>> _activeTrans;
		private TccTransactionPerfCounter _counter;
        private string _transName;

        public string TransName
        {
            get { return _transName; }
        }

		public TccCoordinator(string transName)
		{
			_transName = transName;
			_activeTrans = new Dictionary<string, TccTransaction<TContext>>();
			_tracing = TracingManager.GetTracing("TccTransaction." + _transName);
			_counter = IICPerformanceCounterFactory.GetCounters<TccTransactionPerfCounter>(transName);

			_monitorThread = new Thread(MonitorProc);
			_monitorThread.IsBackground = false;
			_monitorThread.Name = "TccCoordinator.MonitorProc<" + _transName + ">";
		}

		internal ITracing Tracing
		{
			get { return _tracing; }
		}

		public void Initialize()
		{
		}

		public TccPersister Persister
		{
			get { return _persister; }
			set { _persister = value; }
		}

		public IEnumerable<TccTransaction<TContext>> GetActiveTransactions()
		{
			lock (_syncRoot) {
				foreach (var a in _activeTrans) {
					yield return a.Value;
				}
			}
		}

        public IEnumerable<TccTransactionData> GetActiveTransactionDatas()
        {
            lock (_syncRoot)
            {
                foreach (var a in _activeTrans)
                {
                    yield return a.Value.GetData();
                }
            }
        }


		//
		// 从数据库里面读取FailedTransaction, 然后尝试Close: Confirm, Cancel
		// 如果两个Transaction
        public IEnumerable<TccTransactionData> GetFailedTransactions()
		{
            //lock (_syncRoot) {
            //    foreach (var a in _activeTrans) {
            //        yield return a.Value;
            //    }
            //}
            if (_persister != null)
            {
                IList<TccTransactionData> dataList = _persister.LoadFailedTransaction();
                return dataList;
            }
            else
                return null;
		}

		internal void NewTransaction(TccTransaction<TContext> trans)
		{
			_counter.ActiveTransaction.Increment();
			_counter.TransOpenPerSec.Increment();
			
			lock (_syncRoot) {
				_activeTrans.Add(trans.TxId, trans);
			}


            if (_persister != null)
            {
                try
                {
                    _persister.NewTransaction(trans.GetData());
                }
                catch (Exception ex)
                {
                    _tracing.ErrorFmt(ex, "Tcc Persister Failed");
                }
            }
		}

		internal void UpdateTransaction(TccTransaction<TContext> trans)
		{
			TccTransaction<TContext> tx;
			lock (_syncRoot) {
			    if (!_activeTrans.TryGetValue(trans.TxId, out tx)) {
					// 报错
                    throw new Exception("Tansaction not in ActiveTrans");
			    }
			}

            if (_persister != null)
            {
                try
                {
                    _persister.UpdateTransaction(trans.GetData());
                }
                catch (Exception ex)
                {
                    _tracing.ErrorFmt(ex, "Tcc Persister Failed");
                }
            }
		}

		internal void CloseTransaction(TccTransaction<TContext> trans)
		{
			_counter.ActiveTransaction.Decrement();
			lock (_syncRoot) {
				_activeTrans.Remove(trans.TxId);
			}
            if (_persister != null)
            {
                try
                {
                    _persister.CloseTransaction(trans.GetData());
                }
                catch (Exception ex)
                {
                    _tracing.ErrorFmt(ex, "Tcc Persister Failed");
                }
            }
		}

		private void MonitorProc()
		{
			while (true) {
				try {
					Thread.Sleep(1000);
					//
					// TODO：处理Transaction Timeout
                    foreach (TccTransaction<TContext> trans in _activeTrans.Values)
                    {
                        //60秒
                        if (DateTime.Now - trans.BeginTime > new TimeSpan(0,0,60))
                        {
                            trans.CloseOnTimeout();
                        }
                    }
				} catch (ThreadAbortException) {
					Thread.ResetAbort();
					return;
				} catch (Exception ex) {
					string msg = string.Format("TccCoordinator<{0}>.MonitorProc Failed", _transName);
					SystemLog.Error(LogEventID.CommonFailed, ex, msg);
				}
			}
		}
	}
}
