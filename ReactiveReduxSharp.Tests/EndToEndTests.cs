using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;

namespace ReactiveReduxSharp.Tests
{
	public class EndToEndTests
	{
		private class State
		{
			public string Value { get; set; }
			public int[] Numbers { get; set; } = new int[0];
		}

		private class UpdateValueAction : IAction
		{
			public readonly string Payload;

			public UpdateValueAction(string payload)
			{
				Payload = payload;
			}
		}

		private class AddNumberAction : IAction
		{
			public readonly int Payload;

			public AddNumberAction(int payload)
			{
				Payload = payload;
			}
		}

		private class AddNumberSideEffectAction : IAction
		{
			public readonly int Payload;

			public AddNumberSideEffectAction(int payload)
			{
				Payload = payload;
			}
		}

		private static State Reducer(State state, IAction action)
		{
			switch (action)
			{
				case UpdateValueAction updateValue:
					return new State
					{
						Value = updateValue.Payload,
						Numbers = state.Numbers
					};
				case AddNumberAction addNumber:
					return new State
					{
						Value = state.Value,
						Numbers = state.Numbers.Concat(new[] { addNumber.Payload }).ToArray()
					};
				case AddNumberSideEffectAction addNumberSideEffect:
					return new State
					{
						Value = $"Side effect: {addNumberSideEffect.Payload}",
						Numbers = state.Numbers
					};
				default:
					return state;
			}
		}

		private class Effects
		{
			private readonly IObservable<IAction> _actions;

			[Effect] public IObservable<IAction> SideEffect() => _actions
				.Select(action => action as AddNumberAction)
				.Where(action => action != null)
				.Select(action => action.Payload)
				.Select(n => new AddNumberSideEffectAction(n));

			public Effects(IObservable<IAction> actions)
			{
				_actions = actions;
			}
		}

		private ReduxApp<State> _app;

		[SetUp]
		public void Setup()
		{
			_app = new ReduxApp<State>(
				new State { Value = "initial" },
				Reducer,
				new[] { typeof(Effects) }
			);
		}

		[Test]
		public void DispatchingActionsUpdatesTheStore()
		{
			var history = new List<string>();
			var observable = _app.Store
				.Select(state => state.Value);
			using (observable.Subscribe(value => history.Add(value)))
			{
				Assert.That(history, Is.EqualTo(new[] { "initial" }));
				_app.Dispatch(new UpdateValueAction("do the dishes"));
				Assert.That(history, Is.EqualTo(new[] { "initial", "do the dishes" }));
			}
		}

		[Test]
		public void CanGetStoreSliceUsingMemoizedSelector()
		{
			var lastNumber = int.MinValue;
			var observable = _app.Store
				.Selector(state => state.Numbers)
				.Selector(numbers => numbers?.LastOrDefault() ?? 0);
			using (observable.Subscribe(value => lastNumber = value))
			{
				Assert.That(lastNumber, Is.EqualTo(0));
				_app.Dispatch(new AddNumberAction(42));
				Assert.That(lastNumber, Is.EqualTo(42));
			}
		}

		[Test]
		public void EffectsAreInvokedAfterStoreUpdates()
		{
			var lastValue = "";
			var observable = _app.Store
				.Select(state => state.Value);
			using (observable.Subscribe(value => lastValue = value))
			{
				_app.Dispatch(new AddNumberAction(42));
				Assert.That(lastValue, Is.EqualTo("Side effect: 42"));
			}
		}
	}
}