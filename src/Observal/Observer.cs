using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Observal.Extensions;
using Observal.Utilities;

namespace Observal
{
    /// <summary>
    /// The core Observal kernel, which monitors objects for changes. You can add capabilities to this observer using 
    /// extensions, such as the <see cref="PropertyChangedExtension"/> for monitoring property changed events, or 
    /// the <see cref="HierarchyExtension"/> for expanding items into child items.
    /// </summary>
    public class Observer
    {
        private readonly HashSet<IObserverExtension> _extensions = new HashSet<IObserverExtension>();
        private readonly ReferenceCountingHashSet _items = new ReferenceCountingHashSet();
        private readonly object _lock = new object();
        private bool _isFrozen;
        private bool _isConfiguring;

        /// <summary>
        /// Determines whether this observer has a given extension as indicated by a filter callback.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// 	<c>true</c> if the specified filter has extension; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExtension(Func<IObserverExtension, bool> filter)
        {
            lock (_lock)
            {
                return _extensions.Any(filter);
            }
        }

        /// <summary>
        /// Adds an extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public void AddExtension(IObserverExtension extension)
        {
            lock (_lock)
            {
                if (_isFrozen)
                {
                    throw new InvalidOperationException();
                }
                _extensions.Add(extension);
            }
        }

        /// <summary>
        /// Adds an item to the list of objects observed by this observer. If the item is already being observed, 
        /// the reference count will be incremented. When released, the reference count is decremented until 0.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(object item)
        {
            EnsureExtensionsConfigured();
            List<IObserverExtension> extensions;

            lock (_lock)
            {
                var added = _items.Add(item);
                if (!added)
                    return;

                extensions = _extensions.ToList();
            }

            foreach (var extension in extensions)
            {
                extension.Attach(this, item);
            }
        }

        private void EnsureExtensionsConfigured()
        {
            lock (_lock)
            {
                if (_isConfiguring)
                {
                    throw new InvalidOperationException("Items cannot be added or removed while configuring");
                }

                if (_isFrozen)
                {
                    return;
                }

                _isConfiguring = true;

                var configuredExtensions = new List<IObserverExtension>();
                while (_extensions.Count > configuredExtensions.Count)
                {
                    foreach (var extension in _extensions.Where(extension => !configuredExtensions.Contains(extension)).ToList())
                    {
                        configuredExtensions.Add(extension);
                        extension.Configure(this);
                    }
                }

                _isConfiguring = false;
                _isFrozen = true;
            }
        }

        /// <summary>
        /// Removes an item from the collection, by decrementing the reference count. When the reference count 
        /// reaches zero, the item will be removed from the collection and should be garbage collectable.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Release(object item)
        {
            EnsureExtensionsConfigured();
            List<IObserverExtension> extensions;

            lock (_lock)
            {
                var removed = _items.Remove(item);
                if (!removed)
                    return;

                extensions = _extensions.ToList();
            }

            foreach (var extension in extensions)
            {
                extension.Detach(this, item);
            }
        }

        /// <summary>
        /// Gets a snapshot of all elements currently in the observed collection list.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<object> GetAll()
        {
            return _items.GetAll();
        }
    }
}