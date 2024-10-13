using System;
using System.Data.Common;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.Data.LoggedDbConnection;

internal sealed class DbConnectionLogger
{
    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(DbConnectionLogger));
    private readonly LoggedDbConnection _connection;
    private BeforeOpenDbConnectionEventArgs? _beforeOpen;
    private BeforeExecuteCommandEventArgs? _beforeExecuteReader;

    public DbConnectionLogger(LoggedDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        _connection = connection;

        connection.BeforeOpen += ConnectionBeforeOpen;
        connection.AfterOpen += ConnectionAfterOpen;
        connection.BeforeExecuteReader += ConnectionBeforeExecuteReader;
        connection.AfterExecuteReader += ConnectionAfterExecuteReader;
        connection.AfterRead += ConnectionAfterRead;
    }

    private void ConnectionBeforeOpen(object? sender, BeforeOpenDbConnectionEventArgs e)
    {
        var csb = new DbConnectionStringBuilder { ConnectionString = e.ConnectionString };

        if (csb.ContainsKey("Password"))
        {
            csb["Password"] = "<not logged here>";
        }

        Log.Trace("Opening connection {0}...", csb.ConnectionString);

        _beforeOpen = e;
    }

    private void ConnectionAfterOpen(object? sender, AfterOpenDbConnectionEventArgs e)
    {
        var duration = e.Timestamp - _beforeOpen!.Timestamp;
        if (e.Exception != null)
            Log.Write(LogLevel.Error, "Opening connection finished in {0} seconds. Exception:\r\n{1}", StopwatchTimeSpan.ToString(duration, 3),
                e.Exception.ToLogString());
        else
            Log.Trace("Opening connection finished in {0} seconds.", StopwatchTimeSpan.ToString(duration, 3));

        _beforeOpen = null;
    }

    private void ConnectionBeforeExecuteReader(object? sender, BeforeExecuteCommandEventArgs e) => _beforeExecuteReader = e;

    private static string ToString(LoggedDbCommandInfo command, long duration) =>
        $"Executing command started in {StopwatchTimeSpan.ToString(duration, 3)} seconds.\r\ncommandId: {command.CommandId},connectionState: {command.ConnectionState},database: {command.Database},executionType: {command.ExecutionType},commandType: {command.CommandType},commandTimeout: {command.CommandTimeout}\r\ncommandText: {command.CommandText}\r\nparameters:\r\n{command.Parameters}";

    private void ConnectionAfterExecuteReader(object? sender, AfterExecuteCommandEventArgs e)
    {
        var duration = e.Timestamp - _beforeExecuteReader!.Timestamp;
        if (e.Exception != null)
        {
            Log.Write(LogLevel.Error, "{0}\r\nException:\r\n{1}", ToString(e.Command, duration), e.Exception.ToLogString());
            _beforeExecuteReader = null;
        }
        else
            Log.Trace("{0}", ToString(e.Command, duration));
    }

    private void ConnectionAfterRead(object? sender, AfterReadEventArgs e)
    {
        var duration = e.Timestamp - _beforeExecuteReader!.Timestamp;
        Log.Trace("{0} row(s) read in {1} seconds.", e.RowCount, StopwatchTimeSpan.ToString(duration, 3));
    }
}