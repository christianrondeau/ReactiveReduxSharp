using System;
using System.Reactive.Subjects;

namespace ReactiveReduxSharp
{
	public class ReduxApp<TState> : IDisposable where TState: class
	{
		private readonly Func<TState, IAction, TState> _reducer;
		private readonly BehaviorSubject<TState> _store;
		private readonly Subject<IAction> _actions;
		private TState _state;

		public IObservable<TState> Store => _store;
		public IObservable<IAction> Actions => _actions;

		public ReduxApp(TState state, Func<TState, IAction, TState> reducer)
		{
			_state = state;
			_store = new BehaviorSubject<TState>(state);
			_actions = new Subject<IAction>();
			_reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
		}

		public void Dispatch(IAction action)
		{
			_state = _reducer(_state, action);
			_store.OnNext(_state);
			_actions.OnNext(action);
		}

		public void Dispose()
		{
			_store.Dispose();
		}
	}
}
