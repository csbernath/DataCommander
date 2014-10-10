namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class WorkerThreadPool
    {
        private readonly Queue<Object> queue = new Queue<Object>();
        private readonly AutoResetEvent enqueueEvent = new AutoResetEvent( false );
        private readonly WorkerThreadPoolDequeuerCollection dequeuers;
        private Int32 maxThreadCount;
        private Int32 activeThreadCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreadCount"></param>
        public WorkerThreadPool( Int32 maxThreadCount )
        {
            this.maxThreadCount = maxThreadCount;
            this.dequeuers = new WorkerThreadPoolDequeuerCollection( this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void QueueUserWorkItem( Object item )
        {
            lock (this.queue)
            {
                this.queue.Enqueue( item );
            }

            this.enqueueEvent.Set();
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

        internal Boolean Dequeue( WaitCallback callback, WaitHandle[] waitHandles )
        {
            Boolean dequeued;
            Object item = null;

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
        public WorkerThreadPoolDequeuerCollection Dequeuers
        {
            get
            {
                return this.dequeuers;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ActiveThreadCount
        {
            get
            {
                return this.activeThreadCount;
            }
        }

        internal AutoResetEvent EnqueueEvent
        {
            get
            {
                return this.enqueueEvent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MaxThreadCount
        {
            get
            {
                return this.maxThreadCount;
            }
        }
    }
}