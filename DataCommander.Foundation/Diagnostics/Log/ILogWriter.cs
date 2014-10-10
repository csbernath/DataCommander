namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ILogWriter : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        void Open();

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="logEntry"></param>
        void Write(LogEntry logEntry);

        /// <summary>
        /// 
        /// </summary>
        void Flush();

        /// <summary>
        /// Closes the LogWriter (e.g. file, database connection etc.)
        /// </summary>
        void Close();
    }
}