namespace Foundation.Log
{
    internal interface ILogFormatter
    {
        string Begin();
        string Format( LogEntry entry );
        string End();
    }
}