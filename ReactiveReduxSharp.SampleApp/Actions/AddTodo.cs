namespace ReactiveReduxSharp.SampleApp.Actions
{
	public class AddTodo : IAction
	{
		public string Payload { get; }

		public AddTodo(string payload)
		{
			Payload = payload;
		}
	}
}