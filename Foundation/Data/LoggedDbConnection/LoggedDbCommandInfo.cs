using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public sealed class LoggedDbCommandInfo
{
    public LoggedDbCommandInfo(int commandId, ConnectionState connectionState, string database, LoggedDbCommandExecutionType executionType,
        CommandType commandType, int commandTimeout, string commandText, string parameters)
    {
        CommandId = commandId;
        ConnectionState = connectionState;
        Database = database;
        ExecutionType = executionType;
        CommandType = commandType;
        CommandText = commandText;
        CommandTimeout = commandTimeout;
        Parameters = parameters;
    }

    public int CommandId { get; }
    public ConnectionState ConnectionState { get; }
    public string Database { get; }
    public LoggedDbCommandExecutionType ExecutionType { get; }
    public CommandType CommandType { get; }
    public int CommandTimeout { get; }
    public string CommandText { get; }
    public string Parameters { get; }
}