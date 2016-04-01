namespace DataCommander.Foundation.Threading
{
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class WorkerThreadPool
    {
        private readonly Queue<object> queue = new Queue<object>();
        private int activeThreadCount;

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
            lock (this.queue)
            {
                this.queue.Enqueue( item );
            }

            this.EnqueueEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => this.queue.Count;

        internal bool Dequeue( WaitCallback callback, WaitHandle[] waitHandles )
        {
            bool dequeued;
            object item = null;

            if (this.queue.Count > 0)
            {
                lock (this.queue)
                {
                    if (this.queue.Count > 0)
                    {
                        item = this.queue.Dequeue();
                    }
                }
            }

            if (item != null)
            {
                Interlocked.Increment( ref this.activeThreadCount );
                callback( item );
                Interlocked.Decrement( ref this.activeThreadCount );
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
        public int ActiveThreadCount => this.activeThreadCount;

        internal AutoResetEvent EnqueueEvent { get; } = new AutoResetEvent( false );

        /// <summary>
        /// 
        /// </summary>
        public int MaxThreadCount { get; }
    }
}