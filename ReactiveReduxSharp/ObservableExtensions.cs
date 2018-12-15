using System;
using System.Reactive.Linq;

namespace ReactiveReduxSharp
{
	public static class ObservableExtensions
	{
		public static IObservable<TStateSlice> Selector<TState, TStateSlice>(this IObservable<TState> observable, Func<TState, TStateSlice> selector) where TState: class
		{
			return observable.Select(new Selector<TState, TStateSlice>(selector).Projector);
		}
	}
}