namespace DataCommander.Foundation.Threading
{
    using System.Collections;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// Asynchronous producer/consumer queue handler class.
    /// </summary>
    public class AsyncQueue
    {
        private string name;
        private IAsyncQueue asyncQueue;
        private readonly Queue queue = new Queue();
        private readonly AutoResetEvent queueEvent = new AutoResetEvent( false );

        private sealed class ConsumerThread
        {
            private readonly AsyncQueue queue;
            private readonly IConsumer consumer;

            public ConsumerThread(
                AsyncQueue queue,
                int id,
                ThreadPriority priority )
            {
                this.queue = queue;
                this.Thread = new WorkerThread( this.ThreadStart );
                this.Thread.Name = $"Consumer({queue.name},{id})";
                this.Thread.Priority = priority;
                this.consumer = this.queue.asyncQueue.CreateConsumer( this.Thread, id );
            }

            private void ThreadStart()
            {
                var state = this.consumer.Enter();

                try
                {
                    while (!this.Thread.IsStopRequested)
                    {
                        this.queue.Dequeue( this );
                    }
                }
                finally
                {
                    this.consumer.Exit( state );
                }
            }

            public void Consume( object item )
            {
                this.consumer.Consume( item );
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
            ThreadPriority priority )
        {
            Contract.Requires( consumerCount > 0 );

            this.name = name;
            this.asyncQueue = asyncQueue;

            for (var id = 0; id < consumerCount; id++)
            {
                var consumerThread = new ConsumerThread( this, id, priority );
                this.Consumers.Add( consumerThread.Thread );
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
            ThreadPriority priority )
        {
            this.Initialize( name, asyncQueue, consumerCount, priority );
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue( object item )
        {
            Contract.Requires( item != null );

            lock (this.queue)
            {
                this.queue.Enqueue( item );
            }

            this.queueEvent.Set();
        }

        private object Dequeue()
        {
            object item = null;

            if (this.queue.Count > 0)
            {
                lock (this.queue)
                {
                    if (this.queue.Count > 0)
                    {
                        item = this.queue.Dequeue();
                    }
                }
            }

            return item;
        }

        private void Consume( ConsumerThread consumerThread, object item )
        {
            Contract.Requires( consumerThread != null );

            var args = new AsyncQueueConsumeEventArgs( item );
            var eventHandler = this.asyncQueue.BeforeConsume;

            if (eventHandler != null)
            {
                eventHandler( this, args );
            }

            consumerThread.Consume( item );

            eventHandler = this.asyncQueue.AfterConsume;

            if (eventHandler != null)
            {
                eventHandler( this, args );
            }
        }

        private void Dequeue( ConsumerThread consumerThread )
        {
            var thread = consumerThread.Thread;
            WaitHandle[] waitHandles = { thread.StopRequest, this.queueEvent };

            while (!thread.IsStopRequested)
            {
                var item = this.Dequeue();

                if (item != null)
                {
                    this.Consume( consumerThread, item );
                }
                else
                {
                    WaitHandle.WaitAny( waitHandles );
                }
            }
        }

        /// <summary>
        /// Gets the number of unconsumed items (queued items).
        /// </summary>
        public int Count => this.queue.Count;

        /// <summary>
        /// Gets the consumer thread list.
        /// </summary>
        public WorkerThreadCollection Consumers { get; } = new WorkerThreadCollection();
    }
}