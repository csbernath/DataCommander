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
        public WorkerThreadPool( int maxThreadCount )
        {
            this.MaxThreadCount = maxThreadCount;
            this.Dequeuers = new WorkerThreadPoolDequeuerCollection( this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void QueueUserWorkItem( object item )
        {
            lock (this._queue)
            {
                this._queue.Enqueue( item );
            }

            this.EnqueueEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => this._queue.Count;

        internal bool Dequeue( WaitCallback callback, WaitHandle[] waitHandles )
        {
            bool dequeued;
            object item = null;

            if (this._queue.Count > 0)
            {
                lock (this._queue)
                {
                    if (this._queue.Count > 0)
                    {
                        item = this._queue.Dequeue();
                    }
                }
            }

            if (item != null)
            {
                Interlocked.Increment( ref this._activeThreadCount );
                callback( item );
                Interlocked.Decrement( ref this._activeThreadCount );
                dequeued = true;
            }
            else
            {
                WaitHandle.WaitAny( waitHandles, 100, false );
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
        public int ActiveThreadCount => this._activeThreadCount;

        internal AutoResetEvent EnqueueEvent { get; } = new AutoResetEvent( false );

        /// <summary>
        /// 
        /// </summary>
        public int MaxThreadCount { get; }
    }
}