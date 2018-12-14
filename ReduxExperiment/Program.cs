using System;
using System.Linq;
using Newtonsoft.Json;
using ReduxExperiment.Store;

namespace ReduxExperiment
{
	public class State
	{
		public string[] Todos { get; set; } = new string[0];
	}

	public class AddTodo : IAction
	{
		public string Type { get; } = "ADD_TODO";
		public string Payload;

		public AddTodo(string name)
		{
			Payload = name;
		}
	}

	public static class Reducers
	{
		public static State Reducer(State state, IAction action)
		{
			switch (action)
			{
				case AddTodo addTodo :
					return new State
					{
						Todos = state.Todos.Concat(new[] {addTodo.Payload}).ToArray()
					};
				default:
					return state;
			}
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var store = new ReduxStore<State>(new State(), Reducers.Reducer);
			var subscription = store.Observable.Subscribe(
				state => Console.WriteLine(JsonConvert.SerializeObject(state))
			);
			store.Dispatch(new AddTodo(args[0]));
			Console.ReadLine();
			subscription.Dispose();
		}
	}
}
