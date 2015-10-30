using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	public abstract class TccWorkUnit<TContext>
	{
		private string _workName;
		private Exception _error;
		private TccWorkState _state = TccWorkState.None;
		private TccWorkUnit<TContext>[] _prevWorkUnits = null;
		
		public TccWorkUnit(string workName)
		{
			_workName = workName;
		}
        
		protected abstract void Try(TccWorkUnitContext<TContext> ctx);
		protected abstract void Confirm(TccWorkUnitContext<TContext> ctx);
		protected abstract void Cancel(TccWorkUnitContext<TContext> ctx);

		public string WorkName
		{
			get { return _workName; }
		}
			
		public TccWorkState WorkState
		{
			get { return _state; }
		}
		
		public Exception Error
		{
			get { return _error; }
		}

        internal ITracing Tracing = null; 

		internal TccWorkUnit<TContext>[] PrevWorks
		{
			get { return _prevWorkUnits; }
			set { _prevWorkUnits = value; }
		}

		internal void RunAction(TContext context, TccAction nextAction, Action<Exception> callback)
		{
			Action<TccWorkUnitContext<TContext>> Proc;
			TccWorkState succState;
			TccWorkState failState;

            Tracing.InfoFmt("WorkUnit<{0}>.Run{1} State:{2}", _workName, nextAction, _state);

			switch (nextAction) {
				case TccAction.Try:
					if (_state != TccWorkState.None) {
						throw new Exception("Unexcepted WorkUnit State:" + _state);
					}
					_state = TccWorkState.Trying;
					Proc = Try;
					succState = TccWorkState.Tryed;
					failState = TccWorkState.TryFailed;
					break;
				case TccAction.Confirm:
					if (_state != TccWorkState.Tryed) {
						throw new Exception("Unexcepted WorkUnit State:" + _state);
					}
					Proc = Confirm;
					_state = TccWorkState.Confirming;
					succState = TccWorkState.Confirmed;
					failState = TccWorkState.ConfirmFailed;
					break;
				case TccAction.Cancel:
					if (_state != TccWorkState.Tryed && _state != TccWorkState.TryFailed) {
						throw new Exception("Unexcepted WorkUnit State:" + _state);
					}
					Proc = Cancel;
					_state = TccWorkState.Cancelling;
					succState = TccWorkState.Cancelled;
					failState = TccWorkState.CancelFailed;
					break;
				default:
					throw new Exception("NeverGoHere");
			}

			int hasReturned = 0;
			TccWorkUnitContext<TContext> ctx = new TccWorkUnitContext<TContext>(
				context,
				delegate(Exception ex) {
                    if (ex != null)
                        Tracing.ErrorFmt(ex, "WorkUnit<{0}>.Run{1} Failed in callback.", _workName, nextAction);
                    else
                        Tracing.InfoFmt("WorkUnit<{0}>.Run{1} ok.", _workName, nextAction);

					if (Interlocked.CompareExchange(ref hasReturned, 1, 0) == 0) {
						if (ex == null) {
							_state = succState;
							callback(null);
						} else {
							_error = ex;
							_state = failState;
							callback(ex);
						}
					} else {
                        Tracing.ErrorFmt("WorkUnit<{0}>.Run{1} callback return more than once.", _workName, nextAction);
					}
				}
			);

			try {
				Proc(ctx);
			} catch (Exception ex) {
				if (Interlocked.CompareExchange(ref hasReturned, 1, 0) == 0) {
					Tracing.ErrorFmt(ex, "WorkUnit<{0}>.Run{1} Failed.", _workName, nextAction);
					_error = ex;
					_state = failState;
					callback(ex);
				} else {
                    Tracing.ErrorFmt("WorkUnit<{0}>.Run{1} return more than once.", _workName, nextAction);
				}
			}
		}

		public bool CanTry()
		{
			if (_prevWorkUnits == null)
				return true;

			foreach (var w in _prevWorkUnits) {
				if (w.WorkState != TccWorkState.Tryed) {
					return false;
				}
			}
			return true;
		}
	}
}
