namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Threading;

    internal sealed class AsyncLogFile : ILogFile
    {
        private String path;
        private readonly Int32 bufferSize;
        private readonly LogFile logFile;
        private readonly ILogFormatter formatter;
        private readonly Queue<LogEntry> queue;
        private readonly TimeSpan timerPeriod;
        private Timer timer;
        private readonly Object flushLock = new Object();

        public AsyncLogFile(
            String path,
            Encoding encoding,
            Int32 queueCapacity,
            Int32 bufferSize,
            TimeSpan timerPeriod,
            ILogFormatter formatter,
            FileAttributes fileAttributes )
        {
            this.path = path;
            this.bufferSize = bufferSize;
            this.timerPeriod = timerPeriod;
            this.queue = new Queue<LogEntry>( queueCapacity );
            this.formatter = formatter;
            this.logFile = new LogFile( path, encoding, 1024, true, formatter, fileAttributes );
        }

        #region ILogFile Members

        void ILogFile.Open()
        {
            Contract.Assert( this.timer == null );

            this.timer = new Timer( this.TimerCallback, null, this.timerPeriod, this.timerPeriod );
        }

        public void Write( LogEntry entry )
        {
            lock (this.queue)
            {
                this.queue.Enqueue( entry );
            }
        }

        public void Flush()
        {
            while (this.queue.Count > 0)
            {
                lock (this.flushLock)
                {
                    LogEntry[] entries;

                    lock (this.queue)
                    {
                        entries = new LogEntry[ this.queue.Count ];
                        this.queue.CopyTo( entries, 0 );
                        this.queue.Clear();
                    }

                    Int32 startIndex = 0;
                    while (startIndex < entries.Length)
                    {
                        startIndex = this.Write( entries, startIndex );
                    }
                }
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

        private void TimerCallback( Object state )
        {
            var thread = Thread.CurrentThread;
            thread.Priority = ThreadPriority.Lowest;

            this.Flush();
        }

        private Int32 Write( LogEntry[] entries, Int32 startIndex )
        {
            StringBuilder sb = null;
            Int32 i;
            DateTime date = DateTime.MinValue;

            for (i = startIndex; i < entries.Length; i++)
            {
                var entry = entries[ i ];

                if (i == startIndex)
                {
                    date = entry.CreationTime.Date;
                }
                else if (entry.CreationTime.Date != date)
                {
                    break;
                }

                String text = this.formatter.Format( entry );

                if (sb == null)
                {
                    sb = new StringBuilder( this.bufferSize );
                }
                else if (sb.Length + text.Length > this.bufferSize)
                {
                    this.logFile.Write( date, sb.ToString() );
                    sb.Length = 0;
                }

                sb.Append( text );
            }

            if (sb.Length > 0)
            {
                this.logFile.Write( date, sb.ToString() );
            }

            return i;
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