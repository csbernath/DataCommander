namespace DataCommander.Foundation.Diagnostics.Log
{
    internal sealed class TextLogFormatter : ILogFormatter
    {
        internal static string Format(LogEntry entry)
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

            return
                $"[{entry.CreationTime.ToString("HH:mm:ss.fff")}|{entry.Id}|{logLevelChar}|{entry.ThreadName},{entry.ManagedThreadId}|{entry.LogName}] {entry.Message}\r\n";
        }

        string ILogFormatter.Begin()
        {
            return null;
        }

        string ILogFormatter.Format(LogEntry entry)
        {
            return Format(entry);
        }

        string ILogFormatter.End()
        {
            return null;
        }
    }
}