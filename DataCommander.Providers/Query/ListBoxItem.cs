using System;

namespace DataCommander.Providers
{
    internal sealed class ListBoxItem<T>
    {
        private T item;
        private Func<T, string> toString;

        public ListBoxItem(T item, Func<T, string> toString)
        {
            this.item = item;
            this.toString = toString;
        }

        public T Item
        {
            get
            {
                return item;
            }
        }

        public override string ToString()
        {
            return this.toString(this.item);
        }
    }
}