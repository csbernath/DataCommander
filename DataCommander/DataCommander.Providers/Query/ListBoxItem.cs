using System;

namespace DataCommander.Providers.Query
{
    internal sealed class ListBoxItem<T>
    {
        private readonly Func<T, string> _toString;

        public readonly T Item;

        public ListBoxItem(T item, Func<T, string> toString)
        {
            Item = item;
            _toString = toString;
        }

        public override string ToString() => _toString(Item);
    }
}