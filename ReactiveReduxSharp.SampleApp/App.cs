using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveReduxSharp.SampleApp.Actions;

namespace ReactiveReduxSharp.SampleApp
{
	public class App : IDisposable
	{
		public static App Create()
		{
			return new App(new State(), Reducers.Reducer);
		}

		private readonly ReduxStore<State> _store;
		private readonly List<IDisposable> _subscriptions;

		public App(State state, Func<State, IAction, State> reducer)
		{
			_subscriptions = new List<IDisposable>();
			_store = new ReduxStore<State>(state, reducer);
			var todos = _store.Observable.Select(new Selector<State, string[]>(s => s.Todos).Projector);
			_subscriptions.Add(
				todos
					.Select(new Selector<string[], int>(l => l.Length).Projector)
					.Where(c => c > 0)
					.Subscribe(OutputTodosCount)
				);
			_subscriptions.Add(
				todos
					.Select(new Selector<string[], string[]>(l => l.TakeLast(3).ToArray()).Projector)
					.Where(l => l.Length > 0)
					.Subscribe(OutputTodosList)
				);
		}

		public void Listen()
		{
			Console.WriteLine("Type your todos and press enter. Enter 'q' to exit.");
			while (true)
			{
				Console.Write("> ");
				var command = Console.ReadLine();
				switch (command)
				{
					case "":
						break;
					case "q":
						return;
					default:
						_store.Dispatch(new AddTodo(command));
						break;

				}
			}
		}

		private static void OutputTodosCount(int todosCount)
		{
			Console.WriteLine($"There is now {todosCount} todo{(todosCount == 1 ? "" : "s")}");
		}

		private static void OutputTodosList(string[] todos)
		{
			Console.WriteLine($"Latest todos:");
			foreach (var todo in todos)
			{
				Console.WriteLine($"* {todo}");
			}
		}

		public void Dispose()
		{
			_subscriptions.ForEach(s => s.Dispose());
			_store.Dispose();
		}
	}
}