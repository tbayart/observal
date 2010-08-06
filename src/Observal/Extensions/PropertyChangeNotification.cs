namespace Observal.Extensions
{
    public class PropertyChangeNotification : PropertyChangeNotification<object>
    {
        public PropertyChangeNotification(object source, string propertyName)
            : base(source, propertyName)
        {
        }
    }

    public class PropertyChangeNotification<T>
    {
        private readonly object _source;
        private readonly string _propertyName;

        public PropertyChangeNotification(object source, string propertyName)
        {
            _source = source;
            _propertyName = propertyName;
        }

        public object Source
        {
            get { return _source; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}