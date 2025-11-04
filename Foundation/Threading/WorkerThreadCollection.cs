using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Foundation.Threading;

public sealed class WorkerThreadCollection : IList<WorkerThread>
{
    private readonly List<WorkerThread> _threads = [];
    private readonly Lock _threadsLock = new();

    int IList<WorkerThread>.IndexOf(WorkerThread item)
    {
        var index = _threads.IndexOf(item);
        return index;
    }

    public void Insert(int index, WorkerThread item)
    {
        using (_threadsLock.EnterScope())
            _threads.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        using (_threadsLock.EnterScope())
            _threads.RemoveAt(index);
    }

    WorkerThread IList<WorkerThread>.this[int index]
    {
        get => throw new Exception("The method or operation is not implemented.");

        set => throw new Exception("The method or operation is not implemented.");
    }

    public void Add(WorkerThread item)
    {
        using (_threadsLock.EnterScope())
            _threads.Add(item);
    }

    void ICollection<WorkerThread>.Clear() => throw new Exception("The method or operation is not implemented.");

    bool ICollection<WorkerThread>.Contains(WorkerThread item) => throw new Exception("The method or operation is not implemented.");

    void ICollection<WorkerThread>.CopyTo(WorkerThread[] array, int arrayIndex) => throw new Exception("The method or operation is not implemented.");

    public int Count => _threads.Count;

    bool ICollection<WorkerThread>.IsReadOnly => throw new Exception("The method or operation is not implemented.");

    bool ICollection<WorkerThread>.Remove(WorkerThread item) => throw new Exception("The method or operation is not implemented.");

    IEnumerator<WorkerThread> IEnumerable<WorkerThread>.GetEnumerator() => _threads.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => throw new Exception("The method or operation is not implemented.");

    public void Start()
    {
        using (_threadsLock.EnterScope())
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }
    }

    public void Stop()
    {
        using (_threadsLock.EnterScope())
        {
            foreach (var thread in _threads)
            {
                thread.Stop();
            }
        }
    }

    public void Stop(EventWaitHandle stopEvent)
    {
        ArgumentNullException.ThrowIfNull(stopEvent);
        var stopper = new Stopper(_threads, stopEvent);
        stopper.Stop();
    }

    private sealed class Stopper(IList<WorkerThread> threads, EventWaitHandle stopEvent)
    {
        private int _count;
        private readonly Lock _threadsLock = new();

        public void Stop()
        {
            using (_threadsLock.EnterScope())
            {
                foreach (var thread in threads)
                {
                    thread.Stopped += Thread_Stopped!;
                    thread.Stop();
                }
            }
        }

        private void Thread_Stopped(object? sender, EventArgs e)
        {
            Interlocked.Increment(ref _count);

            if (_count == threads.Count)
                stopEvent.Set();
        }
    }
}