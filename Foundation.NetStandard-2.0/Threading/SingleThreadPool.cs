using System;
using System.Collections.Generic;
using System.Threading;
using Foundation.Diagnostics.Contracts;
using Foundation.Log;

namespace Foundation.Threading
{
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
            Thread = new WorkerThread(Start);
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread { get; }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => _queuedItemCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        public void QueueUserWorkItem(WaitCallback callback, object state)
        {
            FoundationContract.Requires<ArgumentException>(callback != null);

            var tuple = Tuple.Create(callback, state);

            lock (_workItems)
            {
                _workItems.Enqueue(tuple);
            }

            Interlocked.Increment(ref _queuedItemCount);
            _enqueueEvent.Set();
        }

        private void Dequeue()
        {
            Tuple<WaitCallback, object>[] array;

            lock (_workItems)
            {
                array = new Tuple<WaitCallback, object>[_workItems.Count];
                _workItems.CopyTo(array, 0);
                _workItems.Clear();
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

                Interlocked.Decrement(ref _queuedItemCount);
            }
        }

        private void Start()
        {
            var waitHandles = new WaitHandle[]
            {
                Thread.StopRequest,
                _enqueueEvent
            };

            while (true)
            {
                WaitHandle.WaitAny(waitHandles);

                if (Thread.IsStopRequested)
                {
                    break;
                }

                Dequeue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Thread.Stop();
            Thread.Join();
            Dequeue();
        }
    }
}