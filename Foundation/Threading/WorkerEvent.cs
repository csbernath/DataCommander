using System;
using System.Threading;

namespace Foundation.Threading
{
    internal class WorkerEvent : WaitHandle
    {
        private readonly EventWaitHandle _eventWaitHandle;

        public WorkerEvent(WorkerEventState initialState)
        {
            this._eventWaitHandle = new EventWaitHandle(initialState == WorkerEventState.Signaled, EventResetMode.ManualReset);
            this.State = initialState;
            this.SafeWaitHandle = this._eventWaitHandle.SafeWaitHandle;
        }

        public WorkerEventState State { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            this.State = WorkerEventState.NonSignaled;
            return this._eventWaitHandle.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Set()
        {
            this.State = WorkerEventState.Signaled;
            return this._eventWaitHandle.Set();
        }

        public override void Close()
        {
            this._eventWaitHandle.Close();
        }

        public override bool WaitOne()
        {
            return this._eventWaitHandle.WaitOne();
        }

        public override bool WaitOne(int millisecondsTimeout)
        {
            return this._eventWaitHandle.WaitOne(millisecondsTimeout);
        }

        public override bool WaitOne(TimeSpan timeout)
        {
            return this._eventWaitHandle.WaitOne(timeout);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return this._eventWaitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
    }
}