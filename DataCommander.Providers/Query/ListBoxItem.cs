namespace DataCommander.Providers
{
    using System;

    internal sealed class ListBoxItem<T>
    {
        private readonly T item;
        private readonly Func<T, string> toString;

        public ListBoxItem(T item, Func<T, string> toString)
        {
            this.item = item;
            this.toString = toString;
        }

        public T Item => this.item;

        public override string ToString()
        {
            return this.toString(this.item);
        }
    }
}