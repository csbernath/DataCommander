namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal interface ILogFormatter
    {
        string Begin();
        string Format( LogEntry entry );
        string End();
    }
}