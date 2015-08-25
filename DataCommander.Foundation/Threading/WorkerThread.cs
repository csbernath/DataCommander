namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using ThreadState = System.Threading.ThreadState;

    /// <summary>
    /// 
    /// </summary>
    public class WorkerThread
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Thread thread;
        private ThreadStart start;
        private DateTime startTime;
        private DateTime stopTime;
        private readonly WorkerEvent stopRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private bool isStopAccepted;
        private readonly WorkerEvent pauseRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private readonly WorkerEvent continueRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private EventHandler started;
        private EventHandler stopped;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public WorkerThread(ThreadStart start)
        {
            Contract.Requires<ArgumentNullException>(start != null);

            this.start = start;
            this.thread = new Thread(this.PrivateStart);
            ThreadMonitor.Add(this);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Started
        {
            add
            {
                this.started += value;
            }

            remove
            {
                this.started -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Stopped
        {
            add
            {
                this.stopped += value;
            }

            remove
            {
                this.stopped -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsPauseRequested
        {
            get
            {
                return this.pauseRequest.State == WorkerEventState.Signaled;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopRequested
        {
            get
            {
                bool isStopRequested = this.stopRequest.State == WorkerEventState.Signaled;

                if (isStopRequested)
                {
                    if (!this.isStopAccepted)
                    {
                        this.isStopAccepted = true;
                        if (log.IsTraceEnabled)
                        {
                            var stackTrace = new StackTrace(1, true);
                            log.Trace("WorkerThread({0},{1}) accepted stop request.\r\n{2}", this.thread.Name,
                                this.thread.ManagedThreadId, stackTrace);
                        }
                    }
                }

                return isStopRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ThreadState ThreadState
        {
            get
            {
                return this.thread.ThreadState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle PauseRequest
        {
            get
            {
                return this.pauseRequest;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle StopRequest
        {
            get
            {
                return this.stopRequest;
            }
        }

        /// <summary>
        ///  Gets a unique identifier for the current managed thread.
        /// </summary>
        public int ManagedThreadId
        {
            get
            {
                return this.thread.ManagedThreadId;
            }
        }

        /// <summary>
        ///  Gets or sets the name of the thread.
        /// </summary>
        public string Name
        {
            get
            {
                return this.thread.Name;
            }

            set
            {
                this.thread.Name = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating the scheduling priority of a thread.
        /// </summary>
        public ThreadPriority Priority
        {
            get
            {
                return this.thread.Priority;
            }

            set
            {
                this.thread.Priority = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return this.thread.IsBackground;
            }

            set
            {
                this.thread.IsBackground = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Thread Thread
        {
            get
            {
                return this.thread;
            }
        }

        #endregion

        #region Internal Properties

        internal DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        internal DateTime StopTime
        {
            get
            {
                return this.stopTime;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            log.Trace("Starting WorkerThread({0})...", this.thread.Name);
            this.startTime = LocalTime.Default.Now;
            this.thread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            log.Trace("Stopping WorkerThread({0},{1})...", this.thread.Name, this.thread.ManagedThreadId);
            this.stopTime = LocalTime.Default.Now;
            this.stopRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            log.Trace("WorkerThread({0},{1}) is requested to pause.", this.thread.Name, this.thread.ManagedThreadId);
            this.pauseRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Continue()
        {
            log.Trace("WorkerThread({0},{1}) is requested to continue.", this.thread.Name, this.thread.ManagedThreadId);
            this.pauseRequest.Reset();
            this.continueRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitForStopOrContinue()
        {
            log.Write(LogLevel.Error, "WorkerThread({0},{1}) is waiting for stop or continue request...",
                this.thread.Name, this.thread.ManagedThreadId);
            long ticks = Stopwatch.GetTimestamp();
            WaitHandle[] waitHandles = {this.stopRequest, this.continueRequest};
            int index = WaitHandle.WaitAny(waitHandles);
            ticks = Stopwatch.GetTimestamp() - ticks;
            string request;

            switch (index)
            {
                case 0:
                    request = "stop";
                    break;

                case 1:
                    request = "continue";
                    break;

                default:
                    request = "unknown";
                    break;
            }

            log.Trace("WorkerThread({0},{1}) accepted {2} request in {3} seconds.", this.thread.Name,
                this.thread.ManagedThreadId, request, StopwatchTimeSpan.ToString(ticks, 6));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Join()
        {
            this.thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Join(int millisecondsTimeout)
        {
            return this.thread.Join(millisecondsTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(TimeSpan timeout)
        {
            bool signaled = this.stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(int timeout)
        {
            bool signaled = this.stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            this.thread.Abort();
        }

        #endregion

        #region Private Methods

        private void PrivateStart()
        {
            var now = LocalTime.Default.Now;
            TimeSpan elapsed = now - this.startTime;
            UInt32 win32threadId = NativeMethods.GetCurrentThreadId();

            log.Write(
                LogLevel.Trace,
                "WorkerThread({0},{1}) started in {2} seconds. Win32ThreadId: {3}",
                this.thread.Name,
                this.thread.ManagedThreadId,
                elapsed,
                win32threadId);

            this.thread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (this.started != null)
            {
                this.started(this, null);
            }

            try
            {
                this.start();
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, "WorkerThread({0},{1}) unhandled exception:\r\n{2}", this.thread.Name,
                    this.thread.ManagedThreadId, e);
            }

            now = LocalTime.Default.Now;
            if (this.stopRequest.State == WorkerEventState.Signaled)
            {
                elapsed = now - this.stopTime;
            }
            else
            {
                elapsed = TimeSpan.Zero;
            }

            this.stopTime = now;

            log.Trace(
                "WorkerThread({0},{1}) stopped in {2} seconds.",
                this.thread.Name,
                this.thread.ManagedThreadId,
                elapsed);

            if (this.stopped != null)
            {
                this.stopped(this, null);
            }

            this.start = null;
        }

        #endregion
    }
}