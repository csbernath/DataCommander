namespace Foundation.Linq
{
    public sealed class MinMaxResult<T>
    {
        public readonly int Count;
        public readonly int WhereCount;
        public readonly IndexedItem<T> Min;
        public readonly IndexedItem<T> Max;

        public MinMaxResult(int count, int whereCount, IndexedItem<T> min, IndexedItem<T> max)
        {
            Count = count;
            WhereCount = whereCount;
            Min = min;
            Max = max;
        }
    }
}