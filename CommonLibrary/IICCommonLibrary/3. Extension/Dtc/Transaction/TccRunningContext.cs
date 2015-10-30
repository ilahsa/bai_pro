using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Dtc
{
	class TccRunningContext<Context>
	{
		private bool _successed;
		private Context _ctx;
		private ITracing _tracing;
		private TccTransactionState _succState = TccTransactionState.New;
		private TccTransactionState _failedState = TccTransactionState.New;
		private List<ComboClass<TccWorkUnit<Context>, TccAction>> _works;

		public TccRunningContext(Context ctx, ITracing tracing)
		{
			_ctx = ctx;
			_tracing = tracing;
			_successed = true;
			_works = new List<ComboClass<TccWorkUnit<Context>, TccAction>>();
		}

		public bool Successed
		{
			get { return _successed; }
		}

		public TccTransactionState NextState
		{
			get { return _successed ? _succState : _failedState; }
		}

		public bool Finished
		{
			get { return NextState == TccTransactionState.Confirmed || NextState == TccTransactionState.Cancelled; }
		}

		public void AddWork(TccWorkUnit<Context> work, TccAction nextAction)
		{
			_works.Add(new ComboClass<TccWorkUnit<Context>,TccAction>(work, nextAction));
		}

		public void SetNextState(TccTransactionState succState, TccTransactionState failedState)
		{
			_succState = succState;
			_failedState = failedState;
		}

		public void Run(Action callback)
		{
			int count = _works.Count;
			foreach (var a in _works) {
				a.V1.RunAction(_ctx, a.V2, 
					delegate(Exception ex) {
						if (ex != null) {
							_successed = false;
						}
						Interlocked.Decrement(ref count);
						if (count == 0) {
							callback();
						}
					}
				);
			}
		}

		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			foreach (var w in _works) {
				s.AppendFormat("WorkUnit<{0}>.Run{1} State:{2}\r\n", w.V1.WorkName, w.V2, w.V1.WorkState);
			}
			return s.ToString();
		}
	}
}
