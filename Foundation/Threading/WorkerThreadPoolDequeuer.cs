using System.Diagnostics;
using System.Threading;

namespace Foundation.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkerThreadPoolDequeuer
    {
        private WorkerThreadPool _pool;
        private readonly WaitCallback _callback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public WorkerThreadPoolDequeuer(WaitCallback callback)
        {
            this._callback = callback;
            this.Thread = new WorkerThread(this.Start);
        }

        private void Start()
        {
            WaitHandle[] waitHandles = { this.Thread.StopRequest, this._pool.EnqueueEvent };

            while (!this.Thread.IsStopRequested)
            {
                var dequeued = this._pool.Dequeue(this._callback, waitHandles);

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
            set => this._pool = value;
        }

        internal long LastActivityTimestamp { get; private set; }
    }
}