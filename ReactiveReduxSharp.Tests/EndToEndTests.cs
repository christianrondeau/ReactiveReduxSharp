using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;

namespace ReactiveReduxSharp.Tests
{
	public class EndToEndTests
	{
		public class State
		{
			public string Value { get; set; }
			public int[] Numbers { get; set; } = new int[0];
		}

		public class UpdateValueAction : IAction
		{
			public string Payload;

			public UpdateValueAction(string payload)
			{
				Payload = payload;
			}
		}

		public class AddNumberAction : IAction
		{
			public int Payload;

			public AddNumberAction(int payload)
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
				default:
					return state;
			}
		}

		private ReduxStore<State> _store;

		[SetUp]
		public void Setup()
		{
			_store = new ReduxStore<State>(
				new State { Value = "initial" },
				Reducer
			);
		}

		[Test]
		public void DispatchingActionsUpdatesTheStore()
		{
			var history = new List<string>();
			var observable = _store.Observable
				.Select(state => state.Value);
			using (observable.Subscribe(value => history.Add(value)))
			{
				Assert.That(history, Is.EqualTo(new[] { "initial" }));
				_store.Dispatch(new UpdateValueAction("do the dishes"));
				Assert.That(history, Is.EqualTo(new[] { "initial", "do the dishes" }));
			}
		}

		[Test]
		public void CanGetStoreSliceUsingMemoizedSelector()
		{
			var lastNumber = int.MinValue;
			var observable = _store.Observable
				.Select(new Selector<State, int[]>(state => state.Numbers).Projector)
				.Select(new Selector<int[], int>(numbers => numbers?.LastOrDefault() ?? 0).Projector);
			using (observable.Subscribe(value => lastNumber = value))
			{
				Assert.That(lastNumber, Is.EqualTo(0));
				_store.Dispatch(new AddNumberAction(42));
				Assert.That(lastNumber, Is.EqualTo(42));
			}
		}
	}
}