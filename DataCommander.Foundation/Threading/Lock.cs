namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal sealed class Lock
    {
        private readonly object lockObject = new object();
        private int counter;

        public IDisposable Enter()
        {
            Monitor.Enter(this.lockObject);
            Interlocked.Increment(ref this.counter);
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            return new Disposer(this.Exit);
        }

        public bool TryEnter()
        {
            bool entered = Monitor.TryEnter(this.lockObject);
            if (entered)
            {
                Interlocked.Increment(ref this.counter);
                this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            return entered;
        }

        public void Exit()
        {
            this.ThreadId = 0;
            Interlocked.Decrement(ref this.counter);
            Monitor.Exit(this.lockObject);
        }

        public bool Locked => this.counter > 0;

        public int ThreadId { get; private set; }
    }
}