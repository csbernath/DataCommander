using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Threading;

public sealed class WorkerThreadPoolDequeuerCollection : IList<WorkerThreadPoolDequeuer>
{
    private readonly List<WorkerThreadPoolDequeuer> _list = [];
    private readonly WorkerThreadPool _pool;

    internal WorkerThreadPoolDequeuerCollection(WorkerThreadPool pool)
    {
        _pool = pool;
    }

    public WorkerThreadCollection Threads { get; } = [];

    public void Add(WorkerThreadPoolDequeuer dequeuer)
    {
        ArgumentNullException.ThrowIfNull(dequeuer);
        
        Threads.Add(dequeuer.Thread);
        _list.Add(dequeuer);
        dequeuer.Pool = _pool;
    }

    public void Remove(WorkerThreadPoolDequeuer dequeuer) => _list.Remove(dequeuer);

    public int IndexOf(WorkerThreadPoolDequeuer item) => _list.IndexOf(item);

    public void Insert(int index, WorkerThreadPoolDequeuer item) => _list.Insert(index, item);

    public void RemoveAt(int index) => _list.RemoveAt(index);

    public WorkerThreadPoolDequeuer this[int index]
    {
        get => _list[index];

        set => throw new NotImplementedException();
    }

    public void Clear() => throw new NotImplementedException();

    public bool Contains(WorkerThreadPoolDequeuer item) => throw new NotImplementedException();

    public void CopyTo(WorkerThreadPoolDequeuer[] array, int arrayIndex) => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    bool ICollection<WorkerThreadPoolDequeuer>.Remove(WorkerThreadPoolDequeuer item) => throw new NotImplementedException();

    public IEnumerator<WorkerThreadPoolDequeuer> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}