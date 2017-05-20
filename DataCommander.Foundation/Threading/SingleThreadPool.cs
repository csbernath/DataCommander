namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Diagnostics.Log;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SingleThreadPool
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Queue<Tuple<WaitCallback, object>> _workItems = new Queue<Tuple<WaitCallback, object>>();
        private readonly EventWaitHandle _enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        private int _queuedItemCount;

        /// <summary>
        /// 
        /// </summary>
        public SingleThreadPool()
        {
            this.Thread = new WorkerThread(this.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread { get; }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => this._queuedItemCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        public void QueueUserWorkItem(WaitCallback callback, object state)
        {
#if CONTRACTS_FULL
            Contract.Requires(callback != null);
#endif

            var tuple = Tuple.Create(callback, state);

            lock (this._workItems)
            {
                this._workItems.Enqueue(tuple);
            }

            Interlocked.Increment(ref this._queuedItemCount);
            this._enqueueEvent.Set();
        }

        private void Dequeue()
        {
            Tuple<WaitCallback, object>[] array;

            lock (this._workItems)
            {
                array = new Tuple<WaitCallback, object>[this._workItems.Count];
                this._workItems.CopyTo(array, 0);
                this._workItems.Clear();
            }

            for (var i = 0; i < array.Length; i++)
            {
                var workItem = array[i];
                var callback = workItem.Item1;
                var state = workItem.Item2;

                try
                {
                    callback(state);
                }
                catch (Exception e)
                {
                    Log.Write( LogLevel.Error, "Executing task failed. callback: {0}, state: {1}\r\n{2}", callback, state, e );
                }

                Interlocked.Decrement(ref this._queuedItemCount);
            }
        }

        private void Start()
        {
            var waitHandles = new WaitHandle[]
            {
                this.Thread.StopRequest,
                this._enqueueEvent
            };

            while (true)
            {
                WaitHandle.WaitAny(waitHandles);

                if (this.Thread.IsStopRequested)
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
            this.Thread.Stop();
            this.Thread.Join();
            this.Dequeue();
        }
    }
}