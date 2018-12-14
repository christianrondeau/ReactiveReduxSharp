using System;
using Newtonsoft.Json;
using ReactiveReduxSharp.SampleApp.Actions;

namespace ReactiveReduxSharp.SampleApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var store = new ReduxStore<State>(new State(), Reducers.Reducer))
			{
				using (store.Observable.Subscribe(LogState))
				{
					while (true)
					{
						var command = Console.ReadLine();
						switch (command)
						{
							case "":
								break;
							case "q":
								return;
							default:
								store.Dispatch(new AddTodo(command));
								break;

						}
					}
				}
			}
		}

		private static void LogState(State state)
		{
			Console.WriteLine(JsonConvert.SerializeObject(state));
		}
	}
}
