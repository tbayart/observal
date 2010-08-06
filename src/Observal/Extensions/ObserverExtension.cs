using System;
using Observal.Utilities;

namespace Observal.Extensions
{
    /// <summary>
    /// A base class that should be used for custom implementations of <see cref="IObserverExtension"/>.
    /// </summary>
    public abstract class ObserverExtension : IObserverExtension
    {
        private bool _hasConfigured;

        /// <summary>
        /// Gets the current observer that this extension is attached to.
        /// </summary>
        /// <value>The observer.</value>
        protected Observer Observer { get; private set; }

        /// <summary>
        /// Ensures that the Configure method has not already been called on this extension. This method 
        /// should be called whenever options are set that should only be set before the first item is added
        /// to the observer.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void AssertNotConfiguredYet(string message)
        {
            if (_hasConfigured)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Configures this extension for a given observer. This allows the extension to initialize itself
        /// or add any other extensions that it depends on.
        /// </summary>
        /// <param name="observer">The observer.</param>
        protected virtual void Configure(Observer observer)
        {
        }

        /// <summary>
        /// Notifies this extension that an item has been added to the current observer.
        /// </summary>
        /// <param name="attachedItem">The attached item that was just added to the observer.</param>
        /// <remarks>
        /// This method is guaranteed to only be called once per item (unless the item is added, removed,
        /// and added again).
        /// </remarks>
        protected abstract void Attach(object attachedItem);

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        protected abstract void Detach(object detachedItem);

        void IObserverExtension.Configure(Observer observer)
        {
            Guard.ArgumentNotNull(observer, "observer");

            if (_hasConfigured) 
                throw new InvalidOperationException("This extension has already been configured for a specific observer.");

            Observer = observer;
            _hasConfigured = true;
            Configure(observer);
        }

        void IObserverExtension.Attach(object attachedItem)
        {
            Attach(attachedItem);
        }

        void IObserverExtension.Detach(object detachedItem)
        {
            Detach(detachedItem);
        }
    }
}
