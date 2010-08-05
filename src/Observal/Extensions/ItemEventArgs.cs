using System;

namespace Observal.Extensions
{
    public class ItemEventArgs : EventArgs
    {
        private readonly object _item;

        public ItemEventArgs(object item)
        {
            _item = item;
        }

        public object Item
        {
            get { return _item; }
        }
    }
}