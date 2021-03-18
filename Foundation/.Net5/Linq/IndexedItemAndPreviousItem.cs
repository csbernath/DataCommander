namespace Foundation.Linq
{
    public sealed class IndexedItemAndPreviousItem<T>
    {
        public readonly T PreviousItem;
        public readonly IndexedItem<T> CurrentItem;

        public IndexedItemAndPreviousItem(T previousItem, IndexedItem<T> currentItem)
        {
            PreviousItem = previousItem;
            CurrentItem = currentItem;
        }
    }
}