using System;

namespace Foundation.Data.LoggedDbConnection;

public sealed class AfterExecuteCommandEventArgs : LoggedEventArgs
{
    public AfterExecuteCommandEventArgs(LoggedDbCommandInfo command, Exception exception)
    {
        Command = command;
        Exception = exception;
    }

    public LoggedDbCommandInfo Command { get; }
    public Exception Exception { get; }
}