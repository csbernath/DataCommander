using System.Collections.Concurrent;
using Foundation.Assertions;

namespace Foundation.Linq
{
    public static class ProducerConsumerCollectionExtensions
    {
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