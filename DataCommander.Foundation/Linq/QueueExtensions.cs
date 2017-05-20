namespace DataCommander.Foundation.Linq
{
    using System.Collections.Generic;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(queue != null);
            Contract.Requires<ArgumentException>(queue.Count > 0);
#endif

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