namespace Foundation.Log
{
    public interface ILogFormatter
    {
        string Begin();
        string Format(LogEntry entry);
        string End();
    }
}