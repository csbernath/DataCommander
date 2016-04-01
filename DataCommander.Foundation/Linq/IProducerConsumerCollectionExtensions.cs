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
        /// <param name="target"></param>
        /// <returns></returns>
        public static int Take<T>(this IProducerConsumerCollection<T> collection, T[] target)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(target != null);

            int i = 0;
            while (i < target.Length)
            {
                T item;
                bool succeeded = collection.TryTake(out item);
                if (succeeded)
                {
                    target[i] = item;
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