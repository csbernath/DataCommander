using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Foundation.Core;

namespace Foundation.Threading;

public sealed class WorkerThreadPoolManager(
    WorkerThreadPool pool,
    IWaitCallbackFactory waitCallbackFactory)
{
    private readonly WorkerThreadPool _pool = pool;

    private readonly IWaitCallbackFactory _waitCallbackFactory = waitCallbackFactory;

    private Timer _timer;

    public void Start() => _timer = new Timer(ManagePoolDequeuers, null, 10000, 10000);

    public void Stop() => _timer.Dispose();

    private void ManagePoolDequeuers(object state)
    {
        if (_pool.QueuedItemCount > 0)
        {
            int addableThreadCount = _pool.MaxThreadCount - _pool.Dequeuers.Count;
            int count = Math.Min(addableThreadCount, 5);

            for (int i = 0; i < count; i++)
            {
                WaitCallback callback = _waitCallbackFactory.CreateWaitCallback();
                WorkerThreadPoolDequeuer dequeuer = new WorkerThreadPoolDequeuer(callback);
                _pool.Dequeuers.Add(dequeuer);
                dequeuer.Thread.Start();
            }
        }
        else
        {
            long timestamp = Stopwatch.GetTimestamp();
            List<WorkerThreadPoolDequeuer> dequeuers = [];
            WorkerThreadCollection threads = [];

            foreach (WorkerThreadPoolDequeuer dequeuer in _pool.Dequeuers)
            {
                int milliseconds = StopwatchTimeSpan.ToInt32(timestamp - dequeuer.LastActivityTimestamp, 1000);

                if (milliseconds >= 10000)
                {
                    dequeuers.Add(dequeuer);
                    threads.Add(dequeuer.Thread);
                }
            }

            foreach (WorkerThreadPoolDequeuer dequeuer in dequeuers)
            {
                _pool.Dequeuers.Remove(dequeuer);
            }

            ManualResetEvent stopEvent = new ManualResetEvent(false);
            threads.Stop(stopEvent);
            stopEvent.WaitOne();
        }
    }
}