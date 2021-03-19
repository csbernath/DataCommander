using System;
using System.Linq;
using System.Threading;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;
using Foundation.Log;

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
            MonitoredObject = monitoredObject;
            _priorityIndex = new NonUniqueIndex<int, LockRequest>(
                "priorityIndex",
                item => GetKeyResponse.Create(true, item.Priority),
                SortOrder.Ascending);

            _lockRequests = new IndexableCollection<LockRequest>(_priorityIndex);
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

            lock (_lockRequests)
            {
                bool isCompleted;

                if (CurrentLockRequest == null)
                {
                    CurrentLockRequest = lockRequest;
                    isCompleted = true;
                }
                else
                {
                    _lockRequests.Add(lockRequest);
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

            lock (_lockRequests)
            {
                if (CurrentLockRequest == null)
                {
                    lockRequest = new LockRequest(this, priority);
                    lockRequest.Initialize(true);
                    CurrentLockRequest = lockRequest;
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
            Assert.IsNotNull(lockRequest);
            Assert.IsTrue(lockRequest.Monitor == this);
            Assert.IsTrue(lockRequest == CurrentLockRequest);

            Log.Trace("Exiting lockRequest... monitoredObject: {0}, priority: {1}", MonitoredObject, lockRequest.Priority);

            lock (_lockRequests)
            {
                if (_lockRequests.Count == 0)
                {
                    CurrentLockRequest = null;
                }
                else
                {
                    var first = _lockRequests.First();
                    _lockRequests.Remove(first);
                    CurrentLockRequest = first;
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
                Assert.IsNotNull(monitor);

                Monitor = monitor;
                Priority = priority;
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
            public WaitHandle AsyncWaitHandle => _asyncWaitHandle;

            /// <summary>
            /// 
            /// </summary>
            public bool IsCompleted { get; private set; }

            internal void Initialize(bool isCompleted)
            {
                Log.Trace("Initializing lockRequest... monitoredObject: {0}, priority: {1}, isCompleted: {2}", Monitor.MonitoredObject, Priority,
                    isCompleted);

                if (isCompleted)
                    IsCompleted = true;
                else
                    _asyncWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            }

            internal void Complete()
            {
                Log.Trace("Completing lockRequest... monitoredObject: {0}, priority:{1}, asyncWaitHandle != null: {2}", Monitor.MonitoredObject,
                    Priority, _asyncWaitHandle != null);

                IsCompleted = true;

                if (_asyncWaitHandle != null)
                {
                    _asyncWaitHandle.Set();
                }
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (Monitor != null)
                {
                    var monitor = Monitor;
                    monitor.Exit(this);
                    Monitor = null;
                }
            }

            #endregion
        }
    }
}