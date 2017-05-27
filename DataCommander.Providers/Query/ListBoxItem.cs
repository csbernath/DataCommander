using System;

namespace DataCommander.Providers.Query
{
    internal sealed class ListBoxItem<T>
    {
        private readonly Func<T, string> _toString;

        public ListBoxItem(T item, Func<T, string> toString)
        {
            Item = item;
            _toString = toString;
        }

        public T Item { get; }

        public override string ToString()
        {
            return _toString(Item);
        }
    }
}