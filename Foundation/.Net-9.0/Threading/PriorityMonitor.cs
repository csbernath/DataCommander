using System;
using System.Linq;
using System.Threading;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;
using Foundation.Log;

namespace Foundation.Threading;

public sealed class PriorityMonitor<T>
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly IndexableCollection<LockRequest> _lockRequests;
    private readonly NonUniqueIndex<int, LockRequest> _priorityIndex;

    public PriorityMonitor(T monitoredObject)
    {
        MonitoredObject = monitoredObject;
        _priorityIndex = new NonUniqueIndex<int, LockRequest>(
            "priorityIndex",
            item => GetKeyResponse.Create(true, item.Priority),
            SortOrder.Ascending);

        _lockRequests = new IndexableCollection<LockRequest>(_priorityIndex);
    }

    public T MonitoredObject { get; }

    public LockRequest CurrentLockRequest { get; private set; }

    public LockRequest Enter(int priority)
    {
        LockRequest lockRequest = new LockRequest(this, priority);

        lock (_lockRequests)
        {
            bool isCompleted;

            if (CurrentLockRequest == null)
            {
                CurrentLockRequest = lockRequest;
                isCompleted = true;
            }
            else
            {
                _lockRequests.Add(lockRequest);
                isCompleted = false;
            }

            lockRequest.Initialize(isCompleted);
        }

        return lockRequest;
    }

    public LockRequest TryEnter(int priority)
    {
        LockRequest lockRequest;

        lock (_lockRequests)
        {
            if (CurrentLockRequest == null)
            {
                lockRequest = new LockRequest(this, priority);
                lockRequest.Initialize(true);
                CurrentLockRequest = lockRequest;
            }
            else
            {
                lockRequest = null;
            }
        }

        return lockRequest;
    }

    internal void Exit(LockRequest lockRequest)
    {
        ArgumentNullException.ThrowIfNull(lockRequest);
        Assert.IsTrue(lockRequest.Monitor == this);
        Assert.IsTrue(lockRequest == CurrentLockRequest);

        Log.Trace("Exiting lockRequest... monitoredObject: {0}, priority: {1}", MonitoredObject, lockRequest.Priority);

        lock (_lockRequests)
        {
            if (_lockRequests.Count == 0)
            {
                CurrentLockRequest = null;
            }
            else
            {
                LockRequest first = _lockRequests.First();
                _lockRequests.Remove(first);
                CurrentLockRequest = first;
                first.Complete();
            }
        }
    }

    public sealed class LockRequest : IDisposable
    {
        private EventWaitHandle _asyncWaitHandle;

        internal LockRequest(PriorityMonitor<T> monitor, int priority)
        {
            ArgumentNullException.ThrowIfNull(monitor);

            Monitor = monitor;
            Priority = priority;
        }

        public PriorityMonitor<T> Monitor { get; private set; }

        public int Priority { get; }

        public WaitHandle AsyncWaitHandle => _asyncWaitHandle;

        public bool IsCompleted { get; private set; }

        internal void Initialize(bool isCompleted)
        {
            Log.Trace("Initializing lockRequest... monitoredObject: {0}, priority: {1}, isCompleted: {2}", Monitor.MonitoredObject, Priority,
                isCompleted);

            if (isCompleted)
                IsCompleted = true;
            else
                _asyncWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        internal void Complete()
        {
            Log.Trace("Completing lockRequest... monitoredObject: {0}, priority:{1}, asyncWaitHandle != null: {2}", Monitor.MonitoredObject,
                Priority, _asyncWaitHandle != null);

            IsCompleted = true;

            if (_asyncWaitHandle != null)
            {
                _asyncWaitHandle.Set();
            }
        }

        void IDisposable.Dispose()
        {
            if (Monitor != null)
            {
                PriorityMonitor<T> monitor = Monitor;
                monitor.Exit(this);
                Monitor = null;
            }
        }
    }
}