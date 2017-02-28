namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Threading;

    internal sealed class AsyncLogFile : ILogFile
    {
        #region Private Fields

        private string path;
        private readonly int bufferSize;
        private readonly LogFile logFile;
        private readonly ILogFormatter formatter;
        private readonly ConcurrentQueue<LogEntry> queue;
        private readonly TimeSpan timerPeriod;
        private Timer timer;

        #endregion

        public AsyncLogFile(
            string path,
            Encoding encoding,
            int bufferSize,
            TimeSpan timerPeriod,
            ILogFormatter formatter,
            FileAttributes fileAttributes,
            DateTimeKind dateTimeKind)
        {
            this.path = path;
            this.bufferSize = bufferSize;
            this.timerPeriod = timerPeriod;
            this.queue = new ConcurrentQueue<LogEntry>();
            this.formatter = formatter;
            this.logFile = new LogFile(path, encoding, 1024, true, formatter, fileAttributes, dateTimeKind);
        }

        #region ILogFile Members

        string ILogFile.FileName => this.logFile.FileName;

        void ILogFile.Open()
        {
            Contract.Assert(this.timer == null);

            this.timer = new Timer(this.TimerCallback, null, this.timerPeriod, this.timerPeriod);
        }

        public void Write(LogEntry entry)
        {
            this.queue.Enqueue(entry);
        }

        public void Flush()
        {
            LogEntry logEntry;
            while (this.queue.TryDequeue(out logEntry))
            {
                var text = this.formatter.Format(logEntry);
                this.logFile.Write(logEntry.CreationTime, text);
            }
        }

        public void Close()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            this.Flush();

            this.logFile.Close();
        }

        #endregion

        #region Private Methods

        private void TimerCallback(object state)
        {
            var thread = Thread.CurrentThread;
            thread.Priority = ThreadPriority.Lowest;

            this.Flush();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Close();
        }

        #endregion
    }
}