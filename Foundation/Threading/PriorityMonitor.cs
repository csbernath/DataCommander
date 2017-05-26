using System;
using System.Linq;
using System.Threading;
using Foundation.Collections.IndexableCollection;
using Foundation.Diagnostics.Log;

namespace Foundation.Threading
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PriorityMonitor<T>
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IndexableCollection<LockRequest> _lockRequests;
        private readonly NonUniqueIndex<int, LockRequest> _priorityIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitoredObject"></param>
        public PriorityMonitor(T monitoredObject)
        {
            this.MonitoredObject = monitoredObject;
            this._priorityIndex = new NonUniqueIndex<int, LockRequest>(
                "priorityIndex",
                item => GetKeyResponse.Create(true, item.Priority),
                SortOrder.Ascending);

            this._lockRequests = new IndexableCollection<LockRequest>(this._priorityIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public T MonitoredObject { get; }

        /// <summary>
        /// 
        /// </summary>
        public LockRequest CurrentLockRequest { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public LockRequest Enter(int priority)
        {
            var lockRequest = new LockRequest(this, priority);

            lock (this._lockRequests)
            {
                bool isCompleted;

                if (this.CurrentLockRequest == null)
                {
                    this.CurrentLockRequest = lockRequest;
                    isCompleted = true;
                }
                else
                {
                    this._lockRequests.Add(lockRequest);
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

            lock (this._lockRequests)
            {
                if (this.CurrentLockRequest == null)
                {
                    lockRequest = new LockRequest(this, priority);
                    lockRequest.Initialize(true);
                    this.CurrentLockRequest = lockRequest;
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
#if CONTRACTS_FULL
            Contract.Requires(lockRequest != null);
            Contract.Requires(lockRequest.Monitor == this);
            Contract.Requires(lockRequest == this.CurrentLockRequest);
#endif

            Log.Write(LogLevel.Trace, "Exiting lockRequest... monitoredObject: {0}, priority: {1}", this.MonitoredObject,
                lockRequest.Priority);

            lock (this._lockRequests)
            {
                if (this._lockRequests.Count == 0)
                {
                    this.CurrentLockRequest = null;
                }
                else
                {
                    var first = this._lockRequests.First();
                    this._lockRequests.Remove(first);
                    this.CurrentLockRequest = first;
                    first.Complete();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class LockRequest : IDisposable
        {
            private EventWaitHandle _asyncWaitHandle;

            internal LockRequest(PriorityMonitor<T> monitor, int priority)
            {
#if CONTRACTS_FULL
                Contract.Requires(monitor != null);
#endif

                this.Monitor = monitor;
                this.Priority = priority;
            }

            /// <summary>
            /// 
            /// </summary>
            public PriorityMonitor<T> Monitor { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public int Priority { get; }

            /// <summary>
            /// 
            /// </summary>
            public WaitHandle AsyncWaitHandle => this._asyncWaitHandle;

            /// <summary>
            /// 
            /// </summary>
            public bool IsCompleted { get; private set; }

            internal void Initialize(bool isCompleted)
            {
                Log.Write(LogLevel.Trace,
                    "Initializing lockRequest... monitoredObject: {0}, priority: {1}, isCompleted: {2}",
                    this.Monitor.MonitoredObject, this.Priority,
                    isCompleted);

                if (isCompleted)
                {
                    this.IsCompleted = true;
                }
                else
                {
                    this._asyncWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                }
            }

            internal void Complete()
            {
                Log.Write(LogLevel.Trace,
                    "Completing lockRequest... monitoredObject: {0}, priority:{1}, asyncWaitHandle != null: {2}",
                    this.Monitor.MonitoredObject, this.Priority,
                    this._asyncWaitHandle != null);
                this.IsCompleted = true;

                if (this._asyncWaitHandle != null)
                {
                    this._asyncWaitHandle.Set();
                }
            }

#region IDisposable Members

            void IDisposable.Dispose()
            {
                if (this.Monitor != null)
                {
                    var monitor = this.Monitor;
                    monitor.Exit(this);
                    this.Monitor = null;
                }
            }

#endregion
        }
    }
}