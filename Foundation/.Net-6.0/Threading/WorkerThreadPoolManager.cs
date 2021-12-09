using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Foundation.Core;

namespace Foundation.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadPoolManager
    {
        #region Private Fields

        private readonly WorkerThreadPool _pool;

        private readonly IWaitCallbackFactory _waitCallbackFactory;

        private Timer _timer;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="waitCallbackFactory"></param>
        public WorkerThreadPoolManager(
            WorkerThreadPool pool,
            IWaitCallbackFactory waitCallbackFactory)
        {
            _pool = pool;
            _waitCallbackFactory = waitCallbackFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            _timer = new Timer(ManagePoolDequeuers, null, 10000, 10000);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _timer.Dispose();
        }

        private void ManagePoolDequeuers(object state)
        {
            if (_pool.QueuedItemCount > 0)
            {
                var addableThreadCount = _pool.MaxThreadCount - _pool.Dequeuers.Count;
                var count = Math.Min(addableThreadCount, 5);

                for (var i = 0; i < count; i++)
                {
                    var callback = _waitCallbackFactory.CreateWaitCallback();
                    var dequeuer = new WorkerThreadPoolDequeuer(callback);
                    _pool.Dequeuers.Add(dequeuer);
                    dequeuer.Thread.Start();
                }
            }
            else
            {
                var timestamp = Stopwatch.GetTimestamp();
                var dequeuers = new List<WorkerThreadPoolDequeuer>();
                var threads = new WorkerThreadCollection();

                foreach (var dequeuer in _pool.Dequeuers)
                {
                    var milliseconds = StopwatchTimeSpan.ToInt32(timestamp - dequeuer.LastActivityTimestamp, 1000);

                    if (milliseconds >= 10000)
                    {
                        dequeuers.Add(dequeuer);
                        threads.Add(dequeuer.Thread);
                    }
                }

                foreach (var dequeuer in dequeuers)
                {
                    _pool.Dequeuers.Remove(dequeuer);
                }

                var stopEvent = new ManualResetEvent(false);
                threads.Stop(stopEvent);
                stopEvent.WaitOne();
            }
        }
    }
}