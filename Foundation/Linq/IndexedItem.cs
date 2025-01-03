namespace Foundation.Linq;

public readonly struct IndexedItem<T>(int index, T value)
{
    public readonly int Index = index;
    public readonly T Value = value;
}