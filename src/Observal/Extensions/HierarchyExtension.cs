using System;
using System.Collections.Generic;

namespace Observal.Extensions
{
    public class HierarchyExtension : ObserverExtension
    {
        private readonly List<Tuple<Type, Func<object, object>>> _childrenReaders = new List<Tuple<Type, Func<object, object>>>();

        public HierarchyExtension AddChildren<T>(Func<T, object> childrenSelector)
        {
            _childrenReaders.Add(new Tuple<Type, Func<object, object>>(typeof(T), x => childrenSelector((T)x)));
            return this;
        }

        protected override void Attach(Observer observer, object attachedItem)
        {
            foreach (var reader in _childrenReaders)
            {
                var type = reader.Item1;
                var callback = reader.Item2;

                if (type.IsAssignableFrom(attachedItem.GetType()))
                {
                    var children = callback(attachedItem);
                    if (children != null)
                    {
                        observer.Add(children);
                    }
                }
            }
        }

        protected override void Detach(Observer observer, object detachedItem)
        {
            foreach (var reader in _childrenReaders)
            {
                var type = reader.Item1;
                var callback = reader.Item2;

                if (type.IsAssignableFrom(detachedItem.GetType()))
                {
                    var children = callback(detachedItem);
                    if (children != null)
                    {
                        observer.Release(children);
                    }
                }
            }
        }
    }
}