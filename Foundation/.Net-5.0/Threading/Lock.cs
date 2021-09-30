using System;
using System.Threading;
using Foundation.Core;

namespace Foundation.Threading
{
    internal sealed class Lock
    {
        private readonly object _lockObject = new();
        private int _counter;

        public IDisposable Enter()
        {
            Monitor.Enter(_lockObject);
            Interlocked.Increment(ref _counter);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            return new Disposer(Exit);
        }

        public bool TryEnter()
        {
            var entered = Monitor.TryEnter(_lockObject);
            if (entered)
            {
                Interlocked.Increment(ref _counter);
                ThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            return entered;
        }

        public void Exit()
        {
            ThreadId = 0;
            Interlocked.Decrement(ref _counter);
            Monitor.Exit(_lockObject);
        }

        public bool Locked => _counter > 0;

        public int ThreadId { get; private set; }
    }
}