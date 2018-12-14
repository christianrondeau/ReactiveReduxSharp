using System;
using Newtonsoft.Json;
using ReduxExperiment.Store;

namespace ReduxExperiment
{
	public class State
	{
		public string[] Todos { get; set; } = new string[0];
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var store = new ReduxStore<State>(new State());
			store.Dispatch(new ReduxAction("ADD_TODO"));
			var subscription = store.Observable.Subscribe(
				state => Console.WriteLine(JsonConvert.SerializeObject(state))
			);
			Console.ReadLine();
			subscription.Dispose();
		}
	}
}
