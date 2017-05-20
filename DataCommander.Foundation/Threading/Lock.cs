namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal sealed class Lock
    {
        private readonly object _lockObject = new object();
        private int _counter;

        public IDisposable Enter()
        {
            Monitor.Enter(this._lockObject);
            Interlocked.Increment(ref this._counter);
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            return new Disposer(this.Exit);
        }

        public bool TryEnter()
        {
            var entered = Monitor.TryEnter(this._lockObject);
            if (entered)
            {
                Interlocked.Increment(ref this._counter);
                this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            return entered;
        }

        public void Exit()
        {
            this.ThreadId = 0;
            Interlocked.Decrement(ref this._counter);
            Monitor.Exit(this._lockObject);
        }

        public bool Locked => this._counter > 0;

        public int ThreadId { get; private set; }
    }
}