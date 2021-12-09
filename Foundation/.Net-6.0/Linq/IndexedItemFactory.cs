namespace Foundation.Linq;

public static class IndexedItemFactory
{
    public static IndexedItem<T> Create<T>(int index, T value) => new(index, value);
}