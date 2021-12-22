using System;

namespace Foundation.Log;

public interface ILogWriter : IDisposable
{
    void Open();

    /// <summary>
    /// Writes a message to the output.
    /// </summary>
    /// <param name="logEntry"></param>
    void Write(LogEntry logEntry);

    void Flush();

    /// <summary>
    /// Closes the LogWriter (e.g. file, database connection etc.)
    /// </summary>
    void Close();
}