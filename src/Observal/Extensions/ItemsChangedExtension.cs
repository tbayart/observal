using System;

namespace Observal.Extensions
{
    public class ItemsChangedExtension : IObserverExtension
    {
        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoved;

        public void Configure(Observer observer)
        {
            
        }

        public void Attach(Observer observer, object attachedItem)
        {
            var handler = ItemAdded;
            if (handler != null) handler(this, new ItemEventArgs(attachedItem));
        }

        public void Detach(Observer observer, object detachedItem)
        {
            var handler = ItemRemoved;
            if (handler != null) handler(this, new ItemEventArgs(detachedItem));
        }
    }
}
