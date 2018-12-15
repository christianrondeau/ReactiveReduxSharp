using System;
using System.Reactive.Linq;
using ReactiveReduxSharp.SampleApp.Actions;

namespace ReactiveReduxSharp.SampleApp
{
	public class TodoEffects
	{
		private readonly IObservable<IAction> _actions;

		[Effect(Dispatch = false)]
		public IObservable<IAction> Reminder() => _actions
			.SelectAction<AddTodo>()
			.Throttle(TimeSpan.FromSeconds(5))
			.Do(action => Console.WriteLine($"Remember to '{action.Payload}'!"));

		public TodoEffects(IObservable<IAction> actions)
		{
			_actions = actions;
		}
	}
}