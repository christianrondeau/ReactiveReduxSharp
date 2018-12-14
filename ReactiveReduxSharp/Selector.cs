using System;

namespace ReactiveReduxSharp
{
	public class Selector<TState, TStateSlice> where TState: class
	{
		private readonly Func<TState, TStateSlice> _projectorFunc;
		private (TState State, TStateSlice Slice) _memoized;

		public TStateSlice Projector(TState state)
		{
			if (_memoized.State == state)
				return _memoized.Slice;

			_memoized = (state, _projectorFunc(state));
			return _memoized.Slice;
		}

		public Selector(Func<TState, TStateSlice> projectorFunc)
		{
			_projectorFunc = projectorFunc ?? throw new ArgumentNullException(nameof(projectorFunc));
		}
	}
}