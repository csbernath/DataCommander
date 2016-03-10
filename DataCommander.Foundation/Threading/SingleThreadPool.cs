namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SingleThreadPool
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly WorkerThread thread;
        private readonly Queue<Tuple<WaitCallback, object>> workItems = new Queue<Tuple<WaitCallback, object>>();
        private readonly EventWaitHandle enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        private int queuedItemCount;

        /// <summary>
        /// 
        /// </summary>
        public SingleThreadPool()
        {
            this.thread = new WorkerThread(this.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread => this.thread;

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => this.queuedItemCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        public void QueueUserWorkItem(WaitCallback callback, object state)
        {
            Contract.Requires(callback != null);

            var tuple = Tuple.Create(callback, state);

            lock (this.workItems)
            {
                this.workItems.Enqueue(tuple);
            }

            Interlocked.Increment(ref this.queuedItemCount);
            this.enqueueEvent.Set();
        }

        private void Dequeue()
        {
            Tuple<WaitCallback, object>[] array;

            lock (this.workItems)
            {
                array = new Tuple<WaitCallback, object>[this.workItems.Count];
                this.workItems.CopyTo(array, 0);
                this.workItems.Clear();
            }

            for (int i = 0; i < array.Length; i++)
            {
                Tuple<WaitCallback, object> workItem = array[i];
                WaitCallback callback = workItem.Item1;
                object state = workItem.Item2;

                try
                {
                    callback(state);
                }
                catch (Exception e)
                {
                    log.Write( LogLevel.Error, "Executing task failed. callback: {0}, state: {1}\r\n{2}", callback, state, e );
                }

                Interlocked.Decrement(ref this.queuedItemCount);
            }
        }

        private void Start()
        {
            var waitHandles = new WaitHandle[]
            {
                this.thread.StopRequest,
                this.enqueueEvent
            };

            while (true)
            {
                WaitHandle.WaitAny(waitHandles);

                if (this.thread.IsStopRequested)
                {
                    break;
                }

                this.Dequeue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            this.thread.Stop();
            this.thread.Join();
            this.Dequeue();
        }
    }
}