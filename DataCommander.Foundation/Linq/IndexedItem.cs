namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class IndexedItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IndexedItem<T> Create<T>(int index, T value)
        {
            return new IndexedItem<T>(index, value);
        }
    }
}