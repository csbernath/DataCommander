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
        private Int32 _maxThreadCount;
        private Int32 _availableThreadCount;
        private Queue<Tuple<Action<T>, T>> _queue = new Queue<Tuple<Action<T>, T>>();

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
            this._maxThreadCount = maxThreadCount;
            _availableThreadCount = maxThreadCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 QueuedItemCount
        {
            get
            {
                return _queue.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 AvailableThreadCount
        {
            get
            {
                return _availableThreadCount;
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
            if (_availableThreadCount > 0)
            {
                Interlocked.Decrement(ref _availableThreadCount);
                succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
            }
            else
            {
                lock (_queue)
                {
                    _queue.Enqueue(item);
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
            if (_queue.Count > 0 && _availableThreadCount > 0)
            {
                lock (_queue)
                {
                    while (_queue.Count > 0 && _availableThreadCount > 0)
                    {
                        var item = _queue.Dequeue();
                        Boolean succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
                        Interlocked.Decrement(ref _availableThreadCount);
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
                Interlocked.Increment(ref _availableThreadCount);
                Pulse();
            }
        }
    }
}