using System.Collections.Generic;
using System.Collections.Specialized;

namespace Observal.Extensions
{
    /// <summary>
    /// When collections that implement <see cref="INotifyCollectionChanged"/> are added to the observer, 
    /// this extension will expand the collection and add all items to the observer. 
    /// </summary>
    /// <remarks>
    /// Works by using the <see cref="ObservableCollectionTracker"/> to do the dirty work of managing the 
    /// collection changed events.
    /// </remarks>
    public class CollectionExpansionExtension : ObserverExtension
    {
        private readonly Dictionary<object, ObservableCollectionTracker> _caches = new Dictionary<object, ObservableCollectionTracker>();
        private bool _useWeakEvents;

        /// <summary>
        /// Specifies that weak events should be used. This allows the observer to be garbage collected even 
        /// if the events are still subscribed to on child items. This feature is NOT enabled by default.
        /// </summary>
        /// <returns>This instance, for fluent interfaces.</returns>
        public CollectionExpansionExtension UseWeakEvents()
        {
            AssertNotConfiguredYet("Cannot change usage of weak events once this extension has been configured.");
            _useWeakEvents = true;
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
            var notifiable = attachedItem as INotifyCollectionChanged;
            if (notifiable == null)
                return;

            var cache = new ObservableCollectionTracker(Observer.Add, Observer.Release, _useWeakEvents);
            cache.Attach(notifiable);
            _caches.Add(attachedItem, cache);
        }

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        protected override void Detach(object detachedItem)
        {
            if (!_caches.ContainsKey(detachedItem)) 
                return;
            
            var cache = _caches[detachedItem];
            cache.Detach();
            _caches.Remove(detachedItem);
            foreach (var item in cache.Items)
            {
                Observer.Release(item);
            }
        }
    }
}