using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using Foundation.InternalLog;
using Foundation.Log;
using Foundation.Threading;

namespace Foundation.DefaultLog;

internal sealed class SqlLogWriter : ILogWriter
{
    private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(SqlLogWriter));
    private const int Period = 10000;
    private readonly Func<IDbConnection> _createConnection;
    private readonly Func<LogEntry, string> _logEntryToCommandText;
    private readonly int _commandTimeout;
    private readonly SingleThreadPool _singleThreadPool;
    private readonly List<LogEntry> _entryQueue = [];
    private readonly object _lockObject = new();
    private Timer? _timer;

    public SqlLogWriter(
        Func<IDbConnection> createConnection,
        Func<LogEntry, string> logEntryToCommandText,
        int commandTimeout,
        SingleThreadPool singleThreadPool)
    {
        ArgumentNullException.ThrowIfNull(createConnection);
        ArgumentNullException.ThrowIfNull(logEntryToCommandText);
        ArgumentNullException.ThrowIfNull(singleThreadPool);

        _createConnection = createConnection;
        _logEntryToCommandText = logEntryToCommandText;
        _singleThreadPool = singleThreadPool;
        _commandTimeout = commandTimeout;
    }

    void ILogWriter.Open() => _timer = new Timer(TimerCallback, null, 0, Period);

    void ILogWriter.Write(LogEntry logEntry)
    {
        lock (_entryQueue)
        {
            _entryQueue.Add(logEntry);
        }
    }

    private void Flush() => TimerCallback(null);

    void ILogWriter.Flush() => Flush();

    void ILogWriter.Close()
    {
        if (_timer != null)
        {
            _timer.Dispose();
            _timer = null;
        }

        Flush();
    }

    void IDisposable.Dispose()
    {
        // TODO
    }

    private void TimerCallback(object? state)
    {
        lock (_lockObject)
        {
            if (_entryQueue.Count > 0)
            {
                if (_timer != null)
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);

                LogEntry[] array;

                lock (_entryQueue)
                {
                    var count = _entryQueue.Count;
                    array = new LogEntry[count];
                    _entryQueue.CopyTo(array);
                    _entryQueue.Clear();
                }

                _singleThreadPool.QueueUserWorkItem(WaitCallback, array);

                if (_timer != null)
                    _timer.Change(Period, Period);
            }
        }
    }

    private void WaitCallback(object? state)
    {
        try
        {
            var array = (LogEntry[]?)state!;
            var sb = new StringBuilder();
            string commandText;

            for (var i = 0; i < array.Length; i++)
            {
                commandText = _logEntryToCommandText(array[i]);
                sb.AppendLine(commandText);
            }

            commandText = sb.ToString();

            using var connection = _createConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = commandText;
            command.CommandTimeout = _commandTimeout;
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
}