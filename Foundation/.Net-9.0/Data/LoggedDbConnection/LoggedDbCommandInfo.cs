using System.Data;

namespace Foundation.Data.LoggedDbConnection;

public sealed class LoggedDbCommandInfo(
    int commandId,
    ConnectionState connectionState,
    string database,
    LoggedDbCommandExecutionType executionType,
    CommandType commandType,
    int commandTimeout,
    string commandText,
    string parameters)
{
    public int CommandId { get; } = commandId;
    public ConnectionState ConnectionState { get; } = connectionState;
    public string Database { get; } = database;
    public LoggedDbCommandExecutionType ExecutionType { get; } = executionType;
    public CommandType CommandType { get; } = commandType;
    public int CommandTimeout { get; } = commandTimeout;
    public string CommandText { get; } = commandText;
    public string Parameters { get; } = parameters;
}