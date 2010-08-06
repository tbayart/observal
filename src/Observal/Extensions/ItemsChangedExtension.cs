using System;
using System.Collections.Generic;

namespace Observal.Extensions
{
    public class ItemsChangedExtension : ObserverExtension
    {
        private readonly List<Action<object>> _addedCallbacks = new List<Action<object>>();
        private readonly List<Action<object>> _removedCallbacks = new List<Action<object>>();

        public ItemsChangedExtension WhenAdded(Action<object> callback)
        {
            return WhenAdded<object>(callback);
        }

        public ItemsChangedExtension WhenAdded<TObject>(Action<TObject> callback)
        {
            AssertNotConfiguredYet("Cannot add callbacks as this extension has already been configured.");
            _addedCallbacks.Add(x =>
            {
                if (x is TObject)
                    callback((TObject)x);
            });
            return this;
        }

        public ItemsChangedExtension WhenRemoved(Action<object> callback)
        {
            return WhenRemoved<object>(callback);
        }

        public ItemsChangedExtension WhenRemoved<TObject>(Action<TObject> callback)
        {
            AssertNotConfiguredYet("Cannot add callbacks as this extension has already been configured.");
            _removedCallbacks.Add(x =>
            {
                if (x is TObject)
                    callback((TObject)x);
            });
            return this;
        }

        public ItemsChangedExtension WhenAddedOrRemoved(Action<object> callback)
        {
            return WhenAddedOrRemoved<object>(callback);
        }

        public ItemsChangedExtension WhenAddedOrRemoved<TObject>(Action<TObject> callback)
        {
            WhenAdded(callback);
            WhenRemoved(callback);
            return this;
        }

        protected override void Attach(Observer observer, object attachedItem)
        {
            var callbacks = _addedCallbacks.ToArray();
            foreach (var cb in callbacks)
            {
                cb(attachedItem);
            }
        }

        protected override void Detach(Observer observer, object detachedItem)
        {
            var callbacks = _removedCallbacks.ToArray();
            foreach (var cb in callbacks)
            {
                cb(detachedItem);
            }
        }
    }
}
