using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Observal.Eventing;

namespace Observal
{
    public class ObservableCollectionTracker
    {
        private readonly Action<object> _itemAdded;
        private readonly Action<object> _itemRemoved;
        private readonly HashSet<object> _items = new HashSet<object>();
        private INotifyCollectionChanged _source;
        private IEnumerable<object> _sourceEnumerable;
        private readonly object _lock = new object();
        private readonly NotifyCollectionChangedEventHandler _handler;

        public ObservableCollectionTracker(Action<object> itemAdded, Action<object> itemRemoved) : this(itemAdded, itemRemoved, false)
        {
        }

        public ObservableCollectionTracker(Action<object> itemAdded, Action<object> itemRemoved, bool useWeakEvents)
        {
            _itemAdded = itemAdded ?? (x => { });
            _itemRemoved = itemRemoved ?? (x => { });
            
            if (useWeakEvents) _handler = new WeakEventHandler<NotifyCollectionChangedEventArgs>(CollectionChanged).WeakHandler;
            else _handler = CollectionChanged;
        }

        public void Attach(INotifyCollectionChanged collection)
        {
            if (collection == null)
                return;

            lock (_lock)
            {
                if (_source != null)
                {
                    throw new InvalidOperationException("This collection tracker has already been attached.");
                }

                _source = collection;
                _sourceEnumerable = ((IEnumerable) _source).Cast<object>();
                _source.CollectionChanged += _handler;
            }

            var items = _sourceEnumerable.ToList();
            var itemsAdded = new List<object>();
            lock (_lock)
            {
                itemsAdded.AddRange(items.Where(item => _items.Add(item)));
            }

            foreach (var item in itemsAdded)
            {
                _itemAdded(item);
            }
        }

        public void Detach()
        {
            lock (_lock)
            {
                if (_source == null) 
                    return;

                _source.CollectionChanged -= _handler;
                _source = null;
            }
        }

        public ReadOnlyCollection<object> Items
        {
            get { return _items.ToList().AsReadOnly(); }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newlyAdded = new List<object>();
            var newlyRemoved = new List<object>();

            var added = (e.NewItems ?? new List<object>()).OfType<object>().ToList();
            var removed = (e.OldItems ?? new List<object>()).OfType<object>().ToList();

            lock (_lock)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in added)
                        {
                            if (_items.Contains(item)) 
                                continue;
                            newlyAdded.Add(item);
                            _items.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in removed)
                        {
                            if (!_items.Contains(item))
                                continue;
                            newlyRemoved.Add(item);
                            _items.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var item in removed)
                        {
                            if (!_items.Contains(item))
                                continue;
                            newlyRemoved.Add(item);
                            _items.Remove(item);
                        }
                        foreach (var item in added)
                        {
                            if (_items.Contains(item))
                                continue;
                            newlyAdded.Add(item);
                            _items.Add(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        var cache = _items.ToList();
                        var current = _sourceEnumerable.ToList();

                        added = current.Except(cache).ToList();
                        removed = cache.Except(current).ToList();
                        foreach (var item in added)
                        {
                            _items.Add(item);
                            newlyAdded.Add(item);
                        }
                        foreach (var item in removed)
                        {
                            _items.Remove(item);
                            newlyRemoved.Add(item);
                        }
                        break;
                }
            }

            foreach (var item in newlyRemoved) _itemRemoved(item);
            foreach (var item in newlyAdded) _itemAdded(item);
        }
    }
}