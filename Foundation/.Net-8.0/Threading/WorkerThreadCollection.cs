using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Foundation.Threading;

/// <summary>
/// 
/// </summary>
public sealed class WorkerThreadCollection : IList<WorkerThread>
{
    private readonly List<WorkerThread> _threads = [];

    #region IList<WorkerThread> Members

    int IList<WorkerThread>.IndexOf(WorkerThread item)
    {
        var index = _threads.IndexOf(item);
        return index;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void Insert(int index, WorkerThread item)
    {
        lock (_threads)
        {
            _threads.Insert(index, item);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        lock (_threads)
        {
            _threads.RemoveAt(index);
        }
    }

    WorkerThread IList<WorkerThread>.this[int index]
    {
        get => throw new Exception("The method or operation is not implemented.");

        set => throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    #region ICollection<WorkerThread> Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void Add(WorkerThread item)
    {
        lock (_threads)
        {
            _threads.Add(item);
        }
    }

    void ICollection<WorkerThread>.Clear()
    {
        throw new Exception("The method or operation is not implemented.");
    }

    bool ICollection<WorkerThread>.Contains(WorkerThread item)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    void ICollection<WorkerThread>.CopyTo(WorkerThread[] array, int arrayIndex)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count => _threads.Count;

    bool ICollection<WorkerThread>.IsReadOnly => throw new Exception("The method or operation is not implemented.");

    bool ICollection<WorkerThread>.Remove(WorkerThread item)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    #region IEnumerable<WorkerThread> Members

    IEnumerator<WorkerThread> IEnumerable<WorkerThread>.GetEnumerator()
    {
        return _threads.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        lock (_threads)
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
        lock (_threads)
        {
            foreach (var thread in _threads)
            {
                thread.Stop();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stopEvent"></param>
    public void Stop(EventWaitHandle stopEvent)
    {
        ArgumentNullException.ThrowIfNull(stopEvent);
        var stopper = new Stopper(_threads, stopEvent);
        stopper.Stop();
    }

    private sealed class Stopper(IList<WorkerThread> threads, EventWaitHandle stopEvent)
    {
        private int _count;

        public void Stop()
        {
            lock (threads)
            {
                foreach (var thread in threads)
                {
                    thread.Stopped += Thread_Stopped;
                    thread.Stop();
                }
            }
        }

        private void Thread_Stopped(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _count);

            if (_count == threads.Count)
            {
                stopEvent.Set();
            }
        }
    }
}