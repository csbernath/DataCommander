namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MinMaxResult<T>
    {
        #region Private Fields

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="whereCount"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public MinMaxResult(
            int count,
            int whereCount,
            IndexedItem<T> min,
            IndexedItem<T> max)
        {
            Count = count;
            WhereCount = whereCount;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        public int WhereCount { get; }

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> Min { get; }

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> Max { get; }
    }
}