namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(queue != null);
            Contract.Requires<ArgumentException>(queue.Count > 0);

            var array = new T[queue.Count];
            queue.CopyTo(array, 0);
            queue.Clear();
            int last = array.Length - 1;
            for (int i = 0; i < last; i++)
            {
                queue.Enqueue(array[i]);
            }

            return array[last];
        }
    }
}