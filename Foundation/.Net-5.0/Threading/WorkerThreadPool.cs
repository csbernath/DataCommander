using System.Collections.Generic;
using System.Threading;

namespace Foundation.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkerThreadPool
    {
        private readonly Queue<object> _queue = new Queue<object>();
        private int _activeThreadCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreadCount"></param>
        public WorkerThreadPool(int maxThreadCount)
        {
            MaxThreadCount = maxThreadCount;
            Dequeuers = new WorkerThreadPoolDequeuerCollection(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void QueueUserWorkItem(object item)
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }

            EnqueueEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => _queue.Count;

        internal bool Dequeue(WaitCallback callback, WaitHandle[] waitHandles)
        {
            bool dequeued;
            object item = null;

            if (_queue.Count > 0)
            {
                lock (_queue)
                {
                    if (_queue.Count > 0)
                    {
                        item = _queue.Dequeue();
                    }
                }
            }

            if (item != null)
            {
                Interlocked.Increment(ref _activeThreadCount);
                callback(item);
                Interlocked.Decrement(ref _activeThreadCount);
                dequeued = true;
            }
            else
            {
                WaitHandle.WaitAny(waitHandles, 100, false);
                dequeued = false;
            }

            return dequeued;
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThreadPoolDequeuerCollection Dequeuers { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ActiveThreadCount => _activeThreadCount;

        internal AutoResetEvent EnqueueEvent { get; } = new AutoResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        public int MaxThreadCount { get; }
    }
}