namespace Foundation.Linq
{
    public struct IndexedItem<T>
    {
        public readonly int Index;
        public readonly T Value;

        public IndexedItem(int index, T value)
        {
            Index = index;
            Value = value;
        }
    }
}