namespace DataCommander.Foundation.Linq
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MinMaxResult<T>
    {
        #region Private Fields

        private readonly int count;
        private readonly int whereCount;
        private readonly IndexedItem<T> min;
        private readonly IndexedItem<T> max;

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
            this.count = count;
            this.whereCount = whereCount;
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int WhereCount
        {
            get
            {
                return this.whereCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> Min
        {
            get
            {
                return this.min;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IndexedItem<T> Max
        {
            get
            {
                return this.max;
            }
        }
    }
}