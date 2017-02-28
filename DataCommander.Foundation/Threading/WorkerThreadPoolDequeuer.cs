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
        private readonly WaitCallback callback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public WorkerThreadPoolDequeuer(WaitCallback callback)
        {
            this.callback = callback;
            this.Thread = new WorkerThread(this.Start);
        }

        private void Start()
        {
            WaitHandle[] waitHandles = { this.Thread.StopRequest, this.pool.EnqueueEvent };

            while (!this.Thread.IsStopRequested)
            {
                var dequeued = this.pool.Dequeue(this.callback, waitHandles);

                if (dequeued)
                {
                    this.LastActivityTimestamp = Stopwatch.GetTimestamp();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread { get; }

        internal WorkerThreadPool Pool
        {
            set
            {
                this.pool = value;
            }
        }

        internal long LastActivityTimestamp { get; private set; }
    }
}