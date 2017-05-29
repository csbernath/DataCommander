namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedItemAndPreviousItem<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousItem"></param>
        /// <param name="currentItem"></param>
        public IndexedItemAndPreviousItem(T previousItem, IndexedItem<T> currentItem)
        {
            this.PreviousItem = previousItem;
            this.CurrentItem = currentItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public T PreviousItem { get; }

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> CurrentItem { get; }
    }
}