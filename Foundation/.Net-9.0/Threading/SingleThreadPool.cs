using System;
using System.Collections.Generic;
using System.Threading;
using Foundation.Log;

namespace Foundation.Threading;

public sealed class SingleThreadPool
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly Queue<Tuple<WaitCallback, object>> _workItems = new();
    private readonly EventWaitHandle _enqueueEvent = new(false, EventResetMode.AutoReset);
    private int _queuedItemCount;

    public SingleThreadPool()
    {
        Thread = new WorkerThread(Start);
    }

    public WorkerThread Thread { get; }

    public int QueuedItemCount => _queuedItemCount;

    public void QueueUserWorkItem(WaitCallback callback, object state)
    {
        ArgumentNullException.ThrowIfNull(callback);

        Tuple<WaitCallback, object> tuple = Tuple.Create(callback, state);

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
                Log.Write(LogLevel.Error, "Executing task failed. callback: {0}, state: {1}\r\n{2}", callback, state, e);
            }

            Interlocked.Decrement(ref _queuedItemCount);
        }
    }

    private void Start()
    {
        WaitHandle[] waitHandles = new[]
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

    public void Stop()
    {
        Thread.Stop();
        Thread.Join();
        Dequeue();
    }
}