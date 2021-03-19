using System;

namespace Foundation.Data.LoggedDbConnection
{
    public sealed class AfterOpenDbConnectionEventArgs : LoggedEventArgs
    {
        public AfterOpenDbConnectionEventArgs(Exception exception) => Exception = exception;

        public Exception Exception { get; }
    }
}