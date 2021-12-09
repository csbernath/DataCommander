using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Threading
{
#if FOUNDATION_3_5
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Foundation.Collections;
    using Foundation.Diagnostics;
    using Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LimitedConcurrencyLevelTaskScheduler
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private string name;
        private IProducerConsumerCollection<Action> queue;
        private int maximumConcurrencyLevel;
        private int threadCount;
        private int id;
        private EventHandler done;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maximumConcurrencyLevel"></param>
        /// <param name="queue"></param>
        public LimitedConcurrencyLevelTaskScheduler( string name, int maximumConcurrencyLevel, IProducerConsumerCollection<Action> queue )
        {
            Assert.IsTrue( maximumConcurrencyLevel >= 0 );
            Assert.IsTrue( queue != null );

            this.name = name;
            this.queue = queue;
            this.maximumConcurrencyLevel = maximumConcurrencyLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaximumConcurrencyLevel
        {
            get
            {
                return this.maximumConcurrencyLevel;
            }

            set
            {
                Assert.IsTrue( value >= 0 );
                this.maximumConcurrencyLevel = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ThreadCount
        {
            get
            {
                return this.threadCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Done
        {
            add
            {
                this.done += value;
            }

            remove
            {
                this.done -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void Enqueue( Action action )
        {
            lock (this.queue)
            {
                bool succeeded = this.queue.TryAdd( action );
                Assert.IsTrue( succeeded );

                if (this.threadCount < this.maximumConcurrencyLevel)
                {
                    Interlocked.Increment( ref this.threadCount );
                    ThreadPool.QueueUserWorkItem( this.Start );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (this.queue)
            {
                while (this.queue.Count > 0)
                {
                    Action action;
                    this.queue.TryTake( out action );
                }
            }
        }

        private void Start( object state )
        {
            try
            {
                var thread = WorkerThread.Current;
                int id = Interlocked.Increment( ref this.id );
                thread.Name = string.Format( "LimitedConcurrencyLevelTaskScheduler({0},{1})", this.name, id );
                // log.Trace( "Starting scheduler thread({0}), threadCount: {1}", id, this.threadCount );
                var stopwatch = Stopwatch.StartNew();

                while (this.queue.Count > 0)
                {
                    Action action = null;

                    lock (this.queue)
                    {
                        if (this.queue.Count > 0)
                        {
                            this.queue.TryTake( out action );
                        }
                    }

                    if (action != null)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception e)
                        {
                            log.Error( "Action failed. Exception:\r\n{0}", e.ToLogString() );
                        }
                    }
                }

                stopwatch.Stop();
                // log.Trace( "Stopping scheduler thread({0}), count: {1}, elapsed: {2}, threadCount: {3}", id, count, stopwatch.Elapsed, this.threadCount );
                int threadCount = Interlocked.Decrement( ref this.threadCount );

                if (this.queue.Count == 0 && threadCount == 0 && this.done != null)
                {
                    this.done( this, null );
                }
            }
            catch (Exception e)
            {
                log.Error( "Scheduler thread failure!!!, exception:\r\n{0}", e.ToLogString() );
            }
        }
    }
#else
    /// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while
    /// running on top of the ThreadPool.
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic] private static bool _currentThreadIsProcessingItems;

        /// <summary>The list of tasks to be executed.</summary>
        private readonly LinkedList<Task> _tasks = new(); // protected by lock(_tasks)

        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int _maxDegreeOfParallelism;

        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private int _delegatesQueuedOrRunning; // protected by lock(_tasks)

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
                throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism));
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally
                {
                    _currentThreadIsProcessingItems = false;
                }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns>Whether the task could be executed on the current thread.</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems)
                return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
                TryDequeue(task);

            // Try to run the task.
            return TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks)
                return _tasks.Remove(task);
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public sealed override int MaximumConcurrencyLevel => _maxDegreeOfParallelism;

        /// <summary>
        /// 
        /// </summary>
        public int QueuedItemCount => _tasks.Count;

        /// <summary>
        /// 
        /// </summary>
        public int ThreadCount => _delegatesQueuedOrRunning;

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken)
                    return _tasks.ToArray();
                else
                    throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_tasks);
            }
        }
    }
#endif
}