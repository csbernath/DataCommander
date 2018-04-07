namespace Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
#if FOUNDATION_2_0 || FOUNDATION_3_5
    using Foundation.Diagnostics;
#else
    using System.Diagnostics.Contracts;
#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class LimitedThreadPool<T>
    {
        private Int32 maxThreadCount;
        private Int32 availableThreadCount;
        private Queue<Tuple<Action<T>, T>> queue = new Queue<Tuple<Action<T>, T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreadCount"></param>
        public LimitedThreadPool(Int32 maxThreadCount)
        {
#if FOUNDATION_2_0 || FOUNDATION_3_5
            Assert.Compare<Int32>( maxThreadCount, Comparers.GreaterThan, 0, "maxThreadCount", null );
#else
            Contract.Requires(maxThreadCount > 0);
#endif
            this.maxThreadCount = maxThreadCount;
            availableThreadCount = maxThreadCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 QueuedItemCount
        {
            get
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 AvailableThreadCount
        {
            get
            {
                return availableThreadCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Boolean QueueUserWorkItem(Action<T> waitCallback, T state)
        {
#if FOUNDATION_2_0 || FOUNDATION_3_5
            Assert.IsNotNull(waitCallback, "waitCallback");           
#else
            Contract.Requires(waitCallback != null);
#endif
            var item = Tuple.Create(waitCallback, state);
            Boolean succeeded;
            if (availableThreadCount > 0)
            {
                Interlocked.Decrement(ref availableThreadCount);
                succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
            }
            else
            {
                lock (queue)
                {
                    queue.Enqueue(item);
                }

                succeeded = true;
            }

            return succeeded;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pulse()
        {
            if (queue.Count > 0 && availableThreadCount > 0)
            {
                lock (queue)
                {
                    while (queue.Count > 0 && availableThreadCount > 0)
                    {
                        var item = queue.Dequeue();
                        Boolean succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
                        Interlocked.Decrement(ref availableThreadCount);
                    }
                }
            }
        }

        private void Callback(Object stateObject)
        {
            try
            {
                var item = (Tuple<Action<T>, T>)stateObject;
                Action<T> waitCallback = item.Item1;
                T state = item.Item2;
                waitCallback(state);
            }
            finally
            {
                Interlocked.Increment(ref availableThreadCount);
                Pulse();
            }
        }
    }
}