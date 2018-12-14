using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NUnit.Framework;

namespace ReactiveReduxSharp.Tests
{
	public class EndToEndTests
	{
		public class State
		{
			public string Value { get; set; }
		}

		public class UpdateValueAction : IAction
		{
			public string Payload;

			public UpdateValueAction(string payload)
			{
				Payload = payload;
			}
		}

		private ReduxStore<State> _store;

		[SetUp]
		public void Setup()
		{
			_store = new ReduxStore<State>(
				new State {Value = "initial"},
				(previousState, action) => new State {Value = ((UpdateValueAction)action).Payload}
			);
		}

		[Test]
		public void Test1()
		{
			var history = new List<string>();
			var observable = _store.Observable.Select(state => state.Value);
			using (observable.Subscribe(value => history.Add(value)))
			{
				Assert.That(history, Is.EqualTo(new[] {"initial"}));
				_store.Dispatch(new UpdateValueAction("do the dishes"));
				Assert.That(history, Is.EqualTo(new[] {"initial", "do the dishes"}));
			}
		}
	}
}