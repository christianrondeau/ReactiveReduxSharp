using System;
using System.Reactive.Subjects;

namespace ReactiveReduxSharp
{
	public class ReduxStore<TState> : IDisposable where TState: class
	{
		private readonly Func<TState, IAction, TState> _reducer;
		private readonly BehaviorSubject<TState> _subject;
		private TState _state;

		public IObservable<TState> Observable => _subject;

		public ReduxStore(TState state, Func<TState, IAction, TState> reducer)
		{
			_state = state;
			_subject = new BehaviorSubject<TState>(state);
			_reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
		}

		public void Dispatch(IAction action)
		{
			_state = _reducer(_state, action);
			_subject.OnNext(_state);
		}

		public void Dispose()
		{
			_subject.Dispose();
		}
	}
}
