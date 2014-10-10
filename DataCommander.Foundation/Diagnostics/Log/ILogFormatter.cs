namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal interface ILogFormatter
    {
        String Begin();
        String Format( LogEntry entry );
        String End();
    }
}