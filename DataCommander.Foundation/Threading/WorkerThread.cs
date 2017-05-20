namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Diagnostics.Log;
    using ThreadState = System.Threading.ThreadState;

    /// <summary>
    /// 
    /// </summary>
    public class WorkerThread
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private ThreadStart _start;
        private readonly WorkerEvent _stopRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private bool _isStopAccepted;
        private readonly WorkerEvent _pauseRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private readonly WorkerEvent _continueRequest = new WorkerEvent(WorkerEventState.NonSignaled);
        private EventHandler _started;
        private EventHandler _stopped;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public WorkerThread(ThreadStart start)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(start != null);
#endif

            this._start = start;
            this.Thread = new Thread(this.PrivateStart);
            ThreadMonitor.Add(this);
        }

#endregion

#region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Started
        {
            add => this._started += value;

            remove => this._started -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Stopped
        {
            add => this._stopped += value;

            remove => this._stopped -= value;
        }

#endregion

#region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsPauseRequested => this._pauseRequest.State == WorkerEventState.Signaled;

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopRequested
        {
            get
            {
                var isStopRequested = this._stopRequest.State == WorkerEventState.Signaled;

                if (isStopRequested)
                {
                    if (!this._isStopAccepted)
                    {
                        this._isStopAccepted = true;
                        if (Log.IsTraceEnabled)
                        {
                            var stackTrace = new StackTrace(1, true);
                            Log.Trace("WorkerThread({0},{1}) accepted stop request.\r\n{2}", this.Thread.Name,
                                this.Thread.ManagedThreadId, stackTrace);
                        }
                    }
                }

                return isStopRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ThreadState ThreadState => this.Thread.ThreadState;

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle PauseRequest => this._pauseRequest;

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle StopRequest => this._stopRequest;

        /// <summary>
        ///  Gets a unique identifier for the current managed thread.
        /// </summary>
        public int ManagedThreadId => this.Thread.ManagedThreadId;

        /// <summary>
        ///  Gets or sets the name of the thread.
        /// </summary>
        public string Name
        {
            get => this.Thread.Name;

            set => this.Thread.Name = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating the scheduling priority of a thread.
        /// </summary>
        public ThreadPriority Priority
        {
            get => this.Thread.Priority;

            set => this.Thread.Priority = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get => this.Thread.IsBackground;

            set => this.Thread.IsBackground = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Thread Thread { get; }

#endregion

#region Internal Properties

        internal DateTime StartTime { get; private set; }

        internal DateTime StopTime { get; private set; }

#endregion

#region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Log.Trace("Starting WorkerThread({0})...", this.Thread.Name);
            this.StartTime = LocalTime.Default.Now;
            this.Thread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Log.Trace("Stopping WorkerThread({0},{1})...", this.Thread.Name, this.Thread.ManagedThreadId);
            this.StopTime = LocalTime.Default.Now;
            this._stopRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            Log.Trace("WorkerThread({0},{1}) is requested to pause.", this.Thread.Name, this.Thread.ManagedThreadId);
            this._pauseRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Continue()
        {
            Log.Trace("WorkerThread({0},{1}) is requested to continue.", this.Thread.Name, this.Thread.ManagedThreadId);
            this._pauseRequest.Reset();
            this._continueRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitForStopOrContinue()
        {
            Log.Write(LogLevel.Error, "WorkerThread({0},{1}) is waiting for stop or continue request...",
                this.Thread.Name, this.Thread.ManagedThreadId);
            var ticks = Stopwatch.GetTimestamp();
            WaitHandle[] waitHandles = {this._stopRequest, this._continueRequest};
            var index = WaitHandle.WaitAny(waitHandles);
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

            Log.Trace("WorkerThread({0},{1}) accepted {2} request in {3} seconds.", this.Thread.Name,
                this.Thread.ManagedThreadId, request, StopwatchTimeSpan.ToString(ticks, 6));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Join()
        {
            this.Thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Join(int millisecondsTimeout)
        {
            return this.Thread.Join(millisecondsTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(TimeSpan timeout)
        {
            var signaled = this._stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(int timeout)
        {
            var signaled = this._stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            this.Thread.Abort();
        }

#endregion

#region Private Methods

        private void PrivateStart()
        {
            var now = LocalTime.Default.Now;
            var elapsed = now - this.StartTime;
            var win32ThreadId = NativeMethods.GetCurrentThreadId();

            Log.Write(
                LogLevel.Trace,
                "WorkerThread({0},{1}) started in {2} seconds. Win32ThreadId: {3}",
                this.Thread.Name,
                this.Thread.ManagedThreadId,
                elapsed,
                win32ThreadId);

            this.Thread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (this._started != null)
            {
                this._started(this, null);
            }

            try
            {
                this._start();
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, "WorkerThread({0},{1}) unhandled exception:\r\n{2}", this.Thread.Name,
                    this.Thread.ManagedThreadId, e);
            }

            now = LocalTime.Default.Now;
            if (this._stopRequest.State == WorkerEventState.Signaled)
            {
                elapsed = now - this.StopTime;
            }
            else
            {
                elapsed = TimeSpan.Zero;
            }

            this.StopTime = now;

            Log.Trace(
                "WorkerThread({0},{1}) stopped in {2} seconds.",
                this.Thread.Name,
                this.Thread.ManagedThreadId,
                elapsed);

            if (this._stopped != null)
            {
                this._stopped(this, null);
            }

            this._start = null;
        }

#endregion
    }
}