using Foundation.Log;

namespace Foundation.DefaultLog;

public sealed class TextLogFormatter : ILogFormatter
{
    internal static string Format(LogEntry entry)
    {
        var logLevelChar = GetLogLevelChar(entry);
        var result =
            $"[{entry.CreationTime:HH:mm:ss.fff}|{entry.Id}|{logLevelChar}|{entry.ThreadName},{entry.ManagedThreadId}|{entry.LogName}] {entry.Message}\r\n";
        return result;
    }

    string ILogFormatter.Begin() => null;
    string ILogFormatter.Format(LogEntry entry) => Format(entry);
    string ILogFormatter.End() => null;

    private static char GetLogLevelChar(LogEntry entry)
    {
        char logLevelChar;
        switch (entry.LogLevel)
        {
            case LogLevel.Debug:
                logLevelChar = 'D';
                break;

            case LogLevel.Error:
                logLevelChar = 'E';
                break;

            case LogLevel.Information:
                logLevelChar = 'I';
                break;

            case LogLevel.Trace:
                logLevelChar = 'T';
                break;

            case LogLevel.Warning:
                logLevelChar = 'W';
                break;

            default:
                logLevelChar = '?';
                break;
        }

        return logLevelChar;
    }
}