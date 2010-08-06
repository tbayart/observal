using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Observal.Extensions
{
    public class CollectionExpansionExtension : ObserverExtension
    {
        private readonly Dictionary<object, ObservableCollectionTracker> _caches = new Dictionary<object, ObservableCollectionTracker>();

        protected override void Attach(Observer observer, object attachedItem)
        {
            var enumerable = attachedItem as IEnumerable;
            if (enumerable == null)
                return;

            var notifiable = attachedItem as INotifyCollectionChanged;
            if (notifiable == null)
                return;

            var cache = new ObservableCollectionTracker(observer.Add, observer.Release);
            cache.Attach(notifiable);
            _caches.Add(attachedItem, cache);
        }

        protected override void Detach(Observer observer, object detachedItem)
        {
            var enumerable = detachedItem as IEnumerable;
            if (enumerable == null)
                return;

            if (!_caches.ContainsKey(detachedItem)) 
                return;
            
            var cache = _caches[detachedItem];
            cache.Detach();
            _caches.Remove(detachedItem);
            foreach (var item in cache.Items)
            {
                observer.Release(item);
            }
        }
    }
}