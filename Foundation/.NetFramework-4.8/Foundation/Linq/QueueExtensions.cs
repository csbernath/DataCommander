using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static T DequeueTail<T>(this Queue<T> queue)
        {
            Assert.IsNotNull(queue);
            Assert.IsTrue(queue.Count > 0);

            var array = new T[queue.Count];
            queue.CopyTo(array, 0);
            queue.Clear();
            var last = array.Length - 1;
            for (var i = 0; i < last; i++)
                queue.Enqueue(array[i]);

            return array[last];
        }
    }
}