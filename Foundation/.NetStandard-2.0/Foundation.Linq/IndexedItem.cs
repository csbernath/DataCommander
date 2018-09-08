namespace Foundation.Linq
{
    public static class IndexedItem
    {
        public static IndexedItem<T> Create<T>(int index, T value) => new IndexedItem<T>(index, value);
    }
}