namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
#if FOUNDATION_3_5
    using DataCommander.Foundation.Collections;
#else
    using System.Collections.Concurrent;
#endif

    /// <summary>
    /// 
    /// </summary>
    public static class IProducerConsumerCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool TryAddRange<T>( this IProducerConsumerCollection<T> collection, IEnumerable<T> items )
        {
            Contract.Requires( collection != null );

            bool allSucceeded = true;
            if (items != null)
            {
                foreach (var item in items)
                {
                    bool succeeded = collection.TryAdd( item );
                    allSucceeded &= succeeded;
                }
            }

            return allSucceeded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Int32 Take<T>( this IProducerConsumerCollection<T> collection, T[] target )
        {
            Contract.Requires( collection != null );
            Contract.Requires( target != null );

            Int32 i = 0;
            while (i < target.Length)
            {
                T item;
                bool succeeded = collection.TryTake( out item );
                if (succeeded)
                {
                    target[ i ] = item;
                    i++;
                }
                else
                {
                    break;
                }
            }
            return i;
        }
    }
}