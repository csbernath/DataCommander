using System;

namespace Foundation.Data.LoggedDbConnection;

public sealed class AfterExecuteCommandEventArgs(LoggedDbCommandInfo command, Exception exception) : LoggedEventArgs
{
    public LoggedDbCommandInfo Command { get; } = command;
    public Exception Exception { get; } = exception;
}