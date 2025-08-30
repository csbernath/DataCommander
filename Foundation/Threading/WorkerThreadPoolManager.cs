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
    private Timer? _timer;

    public void Start() => _timer = new Timer(ManagePoolDequeuers, null, 10000, 10000);

    public void Stop() => _timer!.Dispose();

    private void ManagePoolDequeuers(object? state)
    {
        if (pool.QueuedItemCount > 0)
        {
            var addableThreadCount = pool.MaxThreadCount - pool.Dequeuers.Count;
            var count = Math.Min(addableThreadCount, 5);

            for (var i = 0; i < count; i++)
            {
                var callback = waitCallbackFactory.CreateWaitCallback();
                var dequeuer = new WorkerThreadPoolDequeuer(callback);
                pool.Dequeuers.Add(dequeuer);
                dequeuer.Thread.Start();
            }
        }
        else
        {
            var timestamp = Stopwatch.GetTimestamp();
            List<WorkerThreadPoolDequeuer> dequeuers = [];
            WorkerThreadCollection threads = [];

            foreach (var dequeuer in pool.Dequeuers)
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
                pool.Dequeuers.Remove(dequeuer);
            }

            var stopEvent = new ManualResetEvent(false);
            threads.Stop(stopEvent);
            stopEvent.WaitOne();
        }
    }
}