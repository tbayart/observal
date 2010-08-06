using System;
using System.Diagnostics;
using System.Reflection;

namespace Observal.Eventing
{
    /// <summary>
    /// Wraps an event handler with a weak reference, allowing the subscriber to be garbage collected without 
    /// being kept alive by the publisher of the event. 
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
    /// <remarks>
    /// Usage example:
    /// <code>
    ///     Alarm.Beep += new WeakEventHandler&lt;FooEventArgs&gt;(FooHappened).WeakHandler;
    /// </code>
    /// </remarks>
    [DebuggerNonUserCode]
    public sealed class WeakEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly Action<object, object, object> _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventHandler&lt;TEventArgs&gt;"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            _callback = CreateDelegateInvoker(callback.Method);
            _targetReference = new WeakReference(callback.Target, true);
        }

        /// <summary>
        /// Used as the event handler which should be subscribed to the event.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">The event args.</param>
        [DebuggerNonUserCode]
        public void WeakHandler(object sender, object e)
        {
            var target = _targetReference.Target;
            if (target == null) return;

            _callback(target, sender, e);
        }

        /// <remarks>
        /// Creates an open delegate for invoking the callback - see Jon Skeet's blog post for an example:
        /// http://msmvps.com/blogs/jon_skeet/archive/2008/08/09/making-reflection-fly-and-exploring-delegates.aspx
        /// </remarks>
        private static Action<object, object, object> CreateDelegateInvoker(MethodInfo method)
        {
            var magicMethodHelper = typeof(WeakEventHandler<TEventArgs>).GetMethod("CreateGenericDelegateInvoker", BindingFlags.Static | BindingFlags.NonPublic);
            magicMethodHelper = magicMethodHelper.MakeGenericMethod(method.DeclaringType, method.GetParameters()[0].ParameterType, method.GetParameters()[1].ParameterType);

            var action = magicMethodHelper.Invoke(null, new object[] { method });

            return (Action<object, object, object>)action;
        }

        private static Action<object, object, object> CreateGenericDelegateInvoker<TTarget, TParam, TReturn>(MethodInfo method) where TTarget : class
        {
            var genericAction = (Action<TTarget, TParam, TReturn>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam, TReturn>), method);
            Action<object, object, object> untypedAction = (target, sender, e) => genericAction((TTarget)target, (TParam)sender, (TReturn)e);
            return untypedAction;
        }
    }
}
