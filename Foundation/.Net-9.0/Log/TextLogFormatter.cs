namespace Foundation.Log;

public sealed class TextLogFormatter : ILogFormatter
{
    internal static string Format(LogEntry entry)
    {
        char logLevelChar = GetLogLevelChar(entry);
        string result =
            $"[{entry.CreationTime:HH:mm:ss.fff}|{entry.Id}|{logLevelChar}|{entry.ThreadName},{entry.ManagedThreadId}|{entry.LogName}] {entry.Message}\r\n";
        return result;
    }

    string ILogFormatter.Begin() => null;
    string ILogFormatter.Format(LogEntry entry) => Format(entry);
    string ILogFormatter.End() => null;

    private static char GetLogLevelChar(LogEntry entry)
    {
        char logLevelChar = entry.LogLevel switch
        {
            LogLevel.Debug => 'D',
            LogLevel.Error => 'E',
            LogLevel.Information => 'I',
            LogLevel.Trace => 'T',
            LogLevel.Warning => 'W',
            _ => '?',
        };
        return logLevelChar;
    }
}