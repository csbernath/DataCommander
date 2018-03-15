using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Contracts;
using Foundation.Log;

namespace Foundation.Threading
{
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
            FoundationContract.Requires<ArgumentNullException>(start != null);

            _start = start;
            Thread = new Thread(PrivateStart);
            ThreadMonitor.Add(this);
        }

#endregion

#region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Started
        {
            add => _started += value;

            remove => _started -= value;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Stopped
        {
            add => _stopped += value;

            remove => _stopped -= value;
        }

#endregion

#region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public bool IsPauseRequested => _pauseRequest.State == WorkerEventState.Signaled;

        /// <summary>
        /// 
        /// </summary>
        public bool IsStopRequested
        {
            get
            {
                var isStopRequested = _stopRequest.State == WorkerEventState.Signaled;

                if (isStopRequested)
                {
                    if (!_isStopAccepted)
                    {
                        _isStopAccepted = true;
                        if (Log.IsTraceEnabled)
                        {
                            var stackTrace = new StackTrace(1, true);
                            Log.Trace("WorkerThread({0},{1}) accepted stop request.\r\n{2}", Thread.Name,
                                Thread.ManagedThreadId, stackTrace);
                        }
                    }
                }

                return isStopRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ThreadState ThreadState => Thread.ThreadState;

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle PauseRequest => _pauseRequest;

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle StopRequest => _stopRequest;

        /// <summary>
        ///  Gets a unique identifier for the current managed thread.
        /// </summary>
        public int ManagedThreadId => Thread.ManagedThreadId;

        /// <summary>
        ///  Gets or sets the name of the thread.
        /// </summary>
        public string Name
        {
            get => Thread.Name;

            set => Thread.Name = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating the scheduling priority of a thread.
        /// </summary>
        public ThreadPriority Priority
        {
            get => Thread.Priority;

            set => Thread.Priority = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get => Thread.IsBackground;

            set => Thread.IsBackground = value;
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
            Log.Trace("Starting WorkerThread({0})...", Thread.Name);
            StartTime = LocalTime.Default.Now;
            Thread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Log.Trace("Stopping WorkerThread({0},{1})...", Thread.Name, Thread.ManagedThreadId);
            StopTime = LocalTime.Default.Now;
            _stopRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Pause()
        {
            Log.Trace("WorkerThread({0},{1}) is requested to pause.", Thread.Name, Thread.ManagedThreadId);
            _pauseRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Continue()
        {
            Log.Trace("WorkerThread({0},{1}) is requested to continue.", Thread.Name, Thread.ManagedThreadId);
            _pauseRequest.Reset();
            _continueRequest.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitForStopOrContinue()
        {
            Log.Write(LogLevel.Error, "WorkerThread({0},{1}) is waiting for stop or continue request...",
                Thread.Name, Thread.ManagedThreadId);
            var ticks = Stopwatch.GetTimestamp();
            WaitHandle[] waitHandles = { _stopRequest, _continueRequest };
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

            Log.Trace("WorkerThread({0},{1}) accepted {2} request in {3} seconds.", Thread.Name,
                Thread.ManagedThreadId, request, StopwatchTimeSpan.ToString(ticks, 6));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Join()
        {
            Thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Join(int millisecondsTimeout)
        {
            return Thread.Join(millisecondsTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(TimeSpan timeout)
        {
            var signaled = _stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitForStop(int timeout)
        {
            var signaled = _stopRequest.WaitOne(timeout, false);
            return signaled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            Thread.Abort();
        }

#endregion

#region Private Methods

        private void PrivateStart()
        {
            var now = LocalTime.Default.Now;
            var elapsed = now - StartTime;
            var win32ThreadId = NativeMethods.GetCurrentThreadId();

            Log.Trace("WorkerThread({0},{1}) started in {2} seconds. Win32ThreadId: {3}", Thread.Name, Thread.ManagedThreadId, elapsed,
                win32ThreadId);

            Thread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (_started != null)
            {
                _started(this, null);
            }

            try
            {
                _start();
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, "WorkerThread({0},{1}) unhandled exception:\r\n{2}", Thread.Name,
                    Thread.ManagedThreadId, e);
            }

            now = LocalTime.Default.Now;
            if (_stopRequest.State == WorkerEventState.Signaled)
            {
                elapsed = now - StopTime;
            }
            else
            {
                elapsed = TimeSpan.Zero;
            }

            StopTime = now;

            Log.Trace(
                "WorkerThread({0},{1}) stopped in {2} seconds.",
                Thread.Name,
                Thread.ManagedThreadId,
                elapsed);

            if (_stopped != null)
            {
                _stopped(this, null);
            }

            _start = null;
        }

#endregion
    }
}