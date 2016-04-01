namespace DataCommander.Providers
{
    using System;

    internal sealed class ListBoxItem<T>
    {
        private readonly Func<T, string> toString;

        public ListBoxItem(T item, Func<T, string> toString)
        {
            this.Item = item;
            this.toString = toString;
        }

        public T Item { get; }

        public override string ToString()
        {
            return this.toString(this.Item);
        }
    }
}