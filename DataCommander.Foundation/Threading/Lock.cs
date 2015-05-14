namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal sealed class Lock
    {
        private readonly object lockObject = new object();
        private int counter;
        private int threadId;

        public IDisposable Enter()
        {
            Monitor.Enter(this.lockObject);
            Interlocked.Increment(ref this.counter);
            this.threadId = Thread.CurrentThread.ManagedThreadId;
            return new Disposer(this.Exit);
        }

        public bool TryEnter()
        {
            bool entered = Monitor.TryEnter(this.lockObject);
            if (entered)
            {
                Interlocked.Increment(ref this.counter);
                this.threadId = Thread.CurrentThread.ManagedThreadId;
            }

            return entered;
        }

        public void Exit()
        {
            this.threadId = 0;
            Interlocked.Decrement(ref this.counter);
            Monitor.Exit(this.lockObject);
        }

        public bool Locked
        {
            get
            {
                return this.counter > 0;
            }
        }

        public int ThreadId
        {
            get
            {
                return this.threadId;
            }
        }
    }
}