namespace DataCommander.Foundation.Diagnostics
{
    internal interface ILogFormatter
    {
        string Begin();
        string Format( LogEntry entry );
        string End();
    }
}