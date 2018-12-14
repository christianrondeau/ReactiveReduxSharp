using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReduxExperiment.Store;

namespace ReduxExperiment.Tests
{
	public class EndToEndTests
	{
		public class State
		{
			public string Value { get; set; }
		}

		public class UpdateValueAction : IAction
		{
			public string Type { get; } = "UPDATE_ACTION";
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
			var updates = new List<State>();
			using (_store.Observable.Subscribe(state => updates.Add(state)))
			{
				Assert.That(updates.Count, Is.EqualTo(1), "Should receive the initial state on subscribe");
				_store.Dispatch(new UpdateValueAction("ADD_TODO"));
				Assert.That(updates.Count, Is.EqualTo(2), "Should update the state");
			}
		}
	}
}