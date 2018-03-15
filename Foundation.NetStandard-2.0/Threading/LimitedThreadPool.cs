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
            this.availableThreadCount = maxThreadCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 QueuedItemCount
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 AvailableThreadCount
        {
            get
            {
                return this.availableThreadCount;
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
            if (this.availableThreadCount > 0)
            {
                Interlocked.Decrement(ref this.availableThreadCount);
                succeeded = ThreadPool.QueueUserWorkItem(this.Callback, item);
            }
            else
            {
                lock (this.queue)
                {
                    this.queue.Enqueue(item);
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
            if (this.queue.Count > 0 && this.availableThreadCount > 0)
            {
                lock (this.queue)
                {
                    while (this.queue.Count > 0 && this.availableThreadCount > 0)
                    {
                        var item = this.queue.Dequeue();
                        Boolean succeeded = ThreadPool.QueueUserWorkItem(this.Callback, item);
                        Interlocked.Decrement(ref this.availableThreadCount);
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
                Interlocked.Increment(ref this.availableThreadCount);
                this.Pulse();
            }
        }
    }
}