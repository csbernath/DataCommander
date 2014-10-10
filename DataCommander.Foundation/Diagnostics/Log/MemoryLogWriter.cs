namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class MemoryLogWriter : ILogWriter
    {
        private readonly ICollection<LogEntry> logEntries;

        /// <summary>
        /// 
        /// </summary>
        public MemoryLogWriter()
        {
            this.logEntries = new List<LogEntry>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<LogEntry> LogEntries
        {
            get
            {
                return this.logEntries;
            }
        }

        #region ILogWriter Members

        void ILogWriter.Open()
        {
        }

        void ILogWriter.Write( LogEntry logEntry )
        {
            lock (this.logEntries)
            {
                this.logEntries.Add( logEntry );
            }
        }

        void ILogWriter.Flush()
        {
        }

        void ILogWriter.Close()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}