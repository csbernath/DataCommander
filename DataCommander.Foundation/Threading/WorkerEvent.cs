namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal enum WorkerEventState
    {
        NonSignaled,
        Signaled
    }

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

        public WorkerEventState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean Reset()
        {
            this.state = WorkerEventState.NonSignaled;
            return this.eventWaitHandle.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean Set()
        {
            this.state = WorkerEventState.Signaled;
            return this.eventWaitHandle.Set();
        }

        public override void Close()
        {
            this.eventWaitHandle.Close();
        }

        public override Boolean WaitOne()
        {
            return this.eventWaitHandle.WaitOne();
        }

        public override Boolean WaitOne(Int32 millisecondsTimeout)
        {
            return this.eventWaitHandle.WaitOne(millisecondsTimeout);
        }

        public override Boolean WaitOne(TimeSpan timeout)
        {
            return this.eventWaitHandle.WaitOne(timeout);
        }

        public override Boolean WaitOne(Int32 millisecondsTimeout, Boolean exitContext)
        {
            return this.eventWaitHandle.WaitOne(millisecondsTimeout, exitContext);
        }
    }
}