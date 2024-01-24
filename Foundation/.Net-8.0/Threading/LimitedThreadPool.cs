using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics.Contracts;

namespace Foundation.Threading
{
    public sealed class LimitedThreadPool<T>
    {
        private int _maxThreadCount;
        private int _availableThreadCount;
        private readonly Queue<Tuple<Action<T>, T>> _queue = new();

        public LimitedThreadPool(int maxThreadCount)
        {
            _maxThreadCount = maxThreadCount;
            _availableThreadCount = maxThreadCount;
        }

        public int QueuedItemCount => _queue.Count;

        public int AvailableThreadCount => _availableThreadCount;

        public bool QueueUserWorkItem(Action<T> waitCallback, T state)
        {
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