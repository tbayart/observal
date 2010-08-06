using System;
using System.Collections.Generic;
using Observal.Utilities;

namespace Observal.Extensions
{
    /// <summary>
    /// When items are added to an observer, this extension can be used to access child properties on the 
    /// item, adding the property value to the observer. To set up a hierarchy of objects, combine this 
    /// extension with <see cref="CollectionExpansionExtension"/>.
    /// </summary>
    public class TraverseExtension : ObserverExtension
    {
        private readonly List<Tuple<Type, Func<object, object>>> _childrenReaders = new List<Tuple<Type, Func<object, object>>>();

        /// <summary>
        /// Sets a path that will be followed and added to the collection. For example, 
        /// <c>Follow&lt;Customer&gt;(c =&gt; c.Address)</c> would specify that anytime a Customer is added 
        /// to the observer, its address should also be added.
        /// </summary>
        /// <typeparam name="TObject">The type of object being added, which contains properties to follow.</typeparam>
        /// <param name="childrenSelector">The children selector.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public TraverseExtension Follow<TObject>(Func<TObject, object> childrenSelector)
        {
            Guard.ArgumentNotNull(childrenSelector, "childrenSelector");
            AssertNotConfiguredYet("Paths to follow must be set before the extension is configured.");
            _childrenReaders.Add(new Tuple<Type, Func<object, object>>(typeof(TObject), x => childrenSelector((TObject)x)));
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
            foreach (var reader in _childrenReaders)
            {
                var type = reader.Item1;
                var callback = reader.Item2;

                if (type.IsAssignableFrom(attachedItem.GetType()))
                {
                    var children = callback(attachedItem);
                    if (children != null)
                    {
                        Observer.Add(children);
                    }
                }
            }
        }

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        protected override void Detach(object detachedItem)
        {
            foreach (var reader in _childrenReaders)
            {
                var type = reader.Item1;
                var callback = reader.Item2;

                if (type.IsAssignableFrom(detachedItem.GetType()))
                {
                    var children = callback(detachedItem);
                    if (children != null)
                    {
                        Observer.Release(children);
                    }
                }
            }
        }
    }
}