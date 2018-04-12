using System.Collections.Concurrent;
using Foundation.Assertions;

namespace Foundation.Linq
{
#if FOUNDATION_3_5
    using Foundation.Collections;
#else
#endif

    /// <summary>
    /// 
    /// </summary>
    public static class ProducerConsumerCollectionExtensions
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
            Assert.IsNotNull(collection);
            Assert.IsNotNull(target);

            var i = 0;
            while (i < target.Length)
            {
                T item;
                var succeeded = collection.TryTake(out item);
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