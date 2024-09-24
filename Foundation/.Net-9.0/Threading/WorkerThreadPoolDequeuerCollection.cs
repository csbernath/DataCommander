using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Threading;

/// <summary>
/// 
/// </summary>
public sealed class WorkerThreadPoolDequeuerCollection : IList<WorkerThreadPoolDequeuer>
{
    private readonly List<WorkerThreadPoolDequeuer> _list = [];
    private readonly WorkerThreadPool _pool;

    internal WorkerThreadPoolDequeuerCollection(WorkerThreadPool pool)
    {
        _pool = pool;
    }

    /// <summary>
    /// 
    /// </summary>
    public WorkerThreadCollection Threads { get; } = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dequeuer"></param>
    public void Add(WorkerThreadPoolDequeuer dequeuer)
    {
        Assert.IsTrue(dequeuer != null);

        Threads.Add(dequeuer.Thread);
        _list.Add(dequeuer);
        dequeuer.Pool = _pool;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dequeuer"></param>
    public void Remove(WorkerThreadPoolDequeuer dequeuer) => _list.Remove(dequeuer);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(WorkerThreadPoolDequeuer item) => _list.IndexOf(item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void Insert(int index, WorkerThreadPoolDequeuer item) => _list.Insert(index, item);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => _list.RemoveAt(index);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public WorkerThreadPoolDequeuer this[int index]
    {
        get => _list[index];

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Clear() => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(WorkerThreadPoolDequeuer item) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(WorkerThreadPoolDequeuer[] array, int arrayIndex) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public int Count => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    public bool IsReadOnly => throw new NotImplementedException();

    bool ICollection<WorkerThreadPoolDequeuer>.Remove(WorkerThreadPoolDequeuer item) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<WorkerThreadPoolDequeuer> GetEnumerator() => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}