namespace DataCommander.Foundation.Threading
{
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class WorkerThreadPoolDequeuer
    {
        private WorkerThreadPool pool;
        private readonly WorkerThread thread;
        private readonly WaitCallback callback;
        private long lastActivityTimestamp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public WorkerThreadPoolDequeuer(WaitCallback callback)
        {
            this.callback = callback;
            this.thread = new WorkerThread(this.Start);
        }

        private void Start()
        {
            WaitHandle[] waitHandles = { this.thread.StopRequest, this.pool.EnqueueEvent };

            while (!this.thread.IsStopRequested)
            {
                bool dequeued = this.pool.Dequeue(this.callback, waitHandles);

                if (dequeued)
                {
                    this.lastActivityTimestamp = Stopwatch.GetTimestamp();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread
        {
            get
            {
                return this.thread;
            }
        }

        internal WorkerThreadPool Pool
        {
            set
            {
                this.pool = value;
            }
        }

        internal long LastActivityTimestamp
        {
            get
            {
                return this.lastActivityTimestamp;
            }
        }
    }
}