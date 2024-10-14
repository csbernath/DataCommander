using System;
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
    private readonly LogFile _logFile = new(path, encoding, bufferSize, true, formatter, fileAttributes, dateTimeKind);
    private readonly ConcurrentQueue<LogEntry> _queue = new();
    private Timer? _timer;

    string? ILogFile.FileName => _logFile.FileName;

    void ILogFile.Open()
    {
        Assert.IsTrue(_timer == null);

        _timer = new Timer(TimerCallback, null, timerPeriod, timerPeriod);
    }

    public void Write(LogEntry entry) => _queue.Enqueue(entry);

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

    private void TimerCallback(object? state)
    {
        var thread = Thread.CurrentThread;
        thread.Priority = ThreadPriority.Lowest;

        Flush();
    }

    void IDisposable.Dispose() => Close();
}