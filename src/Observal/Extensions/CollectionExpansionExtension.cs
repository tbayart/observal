using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Observal.Extensions
{
    public class CollectionExpansionExtension : IObserverExtension
    {
        private readonly Dictionary<object, ObservableCollectionTracker> _caches = new Dictionary<object, ObservableCollectionTracker>();

        public void Configure(Observer observer)
        {

        }

        public void Attach(Observer observer, object attachedItem)
        {
            var enumerable = attachedItem as IEnumerable;
            if (enumerable == null)
                return;

            var notifiable = attachedItem as INotifyCollectionChanged;
            if (notifiable == null)
            {
                return;
            }
            else
            {
                var cache = new ObservableCollectionTracker(observer.Add, observer.Release);
                cache.Attach(notifiable);

                _caches.Add(attachedItem, cache);   
            }
        }

        public void Detach(Observer observer, object detachedItem)
        {
            var enumerable = detachedItem as IEnumerable;
            if (enumerable == null)
                return;

            if (_caches.ContainsKey(detachedItem))
            {
                var cache = _caches[detachedItem];
                _caches.Remove(detachedItem);
                foreach (var item in cache.Items)
                {
                    observer.Release(item);
                }
                
            }
        }
    }
}