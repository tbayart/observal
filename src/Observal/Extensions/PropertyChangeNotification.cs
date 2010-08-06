namespace Observal.Extensions
{
    /// <summary>
    /// A payload for the WhenPropertyChanged callback of the <see cref="PropertyChangedExtension"/>.
    /// </summary>
    public class PropertyChangeNotification : PropertyChangeNotification<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeNotification"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyChangeNotification(object source, string propertyName)
            : base(source, propertyName)
        {
        }
    }

    /// <summary>
    /// A payload for the WhenPropertyChanged callback of the <see cref="PropertyChangedExtension"/>.
    /// </summary>
    /// <typeparam name="TElement">The type of object that the property was changed on.</typeparam>
    public class PropertyChangeNotification<TElement>
    {
        private readonly TElement _source;
        private readonly string _propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeNotification&lt;TElement&gt;"/> class.
        /// </summary>
        /// <param name="source">The source object that the PropertyChanged event was raised by.</param>
        /// <param name="propertyName">Name of the property that was changed.</param>
        public PropertyChangeNotification(TElement source, string propertyName)
        {
            _source = source;
            _propertyName = propertyName;
        }

        /// <summary>
        /// Gets the source object that the PropertyChanged event was raised by.
        /// </summary>
        /// <value>The source.</value>
        public TElement Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets the name of the property that was changed.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}