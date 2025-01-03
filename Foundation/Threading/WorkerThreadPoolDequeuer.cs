using System.Diagnostics;
using System.Threading;

namespace Foundation.Threading;

/// <summary>
/// 
/// </summary>
public class WorkerThreadPoolDequeuer
{
    private WorkerThreadPool _pool;
    private readonly WaitCallback _callback;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public WorkerThreadPoolDequeuer(WaitCallback callback)
    {
        _callback = callback;
        Thread = new WorkerThread(Start);
    }

    private void Start()
    {
        WaitHandle[] waitHandles = [Thread.StopRequest, _pool.EnqueueEvent];

        while (!Thread.IsStopRequested)
        {
            var dequeued = _pool.Dequeue(_callback, waitHandles);

            if (dequeued)
            {
                LastActivityTimestamp = Stopwatch.GetTimestamp();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public WorkerThread Thread { get; }

    internal WorkerThreadPool Pool
    {
        set => _pool = value;
    }

    internal long LastActivityTimestamp { get; private set; }
}