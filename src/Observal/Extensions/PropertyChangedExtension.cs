using System;
using System.Collections.Generic;
using System.ComponentModel;
using Observal.Eventing;

namespace Observal.Extensions
{
    public class PropertyChangedExtension : ObserverExtension
    {
        private readonly List<Action<PropertyChangeNotification>> _subscribers = new List<Action<PropertyChangeNotification>>();
        private PropertyChangedEventHandler _handler;

        public PropertyChangedExtension()
        {
            _handler = ItemPropertyChanged;
        }

        public PropertyChangedExtension UseWeakEvents()
        {
            AssertNotConfiguredYet("Cannot change weak event settings as this extension has already been configured.");
            _handler = new WeakEventHandler<PropertyChangedEventArgs>(ItemPropertyChanged).WeakHandler;
            return this;
        }

        public PropertyChangedExtension WhenPropertyChanges(Action<PropertyChangeNotification> callback)
        {
            AssertNotConfiguredYet("Cannot add property change handlers as this extension has already been configured.");
            _subscribers.Add(callback);
            return this;
        }

        public PropertyChangedExtension WhenPropertyChanges<TObject>(Action<PropertyChangeNotification<TObject>> callback)
        {
            AssertNotConfiguredYet("Cannot add property change handlers as this extension has already been configured.");
            _subscribers.Add(pn =>
            {
                if (pn.Source is TObject)
                {
                    callback(new PropertyChangeNotification<TObject>((TObject)pn.Source, pn.PropertyName));
                }
            });
            return this;
        }

        protected override void Attach(Observer observer, object attachedItem)
        {
            var notifiable = attachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged += _handler;
        }

        protected override void Detach(Observer observer, object detachedItem)
        {
            var notifiable = detachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged -= _handler;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var callbacks = _subscribers.ToArray();
            if (callbacks.Length == 0) return;

            var notification = new PropertyChangeNotification(sender, e.PropertyName);
            foreach (var item in callbacks)
            {
                item(notification);
            }
        }
    }
}