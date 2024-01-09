﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Foundation.Assertions;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class AsyncLogFile(
    string path,
    Encoding encoding,
    int bufferSize,
    TimeSpan timerPeriod,
    ILogFormatter formatter,
    FileAttributes fileAttributes,
    DateTimeKind dateTimeKind)
    : ILogFile
{
    #region Private Fields

    private string _path = path;
    private readonly int _bufferSize = bufferSize;
    private readonly LogFile _logFile = new(path, encoding, 1024, true, formatter, fileAttributes, dateTimeKind);
    private readonly ConcurrentQueue<LogEntry> _queue = new();
    private Timer _timer;

    #endregion

    #region ILogFile Members

    string ILogFile.FileName => _logFile.FileName;

    void ILogFile.Open()
    {
        Assert.IsTrue(_timer == null);

        _timer = new Timer(TimerCallback, null, timerPeriod, timerPeriod);
    }

    public void Write(LogEntry entry)
    {
        _queue.Enqueue(entry);
    }

    public void Flush()
    {
        while (_queue.TryDequeue(out var logEntry))
        {
            var text = formatter.Format(logEntry);
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