using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Observal.Utilities
{
    public class ReferenceCountingHashSet
    {
        private readonly Dictionary<Tuple<Type, int>, Reference> _items = new Dictionary<Tuple<Type, int>, Reference>();
        private readonly object _lock = new object();

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _items.Count;
                }
            }
        }

        public bool Add(object item)
        {
            if (item == null) 
                return false;

            var hash = Hash(item);
            lock (_lock)
            {
                if (_items.ContainsKey(hash))
                {
                    _items[hash].Increment();
                    return false;
                }
                _items.Add(hash, new Reference(item));
            }
            return true;
        }

        public bool Remove(object item)
        {
            if (item == null)
                return false;

            var hash = Hash(item);
            lock (_lock)
            {
                if (!_items.ContainsKey(hash))
                {
                    return false;
                }

                var reference = _items[hash];
                reference.Decrement();
                if (reference.ReferenceCount <= 0)
                {
                    _items.Remove(hash);
                    return true;                    
                }
                return false;
            }
        }

        public bool Contains(object item)
        {
            if (item == null)
                return false;

            var hash = Hash(item);
            lock (_lock)
            {
                return _items.ContainsKey(hash);
            }
        }

        public ReadOnlyCollection<object> GetAll()
        {
            lock (_lock)
            {
                return _items.Select(x => x.Value.Item).Where(x => x != null).ToList().AsReadOnly();
            }
        }

        private static Tuple<Type, int> Hash(object item)
        {
            return Tuple.Create(item.GetType(), item.GetHashCode());
        }

        private class Reference
        {
            private readonly object _item;
            private int _referenceCount;

            public Reference(object item)
            {
                _item = item;
                _referenceCount = 1;
            }

            public object Item
            {
                get { return _item; }
            }

            public int ReferenceCount
            {
                get { return _referenceCount; }
            }

            public void Increment()
            {
                _referenceCount++;
            }

            public void Decrement()
            {
                _referenceCount--;
            }
        }
    }
}