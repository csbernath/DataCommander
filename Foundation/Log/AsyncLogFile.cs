using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace Foundation.Log
{
    internal sealed class AsyncLogFile : ILogFile
    {
        #region Private Fields

        private string _path;
        private readonly int _bufferSize;
        private readonly LogFile _logFile;
        private readonly ILogFormatter _formatter;
        private readonly ConcurrentQueue<LogEntry> _queue;
        private readonly TimeSpan _timerPeriod;
        private Timer _timer;

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
            this._path = path;
            this._bufferSize = bufferSize;
            this._timerPeriod = timerPeriod;
            this._queue = new ConcurrentQueue<LogEntry>();
            this._formatter = formatter;
            this._logFile = new LogFile(path, encoding, 1024, true, formatter, fileAttributes, dateTimeKind);
        }

        #region ILogFile Members

        string ILogFile.FileName => this._logFile.FileName;

        void ILogFile.Open()
        {
#if CONTRACTS_FULL
            Contract.Assert(this.timer == null);
#endif

            this._timer = new Timer(this.TimerCallback, null, this._timerPeriod, this._timerPeriod);
        }

        public void Write(LogEntry entry)
        {
            this._queue.Enqueue(entry);
        }

        public void Flush()
        {
            LogEntry logEntry;
            while (this._queue.TryDequeue(out logEntry))
            {
                var text = this._formatter.Format(logEntry);
                this._logFile.Write(logEntry.CreationTime, text);
            }
        }

        public void Close()
        {
            if (this._timer != null)
            {
                this._timer.Dispose();
                this._timer = null;
            }

            this.Flush();

            this._logFile.Close();
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