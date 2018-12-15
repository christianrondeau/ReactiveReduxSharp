using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReactiveReduxSharp
{
	public class ReduxEffects : IDisposable
	{
		private readonly IObservable<IAction> _actions;
		private readonly Action<IAction> _dispatch;
		private readonly List<IDisposable> _effectSubscriptions = new List<IDisposable>();

		public ReduxEffects(IObservable<IAction> actions, Type[] types, Action<IAction> dispatch)
		{
			_actions = actions;
			_dispatch = dispatch;
			if (types == null) return;
			foreach(var effect in types)
				BindEffectClass(effect);
		}

		public object BindEffectClass(Type effectType)
		{
			var effect = Activator.CreateInstance(effectType, _actions);
			var properties = effectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				var effectAttribute = prop.GetCustomAttribute<EffectAttribute>();
				if (effectAttribute == null) continue;

				var effectObservable = ((IObservable<IAction>) prop.Invoke(effect, null));
				_effectSubscriptions.Add(effectObservable.Subscribe(effectAttribute.Dispatch ? _dispatch : NoDispatch));}

			return effect;
		}

		private static void NoDispatch(IAction obj)
		{
		}

		public void Dispose()
		{
			_effectSubscriptions.ForEach(e => e.Dispose());
		}
	}
}