using System;
using System.Collections.Generic;
using Observal.Utilities;

namespace Observal.Extensions
{
    /// <summary>
    /// When objects are added to or removed from the observer, this extension can notify a set of callbacks
    /// about the change.
    /// </summary>
    public class ItemsChangedExtension : ObserverExtension
    {
        private readonly List<Action<object>> _addedCallbacks = new List<Action<object>>();
        private readonly List<Action<object>> _removedCallbacks = new List<Action<object>>();

        /// <summary>
        /// Sets up a callback that will be invoked any time an object is added to the observer.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenAdded(Action<object> callback)
        {
            return WhenAdded<object>(callback);
        }

        /// <summary>
        /// Sets up a callback that will be invoked any time an object of the given type is added to the 
        /// observer.
        /// </summary>
        /// <param name="callback">The callback that will be invoked on addition of new <see cref="TObject"/> 
        /// instances.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenAdded<TObject>(Action<TObject> callback)
        {
            Guard.ArgumentNotNull(callback, "callback");
            AssertNotConfiguredYet("Cannot add callbacks as this extension has already been configured.");
            _addedCallbacks.Add(x =>
            {
                if (x is TObject)
                    callback((TObject)x);
            });
            return this;
        }

        /// <summary>
        /// Sets up a callback that will be invoked any time an object is removed from the observer.
        /// </summary>
        /// <param name="callback">The callback that will be invoked on removal of object instances.
        /// </param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenRemoved(Action<object> callback)
        {
            return WhenRemoved<object>(callback);
        }

        /// <summary>
        /// Sets up a callback that will be invoked any time an object of the given type is removed to the 
        /// observer.
        /// </summary>
        /// <param name="callback">The callback that will be invoked on removal of <see cref="TObject"/> 
        /// instances.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenRemoved<TObject>(Action<TObject> callback)
        {
            Guard.ArgumentNotNull(callback, "callback");
            AssertNotConfiguredYet("Cannot add callbacks as this extension has already been configured.");
            _removedCallbacks.Add(x =>
            {
                if (x is TObject)
                    callback((TObject)x);
            });
            return this;
        }

        /// <summary>
        /// Sets up a callback that will be invoked any time an object is added to or removed from the 
        /// observer.
        /// </summary>
        /// <param name="callback">The callback that will be invoked on addition or removal of object 
        /// instances.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenAddedOrRemoved(Action<object> callback)
        {
            return WhenAddedOrRemoved<object>(callback);
        }

        /// <summary>
        /// Sets up a callback that will be invoked any time an object of the given type is added to or 
        /// removed from the observer.
        /// </summary>
        /// <param name="callback">The callback that will be invoked on addition or removal of 
        /// <see cref="TObject"/> instances.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public ItemsChangedExtension WhenAddedOrRemoved<TObject>(Action<TObject> callback)
        {
            WhenAdded(callback);
            WhenRemoved(callback);
            return this;
        }

        /// <summary>
        /// Notifies this extension that an item has been added to the current observer.
        /// </summary>
        /// <param name="attachedItem">The attached item that was just added to the observer.</param>
        /// <remarks>
        /// This method is guaranteed to only be called once per item (unless the item is added, removed,
        /// and added again).
        /// </remarks>
        protected override void Attach(object attachedItem)
        {
            var callbacks = _addedCallbacks.ToArray();
            foreach (var cb in callbacks)
            {
                cb(attachedItem);
            }
        }

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        protected override void Detach(object detachedItem)
        {
            var callbacks = _removedCallbacks.ToArray();
            foreach (var cb in callbacks)
            {
                cb(detachedItem);
            }
        }
    }
}
