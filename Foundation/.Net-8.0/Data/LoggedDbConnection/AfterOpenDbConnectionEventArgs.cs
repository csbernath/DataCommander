using System;

namespace Foundation.Data.LoggedDbConnection;

public sealed class AfterOpenDbConnectionEventArgs(Exception exception) : LoggedEventArgs
{
    public Exception Exception { get; } = exception;
}