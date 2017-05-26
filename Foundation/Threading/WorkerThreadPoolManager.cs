using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Foundation.Diagnostics;

namespace Foundation.Threading
{
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
            IWaitCallbackFactory waitCallbackFactory )
        {
            this._pool = pool;
            this._waitCallbackFactory = waitCallbackFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            this._timer = new Timer( this.ManagePoolDequeuers, null, 10000, 10000 );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            this._timer.Dispose();
        }

        private void ManagePoolDequeuers( object state )
        {
            if (this._pool.QueuedItemCount > 0)
            {
                var addableThreadCount = this._pool.MaxThreadCount - this._pool.Dequeuers.Count;
                var count = Math.Min( addableThreadCount, 5 );

                for (var i = 0; i < count; i++)
                {
                    var callback = this._waitCallbackFactory.CreateWaitCallback();
                    var dequeuer = new WorkerThreadPoolDequeuer( callback );
                    this._pool.Dequeuers.Add( dequeuer );
                    dequeuer.Thread.Start();
                }
            }
            else
            {
                var timestamp = Stopwatch.GetTimestamp();
                var dequeuers = new List<WorkerThreadPoolDequeuer>();
                var threads = new WorkerThreadCollection();

                foreach (var dequeuer in this._pool.Dequeuers)
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
                    this._pool.Dequeuers.Remove( dequeuer );
                }

                var stopEvent = new ManualResetEvent( false );
                threads.Stop( stopEvent );
                stopEvent.WaitOne();
            }
        }
    }
}