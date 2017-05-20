namespace DataCommander.Foundation.Threading
{
    using System.Collections;
    using System.Threading;

    /// <summary>
    /// Asynchronous producer/consumer queue handler class.
    /// </summary>
    public class AsyncQueue
    {
        private string _name;
        private IAsyncQueue _asyncQueue;
        private readonly Queue _queue = new Queue();
        private readonly AutoResetEvent _queueEvent = new AutoResetEvent(false);

        private sealed class ConsumerThread
        {
            private readonly AsyncQueue _queue;
            private readonly IConsumer _consumer;

            public ConsumerThread(
                AsyncQueue queue,
                int id,
                ThreadPriority priority)
            {
                this._queue = queue;
                this.Thread = new WorkerThread(this.ThreadStart);
                this.Thread.Name = $"Consumer({queue._name},{id})";
                this.Thread.Priority = priority;
                this._consumer = this._queue._asyncQueue.CreateConsumer(this.Thread, id);
            }

            private void ThreadStart()
            {
                var state = this._consumer.Enter();

                try
                {
                    while (!this.Thread.IsStopRequested)
                    {
                        this._queue.Dequeue(this);
                    }
                }
                finally
                {
                    this._consumer.Exit(state);
                }
            }

            public void Consume(object item)
            {
                this._consumer.Consume(item);
            }

            public WorkerThread Thread { get; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected AsyncQueue()
        {
        }

        /// <summary>
        /// Inherited class must call this initializer method first.
        /// </summary>
        /// <param name="name">The name of the <see cref="AsyncQueue"/></param>
        /// <param name="asyncQueue">The <see cref="IAsyncQueue"/> implementation</param>
        /// <param name="consumerCount">Number of consumers</param>
        /// <param name="priority">priority of consumer threads</param>
        protected void Initialize(
            string name,
            IAsyncQueue asyncQueue,
            int consumerCount,
            ThreadPriority priority)
        {
#if CONTRACTS_FULL
            Contract.Requires(consumerCount > 0);
#endif

            this._name = name;
            this._asyncQueue = asyncQueue;

            for (var id = 0; id < consumerCount; id++)
            {
                var consumerThread = new ConsumerThread(this, id, priority);
                this.Consumers.Add(consumerThread.Thread);
            }
        }

        /// <summary>
        /// Calls <see cref="Initialize"/> method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asyncQueue"></param>
        /// <param name="consumerCount"></param>
        /// <param name="priority"></param>
        public AsyncQueue(
            string name,
            IAsyncQueue asyncQueue,
            int consumerCount,
            ThreadPriority priority)
        {
            this.Initialize(name, asyncQueue, consumerCount, priority);
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(object item)
        {
#if CONTRACTS_FULL
            Contract.Requires(item != null);
#endif

            lock (this._queue)
            {
                this._queue.Enqueue(item);
            }

            this._queueEvent.Set();
        }

        private object Dequeue()
        {
            object item = null;

            if (this._queue.Count > 0)
            {
                lock (this._queue)
                {
                    if (this._queue.Count > 0)
                    {
                        item = this._queue.Dequeue();
                    }
                }
            }

            return item;
        }

        private void Consume(ConsumerThread consumerThread, object item)
        {
#if CONTRACTS_FULL
            Contract.Requires(consumerThread != null);
#endif

            var args = new AsyncQueueConsumeEventArgs(item);
            var eventHandler = this._asyncQueue.BeforeConsume;

            if (eventHandler != null)
            {
                eventHandler(this, args);
            }

            consumerThread.Consume(item);

            eventHandler = this._asyncQueue.AfterConsume;

            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }

        private void Dequeue(ConsumerThread consumerThread)
        {
            var thread = consumerThread.Thread;
            WaitHandle[] waitHandles = {thread.StopRequest, this._queueEvent};

            while (!thread.IsStopRequested)
            {
                var item = this.Dequeue();

                if (item != null)
                {
                    this.Consume(consumerThread, item);
                }
                else
                {
                    WaitHandle.WaitAny(waitHandles);
                }
            }
        }

        /// <summary>
        /// Gets the number of unconsumed items (queued items).
        /// </summary>
        public int Count => this._queue.Count;

        /// <summary>
        /// Gets the consumer thread list.
        /// </summary>
        public WorkerThreadCollection Consumers { get; } = new WorkerThreadCollection();
    }
}