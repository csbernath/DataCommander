using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Foundation.Assertions;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class AsyncLogFile : ILogFile
{
    #region Private Fields

    private string _path;
    private readonly int _bufferSize;
    private readonly LogFile _logFile;
    private readonly ILogFormatter _formatter;
    private readonly ConcurrentQueue<LogEntry> _queue;
    private readonly TimeSpan _timerPeriod;
    private Timer _timer;

    #endregion

    public AsyncLogFile(
        string path,
        Encoding encoding,
        int bufferSize,
        TimeSpan timerPeriod,
        ILogFormatter formatter,
        FileAttributes fileAttributes,
        DateTimeKind dateTimeKind)
    {
        _path = path;
        _bufferSize = bufferSize;
        _timerPeriod = timerPeriod;
        _queue = new ConcurrentQueue<LogEntry>();
        _formatter = formatter;
        _logFile = new LogFile(path, encoding, 1024, true, formatter, fileAttributes, dateTimeKind);
    }

    #region ILogFile Members

    string ILogFile.FileName => _logFile.FileName;

    void ILogFile.Open()
    {
        Assert.IsTrue(_timer == null);

        _timer = new Timer(TimerCallback, null, _timerPeriod, _timerPeriod);
    }

    public void Write(LogEntry entry)
    {
        _queue.Enqueue(entry);
    }

    public void Flush()
    {
        while (_queue.TryDequeue(out var logEntry))
        {
            var text = _formatter.Format(logEntry);
            _logFile.Write(logEntry.CreationTime, text);
        }
    }

    public void Close()
    {
        if (_timer != null)
        {
            _timer.Dispose();
            _timer = null;
        }

        Flush();

        _logFile.Close();
    }

    #endregion

    #region Private Methods

    private void TimerCallback(object state)
    {
        var thread = Thread.CurrentThread;
        thread.Priority = ThreadPriority.Lowest;

        Flush();
    }

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
        Close();
    }

    #endregion
}