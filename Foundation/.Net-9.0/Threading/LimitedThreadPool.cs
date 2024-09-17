using System;
using System.Collections.Generic;
using System.Threading;

namespace Foundation.Threading;

public sealed class LimitedThreadPool<T>(int maxThreadCount)
{
    private readonly int _maxThreadCount = maxThreadCount;
    private int _availableThreadCount = maxThreadCount;
    private readonly Queue<Tuple<Action<T>, T>> _queue = new();

    public int QueuedItemCount => _queue.Count;

    public int AvailableThreadCount => _availableThreadCount;

    public bool QueueUserWorkItem(Action<T> waitCallback, T state)
    {
        Tuple<Action<T>, T> item = Tuple.Create(waitCallback, state);
        bool succeeded;
        if (_availableThreadCount > 0)
        {
            Interlocked.Decrement(ref _availableThreadCount);
            succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
        }
        else
        {
            lock (_queue)
            {
                _queue.Enqueue(item);
            }

            succeeded = true;
        }

        return succeeded;
    }

    public void Pulse()
    {
        if (_queue.Count > 0 && _availableThreadCount > 0)
        {
            lock (_queue)
            {
                while (_queue.Count > 0 && _availableThreadCount > 0)
                {
                    Tuple<Action<T>, T> item = _queue.Dequeue();
                    bool succeeded = ThreadPool.QueueUserWorkItem(Callback, item);
                    Interlocked.Decrement(ref _availableThreadCount);
                }
            }
        }
    }

    private void Callback(object stateObject)
    {
        try
        {
            Tuple<Action<T>, T> item = (Tuple<Action<T>, T>)stateObject;
            Action<T> waitCallback = item.Item1;
            T state = item.Item2;
            waitCallback(state);
        }
        finally
        {
            Interlocked.Increment(ref _availableThreadCount);
            Pulse();
        }
    }
}