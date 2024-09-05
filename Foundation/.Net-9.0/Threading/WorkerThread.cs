using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Foundation.Core;
using Foundation.Log;
using ThreadState = System.Threading.ThreadState;

namespace Foundation.Threading;

public class WorkerThread
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly Thread _thread;
    private ThreadStart _start;
    private readonly WorkerEvent _stopRequest = new(WorkerEventState.NonSignaled);
    private bool _isStopAccepted;
    private readonly WorkerEvent _pauseRequest = new(WorkerEventState.NonSignaled);
    private readonly WorkerEvent _continueRequest = new(WorkerEventState.NonSignaled);
    private EventHandler _started;
    private EventHandler _stopped;

    public WorkerThread(ThreadStart start)
    {
        ArgumentNullException.ThrowIfNull(start);

        _start = start;
        _thread = new Thread(PrivateStart);
        ThreadMonitor.Add(this);
    }

    public event EventHandler Started
    {
        add => _started += value;
        remove => _started -= value;
    }

    public event EventHandler Stopped
    {
        add => _stopped += value;
        remove => _stopped -= value;
    }

    public bool IsPauseRequested => _pauseRequest.State == WorkerEventState.Signaled;

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
                    if (Log.IsTraceEnabled())
                    {
                        var stackTrace = new StackTrace(1, true);
                        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) accepted stop request.\r\n{stackTrace}");
                    }
                }
            }

            return isStopRequested;
        }
    }

    public ThreadState ThreadState => Thread.ThreadState;
    public WaitHandle PauseRequest => _pauseRequest;
    public WaitHandle StopRequest => _stopRequest;
    public int ManagedThreadId => Thread.ManagedThreadId;

    public string Name
    {
        get => Thread.Name;
        set => Thread.Name = value;
    }

    public ThreadPriority Priority
    {
        get => Thread.Priority;
        set => Thread.Priority = value;
    }

    public bool IsBackground
    {
        get => Thread.IsBackground;
        set => Thread.IsBackground = value;
    }

    public Thread Thread => _thread;

    internal DateTime StartTime { get; private set; }

    internal DateTime StopTime { get; private set; }

    public void Start()
    {
        Log.Trace($"Starting WorkerThread({Thread.Name})...");
        StartTime = LocalTime.Default.Now;
        Thread.Start();
    }

    public void Stop()
    {
        Log.Trace($"Stopping WorkerThread({Thread.Name},{Thread.ManagedThreadId})...");
        StopTime = LocalTime.Default.Now;
        _stopRequest.Set();
    }

    public void Pause()
    {
        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) is requested to pause.");
        _pauseRequest.Set();
    }

    public void Continue()
    {
        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) is requested to continue.");
        _pauseRequest.Reset();
        _continueRequest.Set();
    }

    public void WaitForStopOrContinue()
    {
        Log.Write(LogLevel.Error, $"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) is waiting for stop or continue request...");
        var ticks = Stopwatch.GetTimestamp();
        WaitHandle[] waitHandles = [_stopRequest, _continueRequest];
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

        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) accepted {request} request in {StopwatchTimeSpan.ToString(ticks, 6)} seconds.");
    }

    public void Join() => Thread.Join();
    public bool Join(int millisecondsTimeout) => Thread.Join(millisecondsTimeout);

    public bool WaitForStop(TimeSpan timeout)
    {
        var signaled = _stopRequest.WaitOne(timeout, false);
        return signaled;
    }

    public bool WaitForStop(int timeout)
    {
        var signaled = _stopRequest.WaitOne(timeout, false);
        return signaled;
    }

    private void PrivateStart()
    {
        var now = LocalTime.Default.Now;
        var elapsed = now - StartTime;
        var win32ThreadId = NativeMethods.GetCurrentThreadId();

        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) started in {elapsed} seconds. Win32ThreadId: {win32ThreadId}");

        Thread.CurrentUICulture = CultureInfo.InvariantCulture;

        if (_started != null)
            _started(this, null);

        try
        {
            _start();
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, $"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) unhandled exception:\r\n{e}");
        }

        now = LocalTime.Default.Now;
        if (_stopRequest.State == WorkerEventState.Signaled)
            elapsed = now - StopTime;
        else
            elapsed = TimeSpan.Zero;

        StopTime = now;

        Log.Trace($"WorkerThread({Thread.Name},{Thread.ManagedThreadId}) stopped in {elapsed} seconds.");

        if (_stopped != null)
            _stopped(this, null);

        _start = null;
    }
}