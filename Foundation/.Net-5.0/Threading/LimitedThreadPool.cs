using System;
using System.Collections.Generic;
using System.Threading;
#if FOUNDATION_2_0 || FOUNDATION_3_5
    using Foundation.Diagnostics;
#else
using System.Diagnostics.Contracts;

#endif

namespace Foundation.Threading
{
    public sealed class LimitedThreadPool<T>
    {
        private int _maxThreadCount;
        private int _availableThreadCount;
        private readonly Queue<Tuple<Action<T>, T>> _queue = new Queue<Tuple<Action<T>, T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreadCount"></param>
        public LimitedThreadPool(int maxThreadCount)
        {
#if FOUNDATION_2_0 || FOUNDATION_3_5
            Assert.Compare<Int32>( maxThreadCount, Comparers.GreaterThan, 0, "maxThreadCount", null );
#else
            Contract.Requires(maxThreadCount > 0);
#endif
            _maxThreadCount = maxThreadCount;
            _availableThreadCount = maxThreadCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => _queue.Count;

        /// <summary>
        /// 
        /// </summary>
        public int AvailableThreadCount => _availableThreadCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool QueueUserWorkItem(Action<T> waitCallback, T state)
        {
#if FOUNDATION_2_0 || FOUNDATION_3_5
            Assert.IsNotNull(waitCallback, "waitCallback");           
#else
            Contract.Requires(waitCallback != null);
#endif
            var item = Tuple.Create(waitCallback, state);
            bool succeeded;
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
                        var succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
                        Interlocked.Decrement(ref _availableThreadCount);
                    }
                }
            }
        }

        private void Callback(object stateObject)
        {
            try
            {
                var item = (Tuple<Action<T>, T>)stateObject;
                var waitCallback = item.Item1;
                var state = item.Item2;
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