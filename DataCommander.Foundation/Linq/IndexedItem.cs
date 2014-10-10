using System;

namespace DataCommander.Foundation.Linq
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
        public static IndexedItem<T> Create<T>( Int32 index, T value )
        {
            return new IndexedItem<T>( index, value );
        }
    }
}