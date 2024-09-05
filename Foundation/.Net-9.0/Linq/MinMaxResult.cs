namespace Foundation.Linq;

public sealed class MinMaxResult<T>(int count, int whereCount, IndexedItem<T> min, IndexedItem<T> max)
{
    public readonly int Count = count;
    public readonly int WhereCount = whereCount;
    public readonly IndexedItem<T> Min = min;
    public readonly IndexedItem<T> Max = max;
}