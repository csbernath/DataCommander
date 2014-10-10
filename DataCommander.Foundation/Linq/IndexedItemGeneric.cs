namespace DataCommander.Foundation.Linq
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class IndexedItem<T>
    {
        private readonly Int32 index;
        private readonly T value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public IndexedItem( Int32 index, T value )
        {
            this.index = index;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }
    }
}