namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PriorityMonitor<T>
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly T monitoredObject;
        private LockRequest currentLockRequest;
        private readonly IndexableCollection<LockRequest> lockRequests;
        private readonly NonUniqueIndex<int, LockRequest> priorityIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitoredObject"></param>
        public PriorityMonitor(T monitoredObject)
        {
            this.monitoredObject = monitoredObject;
            this.priorityIndex = new NonUniqueIndex<int, LockRequest>(
                "priorityIndex",
                item => GetKeyResponse.Create(true, item.Priority),
                SortOrder.Ascending);

            this.lockRequests = new IndexableCollection<LockRequest>(this.priorityIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public T MonitoredObject
        {
            get
            {
                return this.monitoredObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LockRequest CurrentLockRequest
        {
            get
            {
                return this.currentLockRequest;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public LockRequest Enter(int priority)
        {
            LockRequest lockRequest = new LockRequest(this, priority);

            lock (this.lockRequests)
            {
                bool isCompleted;

                if (this.currentLockRequest == null)
                {
                    this.currentLockRequest = lockRequest;
                    isCompleted = true;
                }
                else
                {
                    this.lockRequests.Add(lockRequest);
                    isCompleted = false;
                }

                lockRequest.Initialize(isCompleted);
            }

            return lockRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public LockRequest TryEnter(int priority)
        {
            LockRequest lockRequest;

            lock (this.lockRequests)
            {
                if (this.currentLockRequest == null)
                {
                    lockRequest = new LockRequest(this, priority);
                    lockRequest.Initialize(true);
                    this.currentLockRequest = lockRequest;
                }
                else
                {
                    lockRequest = null;
                }
            }

            return lockRequest;
        }

        internal void Exit(LockRequest lockRequest)
        {
            Contract.Requires(lockRequest != null);
            Contract.Requires(lockRequest.Monitor == this);
            Contract.Requires(lockRequest == this.CurrentLockRequest);

            log.Write(LogLevel.Trace, "Exiting lockRequest... monitoredObject: {0}, priority: {1}", this.monitoredObject, lockRequest.Priority);

            lock (this.lockRequests)
            {
                if (this.lockRequests.Count == 0)
                {
                    this.currentLockRequest = null;
                }
                else
                {
                    LockRequest first = this.lockRequests.First();
                    this.lockRequests.Remove(first);
                    this.currentLockRequest = first;
                    first.Complete();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class LockRequest : IDisposable
        {
            private PriorityMonitor<T> monitor;
            private readonly int priority;
            private bool isCompleted;
            private EventWaitHandle asyncWaitHandle;

            internal LockRequest(PriorityMonitor<T> monitor, int priority)
            {
                Contract.Requires(monitor != null);

                this.monitor = monitor;
                this.priority = priority;
            }

            /// <summary>
            /// 
            /// </summary>
            public PriorityMonitor<T> Monitor
            {
                get
                {
                    return this.monitor;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public int Priority
            {
                get
                {
                    return this.priority;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return this.asyncWaitHandle;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool IsCompleted
            {
                get
                {
                    return this.isCompleted;
                }
            }

            internal void Initialize(bool isCompleted)
            {
                log.Write(LogLevel.Trace, "Initializing lockRequest... monitoredObject: {0}, priority: {1}, isCompleted: {2}", this.monitor.MonitoredObject, this.priority,
                    isCompleted);

                if (isCompleted)
                {
                    this.isCompleted = true;
                }
                else
                {
                    this.asyncWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                }
            }

            internal void Complete()
            {
                log.Write(LogLevel.Trace, "Completing lockRequest... monitoredObject: {0}, priority:{1}, asyncWaitHandle != null: {2}", this.monitor.MonitoredObject, this.priority,
                    this.asyncWaitHandle != null);
                this.isCompleted = true;

                if (this.asyncWaitHandle != null)
                {
                    this.asyncWaitHandle.Set();
                }
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (this.monitor != null)
                {
                    PriorityMonitor<T> monitor = this.monitor;
                    monitor.Exit(this);
                    this.monitor = null;
                }
            }

            #endregion
        }
    }
}