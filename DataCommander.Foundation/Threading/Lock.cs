namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Threading;

    internal sealed class Lock
    {
        private readonly Object lockObject = new Object();
        private Int32 counter;
        private Int32 threadId;

        public IDisposable Enter()
        {
            Monitor.Enter( this.lockObject );
            Interlocked.Increment( ref this.counter );
            this.threadId = Thread.CurrentThread.ManagedThreadId;
            return new Disposer( this.Exit );
        }

        public Boolean TryEnter()
        {
            Boolean entered = Monitor.TryEnter( this.lockObject );
            if (entered)
            {
                Interlocked.Increment( ref this.counter );
                this.threadId = Thread.CurrentThread.ManagedThreadId;
            }

            return entered;
        }

        public void Exit()
        {
            this.threadId = 0;
            Interlocked.Decrement( ref this.counter );
            Monitor.Exit( this.lockObject );
        }

        public Boolean Locked
        {
            get
            {
                return this.counter > 0;
            }
        }

        public Int32 ThreadId
        {
            get
            {
                return this.threadId;
            }
        }
    }
}