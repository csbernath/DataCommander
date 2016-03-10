namespace DataCommander.Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedItemAndPreviousItem<T>
    {
        private readonly T previousItem;
        private readonly IndexedItem<T> currentItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousItem"></param>
        /// <param name="currentItem"></param>
        public IndexedItemAndPreviousItem(T previousItem, IndexedItem<T> currentItem)
        {
            this.previousItem = previousItem;
            this.currentItem = currentItem;
        }

        /// <summary>
        /// 
        /// </summary>
        public T PreviousItem => this.previousItem;

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> CurrentItem => this.currentItem;
    }
}