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

        private WorkerThreadPool pool;

        private IWaitCallbackFactory waitCallbackFactory;

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
                int addableThreadCount = this.pool.MaxThreadCount - this.pool.Dequeuers.Count;
                int count = Math.Min( addableThreadCount, 5 );

                for (int i = 0; i < count; i++)
                {
                    WaitCallback callback = this.waitCallbackFactory.CreateWaitCallback();
                    var dequeuer = new WorkerThreadPoolDequeuer( callback );
                    this.pool.Dequeuers.Add( dequeuer );
                    dequeuer.Thread.Start();
                }
            }
            else
            {
                long timestamp = Stopwatch.GetTimestamp();
                List<WorkerThreadPoolDequeuer> dequeuers = new List<WorkerThreadPoolDequeuer>();
                WorkerThreadCollection threads = new WorkerThreadCollection();

                foreach (WorkerThreadPoolDequeuer dequeuer in this.pool.Dequeuers)
                {
                    int milliseconds = StopwatchTimeSpan.ToInt32( timestamp - dequeuer.LastActivityTimestamp, 1000 );

                    if (milliseconds >= 10000)
                    {
                        dequeuers.Add( dequeuer );
                        threads.Add( dequeuer.Thread );
                    }
                }

                foreach (WorkerThreadPoolDequeuer dequeuer in dequeuers)
                {
                    this.pool.Dequeuers.Remove( dequeuer );
                }

                ManualResetEvent stopEvent = new ManualResetEvent( false );
                threads.Stop( stopEvent );
                stopEvent.WaitOne();
            }
        }
    }
}