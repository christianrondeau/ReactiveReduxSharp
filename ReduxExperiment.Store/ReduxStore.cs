using System;
using System.Reactive.Subjects;

namespace ReduxExperiment.Store
{
	public class ReduxStore<TState>
	{
		private readonly Func<TState, IAction, TState> _reducer;
		private readonly BehaviorSubject<TState> _observable;
		private TState _state;

		public IObservable<TState> Observable => _observable;

		public ReduxStore(TState state, Func<TState, IAction, TState> reducer)
		{
			_state = state;
			_observable = new BehaviorSubject<TState>(state);
			_reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
		}

		public void Dispatch(IAction action)
		{
			_state = _reducer(_state, action);
			_observable.OnNext(_state);
		}
	}
}
