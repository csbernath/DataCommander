namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal class WorkerEvent : WaitHandle
    {
        private readonly EventWaitHandle eventWaitHandle;
        private WorkerEventState state;

        public WorkerEvent(WorkerEventState initialState)
        {
            this.eventWaitHandle = new EventWaitHandle(initialState == WorkerEventState.Signaled, EventResetMode.ManualReset);
            this.state = initialState;
            this.SafeWaitHandle = this.eventWaitHandle.SafeWaitHandle;
        }

        public WorkerEventState State => this.state;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            this.state = WorkerEventState.NonSignaled;
            return this.eventWaitHandle.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Set()
        {
            this.state = WorkerEventState.Signaled;
            return this.eventWaitHandle.Set();
        }

        public override void Close()
        {
            this.eventWaitHandle.Close();
        }

        public override bool WaitOne()
        {
            return this.eventWaitHandle.WaitOne();
        }

        public override bool WaitOne(int millisecondsTimeout)
        {
            return this.eventWaitHandle.WaitOne(millisecondsTimeout);
        }

        public override bool WaitOne(TimeSpan timeout)
        {
            return this.eventWaitHandle.WaitOne(timeout);
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return this.eventWaitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
    }
}