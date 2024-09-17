using System;
using System.Data;
using System.Diagnostics;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.Data;

/// <summary>
/// Safe IDbConnection wrapper for Windows Services.
/// </summary>
public class SafeDbConnection : IDbConnection
{
    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(SafeDbConnection));
    private ISafeDbConnection _safeDbConnection;

    protected SafeDbConnection()
    {
    }

    public IDbConnection Connection { get; private set; }

    protected void Initialize(IDbConnection connection, ISafeDbConnection safeDbConnection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(safeDbConnection);

        Connection = connection;
        _safeDbConnection = safeDbConnection;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }
    }

    public IDbTransaction BeginTransaction() => Connection.BeginTransaction();
    IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) => Connection.BeginTransaction(il);
    public void ChangeDatabase(string databaseName) => Connection.ChangeDatabase(databaseName);
    public void Close() => Connection.Close();

    public IDbCommand CreateCommand()
    {
        var command = Connection.CreateCommand();
        return new SafeDbCommand(this, command);
    }

    public void Open()
    {
        var count = 0;

        while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
        {
            count++;
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                Connection.Open();
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds >= 100)
                {
                    Log.Trace("SafeDbConnection.Open() finished. {0}, count: {1}, elapsed: {2}",
                        Connection.ConnectionString, count, stopwatch.Elapsed);
                }

                break;
            }
            catch (Exception e)
            {
                _safeDbConnection.HandleException(e, stopwatch.Elapsed);
            }
        }
    }

    public string ConnectionString
    {
        get => Connection.ConnectionString;

        set => Connection.ConnectionString = value;
    }

    public int ConnectionTimeout => Connection.ConnectionTimeout;
    public string Database => Connection.Database;
    public ConnectionState State => Connection.State;

    internal IDataReader ExecuteReader(IDbCommand command, CommandBehavior behavior)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (Connection.State != ConnectionState.Open)
            Open();

        IDataReader reader = null;

        while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
        {
            var ticks = Stopwatch.GetTimestamp();

            try
            {
                reader = command.ExecuteReader(behavior);
                break;
            }
            catch (Exception e)
            {
                ticks = Stopwatch.GetTimestamp() - ticks;

                if (reader != null)
                {
                    reader.Dispose();
                }

                var state = Connection.State;

                Log.Write(
                    LogLevel.Error,
                    "command.CommandText: {0}\r\nExecution time: {1}, command.CommandTimeout: {2}, connection.State: {3}\r\n{4}",
                    command.CommandText,
                    StopwatchTimeSpan.ToString(ticks, 3),
                    command.CommandTimeout,
                    state,
                    e.ToLogString());

                if (state == ConnectionState.Open)
                {
                    _safeDbConnection.HandleException(e, command);
                }
                else
                {
                    Open();
                }
            }
        }

        return reader;
    }

    internal object ExecuteScalar(IDbCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        object scalar = null;

        if (Connection.State != ConnectionState.Open)
        {
            Open();
        }

        while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
        {
            try
            {
                scalar = command.ExecuteScalar();
                break;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());

                if (Connection.State == ConnectionState.Open)
                {
                    _safeDbConnection.HandleException(e, command);
                }
                else
                {
                    Open();
                }
            }
        }

        return scalar;
    }

    internal int ExecuteNonQuery(IDbCommand command)
    {
        if (Connection.State != ConnectionState.Open)
            Open();

        var count = 0;
        var tryCount = 0;

        while (tryCount == 0 || !_safeDbConnection.CancellationToken.IsCancellationRequested)
        {
            try
            {
                count = command.ExecuteNonQuery();
                break;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());

                if (Connection.State == ConnectionState.Open)
                {
                    _safeDbConnection.HandleException(e, command);
                }
                else
                {
                    Open();
                }
            }

            tryCount++;
        }

        return count;
    }
}