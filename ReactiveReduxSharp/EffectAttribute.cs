using System;

namespace ReactiveReduxSharp
{
	public class EffectAttribute : Attribute
	{
		public bool Dispatch { get; set; }
	}
}