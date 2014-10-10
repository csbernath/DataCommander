namespace DataCommander.Foundation.Linq
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MinMaxResult<T>
    {
        private readonly Int32 count;
        private readonly Int32 whereCount;
        private readonly IndexedItem<T> min;
        private IndexedItem<T> max;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="whereCount"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public MinMaxResult(
            Int32 count,
            Int32 whereCount,
            IndexedItem<T> min,
            IndexedItem<T> max )
        {
            this.count = count;
            this.whereCount = whereCount;
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 WhereCount
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
