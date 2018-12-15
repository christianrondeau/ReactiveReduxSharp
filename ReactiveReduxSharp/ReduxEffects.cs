using System;
using System.Collections.Generic;
using System.Linq;
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
			var properties = effectType
				.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.GetCustomAttribute<EffectAttribute>() != null);

			foreach (var prop in properties)
				_effectSubscriptions.Add(((IObservable<IAction>) prop.Invoke(effect, null)).Subscribe(_dispatch));

			return effect;
		}

		public void Dispose()
		{
			_effectSubscriptions.ForEach(e => e.Dispose());
		}
	}
}