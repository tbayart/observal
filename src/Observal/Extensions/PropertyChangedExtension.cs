using System.Collections.Generic;
using System.ComponentModel;
using Observal.Eventing;

namespace Observal.Extensions
{
    public class PropertyChangedExtension : IObserverExtension, INotifyPropertyChanged
    {
        private WeakEventHandler<PropertyChangedEventArgs> _handler;

        public PropertyChangedExtension()
        {
            _handler = new WeakEventHandler<PropertyChangedEventArgs>(ItemPropertyChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Configure(Observer observer)
        {
            
        }

        public void Attach(Observer observer, object attachedItem)
        {
            var notifiable = attachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged += _handler.WeakHandler;
        }

        public void Detach(Observer observer, object detachedItem)
        {
            var notifiable = detachedItem as INotifyPropertyChanged;
            if (notifiable == null) return;

            notifiable.PropertyChanged -= _handler.WeakHandler;
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(sender, e);
        }
    }
}