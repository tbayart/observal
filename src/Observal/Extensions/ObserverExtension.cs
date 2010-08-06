using System;

namespace Observal.Extensions
{
    public abstract class ObserverExtension : IObserverExtension
    {
        private bool _hasConfigured;

        protected void AssertNotConfiguredYet(string message)
        {
            if (_hasConfigured)
                throw new InvalidOperationException(message);
        }

        protected virtual void Configure(Observer observer)
        {
        }

        protected virtual void Attach(Observer observer, object attachedItem)
        {
        }


        protected virtual void Detach(Observer observer, object detachedItem)
        {
        }

        void IObserverExtension.Configure(Observer observer)
        {
            _hasConfigured = true;
            Configure(observer);
        }

        void IObserverExtension.Attach(Observer observer, object attachedItem)
        {
            Attach(observer, attachedItem);
        }

        void IObserverExtension.Detach(Observer observer, object detachedItem)
        {
            Detach(observer, detachedItem);
        }
    }
}
