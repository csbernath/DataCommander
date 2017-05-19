namespace DataCommander.Foundation.Diagnostics.Log
{
    internal interface ILogFormatter
    {
        string Begin();
        string Format( LogEntry entry );
        string End();
    }
}