namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public interface IWaitCallbackFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        WaitCallback CreateWaitCallback();
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadPoolManager
    {
        #region Private Fields

        private readonly WorkerThreadPool pool;

        private readonly IWaitCallbackFactory waitCallbackFactory;

        private Timer timer;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="waitCallbackFactory"></param>
        public WorkerThreadPoolManager(
            WorkerThreadPool pool,
            IWaitCallbackFactory waitCallbackFactory )
        {
            this.pool = pool;
            this.waitCallbackFactory = waitCallbackFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this.timer = new Timer( this.ManagePoolDequeuers, null, 10000, 10000 );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            this.timer.Dispose();
        }

        private void ManagePoolDequeuers( object state )
        {
            if (this.pool.QueuedItemCount > 0)
            {
                var addableThreadCount = this.pool.MaxThreadCount - this.pool.Dequeuers.Count;
                var count = Math.Min( addableThreadCount, 5 );

                for (var i = 0; i < count; i++)
                {
                    var callback = this.waitCallbackFactory.CreateWaitCallback();
                    var dequeuer = new WorkerThreadPoolDequeuer( callback );
                    this.pool.Dequeuers.Add( dequeuer );
                    dequeuer.Thread.Start();
                }
            }
            else
            {
                var timestamp = Stopwatch.GetTimestamp();
                var dequeuers = new List<WorkerThreadPoolDequeuer>();
                var threads = new WorkerThreadCollection();

                foreach (var dequeuer in this.pool.Dequeuers)
                {
                    var milliseconds = StopwatchTimeSpan.ToInt32( timestamp - dequeuer.LastActivityTimestamp, 1000 );

                    if (milliseconds >= 10000)
                    {
                        dequeuers.Add( dequeuer );
                        threads.Add( dequeuer.Thread );
                    }
                }

                foreach (var dequeuer in dequeuers)
                {
                    this.pool.Dequeuers.Remove( dequeuer );
                }

                var stopEvent = new ManualResetEvent( false );
                threads.Stop( stopEvent );
                stopEvent.WaitOne();
            }
        }
    }
}