using System;
using System.Threading;

namespace Foundation.Threading;

internal class WorkerEvent : WaitHandle
{
    private readonly EventWaitHandle _eventWaitHandle;

    public WorkerEvent(WorkerEventState initialState)
    {
        _eventWaitHandle = new EventWaitHandle(initialState == WorkerEventState.Signaled, EventResetMode.ManualReset);
        State = initialState;
        SafeWaitHandle = _eventWaitHandle.SafeWaitHandle;
    }

    public WorkerEventState State { get; private set; }

    public bool Reset()
    {
        State = WorkerEventState.NonSignaled;
        return _eventWaitHandle.Reset();
    }

    public bool Set()
    {
        State = WorkerEventState.Signaled;
        return _eventWaitHandle.Set();
    }

    public override void Close() => _eventWaitHandle.Close();

    public override bool WaitOne() => _eventWaitHandle.WaitOne();

    public override bool WaitOne(int millisecondsTimeout) => _eventWaitHandle.WaitOne(millisecondsTimeout);

    public override bool WaitOne(TimeSpan timeout) => _eventWaitHandle.WaitOne(timeout);

    public override bool WaitOne(int millisecondsTimeout, bool exitContext) => _eventWaitHandle.WaitOne(millisecondsTimeout, exitContext);
}