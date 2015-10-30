using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public abstract class TccTransaction<TContext> where TContext: ITccContext
	{
		#region Private Fields
		private object _syncRoot = new object();

		private string _txId;
		private ITracing _tracing;
		private TContext _context;
		private TccCoordinator<TContext> _coordinator;

		private DateTime _beginTime = DateTime.Now;
		private TccTransactionState _txState;
		private TccTransactionContext<TContext> _txCtx;

		private TccWorkUnit<TContext> _lock;
		private List<TccWorkUnit<TContext>> _works;
		#endregion

		#region Public Property
		public TContext Context
		{
			get { return _context; }
		}

        public string TxId
		{
			get { return _txId; }
		}

		public DateTime BeginTime
		{
			get { return _beginTime; }
		}

		public TccTransactionState State
		{
			get { return _txState; }
		}

		public IEnumerable<TccWorkUnit<TContext>> Works
		{
			get
			{
				foreach (var u in _works) {
					yield return u;
				}
			}
		}
		#endregion

		#region Constructor
		public TccTransaction(TContext ctx): this(ctx, null)
		{
		}

		public TccTransaction(TContext ctx, TccCoordinator<TContext> coordinator)
		{
			_context = ctx;
			_txId = Guid.NewGuid().ToString();
			_txState = TccTransactionState.New;

			_lock = null;
			_works = new List<TccWorkUnit<TContext>>();
			_txCtx = null;

			Prepare();
			_coordinator = coordinator;
			if (_coordinator != null) {
				_coordinator.NewTransaction(this);
				_tracing = _coordinator.Tracing;
			} else {
				_tracing = _defaultTracing;
			}

            foreach (var a in _works)
                a.Tracing = _tracing;
		}

		public TccTransaction(TccTransactionData data, TccCoordinator<TContext> coordinator,TContext ctx)
		{
            // TODO: 从TransactionData中恢复数据            
            _context = ctx.Deserialize<TContext>(data.ContextData);
            _txId = data.TxId;
            _txState = data.TxState;

			_lock = null;
			_works = new List<TccWorkUnit<TContext>>();
			_txCtx = null;

            _coordinator = coordinator;
            if (_coordinator != null)
            {
                _coordinator.NewTransaction(this);
                _tracing = _coordinator.Tracing;
            }
            else
            {
                _tracing = _defaultTracing;
            }

			Prepare();

            foreach (var a in _works)
                a.Tracing = _tracing;
		}
		#endregion

		#region Abstract and Virtual Method
		protected abstract void Prepare();
		#endregion

		#region Protected Method for Prepare 
		protected void AddLockUnit(TccWorkUnit<TContext> work)
		{
            work.Tracing = _tracing;
			_lock = work;
		}

		protected int AddWorkUnit(TccWorkUnit<TContext> work)
		{
            work.Tracing = _tracing;
			_works.Add(work);
			return _works.Count - 1;
		}

		protected int AddWorkUnit(TccWorkUnit<TContext> work, params int[] prevs)
		{
            work.Tracing = _tracing;
			TccWorkUnit<TContext>[] prevWorks = new TccWorkUnit<TContext>[prevs.Length];

			int n = 0;
			foreach (var i in prevs) {
				if (i < 0 || i >= _works.Count)
					throw new InvalidDataException("Work Dependency Range Failed:" + i);

				prevWorks[n++] = _works[i];
			}

			_works.Add(work);
			work.PrevWorks = prevWorks;
			return _works.Count - 1;
		}
		#endregion

		#region Public Methods
		public void BeginExecute(Action<TccTransactionContext<TContext>> callback)
		{
			lock (_syncRoot) {
				if (_txCtx != null) {
					throw new NotSupportedException("[BUG] BeginInvoke for every TccTransaction can only run once");
				}

				_txCtx = new TccTransactionContext<TContext>(this, callback);
				_txState = _lock != null ? TccTransactionState.LockTrying : TccTransactionState.Trying;
				_beginTime = DateTime.Now;
			}

			try {
				CheckAndRunNext(null);
			} catch (Exception ex) {
				_txCtx.ThrowException("[BUG] BeginExecute CheckAndRunNext Failed", ex);
			}
		}

		public void BeginReclose(Action<TccTransactionContext<TContext>> callback)
		{
			//
			// TODO：尝试重新关闭Transaction，当Confirm或者Cancel Failed
			//
			// ?: 当从Persister当中序列化出来的时候，应当怎样去操作
			//
			// Confirming & ConfirmFailed   :  继续confirm
			// Cancelling & cancelFailed    :  继续cancel
			// Trying                       :  cancel
			// 
            if (_txCtx != null)
                throw new NotSupportedException("TccTransactionContext is not Null");
            _txCtx = new TccTransactionContext<TContext>(this, callback);
            lock (_syncRoot)
            {
                if (_txState == TccTransactionState.CancelFailed || _txState == TccTransactionState.Cancelling || _txState == TccTransactionState.Trying)
                {
                    _txState = TccTransactionState.Cancelling;
                }
                else if (_txState == TccTransactionState.ConfirmFailed || _txState == TccTransactionState.Confirming)
                {
                    _txState = TccTransactionState.Confirming;
                }
                else
                {
                    throw new NotSupportedException("TccTransactionState can not Reclose:"+_txState.ToString());
                }
            }

            try
            {
                CheckAndRunNext(null);
            }
            catch (Exception ex)
            {
                _txCtx.ThrowException("[BUG] BeginClose CheckAndRunNext Failed", ex);
            }
		}

		public void Execute()
		{
			Execute(15000);
		}

		public void Execute(int timeout)
		{
			ManualResetEvent wh = new ManualResetEvent(false);
			Exception _ex = null;
			BeginExecute(
				delegate(TccTransactionContext<TContext> ctx) {
					try {
						ctx.EndInvoke();
					} catch (Exception ex) {
						_ex = ex;
					} finally {
						wh.Set();
					}
				}
			);

            if (!wh.WaitOne(timeout))
                throw new TimeoutException("Execute TimeOut Error!");
			if (_ex != null)
				throw _ex;
		}

        public void Reclose(int timeout)
        {
            // TODO: 类似Execute, 同步执行， 并等待Timeout
            ManualResetEvent wh = new ManualResetEvent(false);
            Exception _ex = null;
            BeginReclose(
                delegate(TccTransactionContext<TContext> ctx)
                {
                    try
                    {
                        ctx.EndInvoke();
                    }
                    catch (Exception ex)
                    {
                        _ex = ex;
                    }
                    finally
                    {
                        wh.Set();
                    }
                }
            );

            if(!wh.WaitOne(timeout))
                throw new TimeoutException("Reclose TimeOut Error!");
            if (_ex != null)
                throw _ex;
        }


		public void Reclose()
		{
			// TODO: 类似Execute, 同步执行， 并等待Timeout
            Reclose(15000);
		}

		public bool CanClose()
		{
			return _txState == TccTransactionState.Confirmed ||
				_txState == TccTransactionState.Cancelled;
		}

		public TccTransactionData GetData()
		{
			// TODO: 返回此次Transaction的数据
            //throw new NotImplementedException();
            TccTransactionData data = new TccTransactionData();
            data.TxId = _txId;
            data.SchemaName = _coordinator.TransName;
            data.ServiceAtComputer = ServiceEnvironment.ServiceName + "@" + ServiceEnvironment.ComputerName;
            data.BeginTime = _beginTime;
            data.TxState = _txState;
            List<TccWorkState> workStates = new List<TccWorkState>();
            //第一个状态为Lock的状态，如果没有，则默认是None
            if (_lock != null)
                workStates.Add(_lock.WorkState);
            else
                workStates.Add(TccWorkState.None);
            foreach (TccWorkUnit<TContext> work in _works)
            {
                workStates.Add(work.WorkState);
            }
            data.WorkStates = workStates.ToArray();
            data.ContextKey = _context.Key;
            data.ContextData = Context.Serialize();
            Exception ex = GetFirstError();
            if (ex != null)
                data.Error = ex.Message+ex.StackTrace;
            else
                data.Error = string.Empty;
            return data;
		}

		public void CloseOnTimeout()
		{
			// TODO: CTX返回TransactionTimeout异常
            if (_txCtx == null)
                throw new Exception("TccTransactionContext is  Null");
            _txCtx.ThrowException("Transaction TimeOut Error", null);           
		}
		#endregion

		#region Private Methods - Inner Logic
		private void CheckAndRunNext(TccRunningContext<TContext> rc)
		{	
			if (rc != null) {
				_txState = rc.NextState;
				if (_txState == TccTransactionState.Confirmed) {
					if (_coordinator != null)
						_coordinator.CloseTransaction(this);
					_txCtx.Return();
					return;
				} else if (_txState == TccTransactionState.Cancelled) {
					if (_coordinator != null)
						_coordinator.CloseTransaction(this);
					_txCtx.ThrowException("Transaction Cancelled", GetFirstError());
					return;
				} else if (_txState == TccTransactionState.ConfirmFailed || _txState == TccTransactionState.CancelFailed) {
					if (_coordinator != null)
                        _coordinator.CloseTransaction(this);
					_txCtx.ThrowException("Transaction Failed", GetFirstError());
					return;
				} else {
					if (_coordinator != null)
						_coordinator.UpdateTransaction(this);
				}
			}
			lock (_syncRoot) {
				rc = GetRunningContext();
			}

			_tracing.InfoFmt("NextRunningContext() = {0}", rc);
			rc.Run(
				delegate() {
					try {
						CheckAndRunNext(rc);
					} catch (Exception ex) {
						_tracing.Error(ex, "CheckAndRunNext() Failed");
						_txCtx.ThrowException("TccTransaction.CheckAndRunNext() Failed", ex);
					}
				}
			);
		}

		private TccRunningContext<TContext> GetRunningContext()
		{
			TccRunningContext<TContext> rc = new TccRunningContext<TContext>(_context, _tracing);
			TccTransactionState nextState;

			switch (_txState) {
				case TccTransactionState.LockTrying:
					rc.AddWork(_lock, TccAction.Try);
					rc.SetNextState(TccTransactionState.Trying, TccTransactionState.Cancelling);
					break;
				case TccTransactionState.Trying:
					bool hasNext = false;
					foreach (var a in _works) {
						if (a.WorkState != TccWorkState.Tryed) {
							if (a.CanTry()) {
								rc.AddWork(a, TccAction.Try);
							} else {
								hasNext = true;
							}
						}
					}
					nextState = hasNext ? TccTransactionState.Trying : TccTransactionState.Confirming;
					rc.SetNextState(nextState, TccTransactionState.Cancelling);
					break;
				case TccTransactionState.Confirming:
					foreach (var a in _works) {
						if (a.WorkState != TccWorkState.Confirmed)
							rc.AddWork(a, TccAction.Confirm);
					}
					nextState = _lock != null ? TccTransactionState.LockConfirming :TccTransactionState.Confirmed;
					rc.SetNextState(nextState, TccTransactionState.ConfirmFailed);
					break;
				case TccTransactionState.LockConfirming:
					rc.AddWork(_lock, TccAction.Confirm);
					rc.SetNextState(TccTransactionState.Confirmed, TccTransactionState.ConfirmFailed);
					break;
				case TccTransactionState.Cancelling:
					foreach (var a in _works) {
						if (a.WorkState != TccWorkState.Cancelled && a.WorkState != TccWorkState.None)
							rc.AddWork(a, TccAction.Cancel);
					}
					nextState = _lock != null ? TccTransactionState.LockCanceling :TccTransactionState.Cancelled;
					rc.SetNextState(nextState, TccTransactionState.CancelFailed);
					break;
				case TccTransactionState.LockCanceling:
					rc.AddWork(_lock, TccAction.Cancel);
					rc.SetNextState(TccTransactionState.Cancelled, TccTransactionState.CancelFailed);
					break;
			}
			return rc;
		}

		private Exception GetFirstError()
		{
			foreach (var a in _works) {
				if (a.Error != null) {
					return a.Error;
				}
			}
			return null;
		}
		#endregion
	
		#region Private Static Fields
		private static ITracing _defaultTracing = TracingManager.GetTracing(typeof(TccTransaction<TContext>));
		#endregion
	}
}
