namespace ReactiveReduxSharp.SampleApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var app = App.Create())
			{
				app.Listen();
			}
		}
	}
}
