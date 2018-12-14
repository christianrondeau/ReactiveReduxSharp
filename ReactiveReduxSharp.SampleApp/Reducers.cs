using System.Linq;
using ReactiveReduxSharp.SampleApp.Actions;

namespace ReactiveReduxSharp.SampleApp
{
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
}