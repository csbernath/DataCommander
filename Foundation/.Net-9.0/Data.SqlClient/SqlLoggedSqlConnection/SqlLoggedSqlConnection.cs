using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Foundation.Core;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection;

/// <summary>
/// Logged SqlConnection class.
/// </summary>
public sealed class SqlLoggedSqlConnection : IDbConnection
{
    private readonly int _applicationId;
    private int _connectionNo;
    private readonly SqlLog.SqlLog _sqlLog;

    public SqlLoggedSqlConnection(SqlLog.SqlLog sqlLog, int applicationId, string userName, string hostName, string? connectionString,
        ISqlLoggedSqlCommandFilter filter)
    {
        ArgumentNullException.ThrowIfNull(sqlLog);

        _sqlLog = sqlLog;
        _applicationId = applicationId;
        UserName = userName;
        HostName = hostName;
        Filter = filter;
        Connection = new SqlConnection(connectionString);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
            _sqlLog.DisposeConnection(Connection);
    }

    IDbTransaction IDbConnection.BeginTransaction() => Connection.BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel il) => Connection.BeginTransaction(il);
    void IDbConnection.ChangeDatabase(string databaseName) => Connection.ChangeDatabase(databaseName);
    public void Close() => _sqlLog.CloseConnection(Connection);

    IDbCommand IDbConnection.CreateCommand()
    {
        IDbCommand command = Connection.CreateCommand();
        return new SqlLoggedSqlCommand(this, command);
    }

    public void Open()
    {
        Exception exception = null;
        var startDate = LocalTime.Default.Now;
        var duration = Stopwatch.GetTimestamp();

        try
        {
            Connection.Open();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            duration = Stopwatch.GetTimestamp() - duration;
            _connectionNo = _sqlLog.ConnectionOpen(_applicationId, Connection, UserName, HostName, startDate, duration, exception);
        }
    }

    [AllowNull]
    public string ConnectionString
    {
        get => Connection.ConnectionString;
        set => Connection.ConnectionString = value;
    }

    public int ConnectionTimeout => Connection.ConnectionTimeout;
    public string Database => Connection.Database;
    public ConnectionState State => Connection.State;

    internal void CommandExeucte(
        IDbCommand command,
        DateTime startDate,
        long duration,
        Exception? exception) => _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);

    internal int ExecuteNonQuery(IDbCommand command)
    {
        int count;
        Exception exception = null;
        var startDate = LocalTime.Default.Now;
        var duration = Stopwatch.GetTimestamp();

        try
        {
            count = command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            duration = Stopwatch.GetTimestamp() - duration;
            var contains = exception != null || Filter == null || Filter.Contains(UserName, HostName, command);

            if (contains)
            {
                _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);
            }
        }

        return count;
    }

    internal object ExecuteScalar(IDbCommand command)
    {
        object scalar = null;

        Exception exception = null;
        var startDate = LocalTime.Default.Now;
        var duration = Stopwatch.GetTimestamp();

        try
        {
            scalar = command.ExecuteScalar();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            duration = Stopwatch.GetTimestamp() - duration;
            var contains = exception != null || Filter == null || Filter.Contains(UserName, HostName, command);

            if (contains)
            {
                _sqlLog.CommandExecute(_applicationId, _connectionNo, command, startDate, duration, exception);
            }
        }

        return scalar;
    }

    public ISqlLoggedSqlCommandFilter Filter { get; }
    public string UserName { get; }
    public string HostName { get; }
    internal SqlConnection Connection { get; }
}