using System;
using System.Collections.Generic;
using System.ComponentModel;
using Observal.Eventing;
using Observal.Utilities;

namespace Observal.Extensions
{
    /// <summary>
    /// When objects that implement the <see cref="INotifyPropertyChanged"/> interface are added to an 
    /// observer, this extension will subscribe to their <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event, and invoke a given set of callbacks whenever a property changes.
    /// </summary>
    public class PropertyChangedExtension : ObserverExtension
    {
        private readonly List<Action<PropertyChangeNotification>> _subscribers = new List<Action<PropertyChangeNotification>>();
        private PropertyChangedEventHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedExtension"/> class.
        /// </summary>
        public PropertyChangedExtension()
        {
            _handler = ItemPropertyChanged;
        }

        /// <summary>
        /// Specifies that weak events should be used. This allows the observer to be garbage collected even 
        /// if the events are still subscribed to on child items. This feature is NOT enabled by default.
        /// </summary>
        /// <returns>This instance, for fluent interfaces.</returns>
        public PropertyChangedExtension UseWeakEvents()
        {
            AssertNotConfiguredYet("Cannot change weak event settings as this extension has already been configured.");
            _handler = new WeakEventHandler<PropertyChangedEventArgs>(ItemPropertyChanged).WeakHandler;
            return this;
        }

        /// <summary>
        /// Sets up a callback that will be invoked whenever a property changes on any object.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public PropertyChangedExtension WhenPropertyChanges(Action<PropertyChangeNotification> callback)
        {
            return WhenPropertyChanges<object>(x => callback(new PropertyChangeNotification(x.Source, x.PropertyName)));
        }

        /// <summary>
        /// Sets up a callback that will be invoked whenever a property changes on any object derived from 
        /// the specified.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>This instance, for fluent interfaces.</returns>
        public PropertyChangedExtension WhenPropertyChanges<TObject>(Action<PropertyChangeNotification<TObject>> callback)
        {
            Guard.ArgumentNotNull(callback, "callback");
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
            var notifiable = attachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged += _handler;
        }

        /// <summary>
        /// Notifies this extension that an item has been removed from the current observer.
        /// </summary>
        /// <param name="detachedItem">The detached item that was just removed from the observer.</param>
        protected override void Detach(object detachedItem)
        {
            var notifiable = detachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged -= _handler;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var callbacks = _subscribers.ToArray();
            if (callbacks.Length == 0) 
                return;

            var notification = new PropertyChangeNotification(sender, e.PropertyName);
            foreach (var item in callbacks)
            {
                item(notification);
            }
        }
    }
}