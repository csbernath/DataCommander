namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class SqlLogWriter : ILogWriter
    {
        private static ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();
        private const Int32 Period = 10000;
        private Func<IDbConnection> createConnection;
        private Func<LogEntry, String> logEntryToCommandText;
        private Int32 commandTimeout;
        private SingleThreadPool singleThreadPool;
        private List<LogEntry> entryQueue = new List<LogEntry>();
        private Object lockObject = new Object();
        private Timer timer;
        private Int32 pooledItemCount;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createConnection"></param>
        /// <param name="logEntryToCommandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="singleThreadPool"></param>
        public SqlLogWriter(
            Func<IDbConnection> createConnection,
            Func<LogEntry, String> logEntryToCommandText,
            int commandTimeout,
            SingleThreadPool singleThreadPool )
        {
            Contract.Requires( createConnection != null );
            Contract.Requires( logEntryToCommandText != null );
            Contract.Requires( singleThreadPool != null );

            this.createConnection = createConnection;
            this.logEntryToCommandText = logEntryToCommandText;
            this.singleThreadPool = singleThreadPool;
            this.commandTimeout = commandTimeout;
        }

        #region ILogWriter Members

        void ILogWriter.Open()
        {
            this.timer = new Timer( this.TimerCallback, null, 0, Period );
        }

        void ILogWriter.Write( LogEntry logEntry )
        {
            lock (this.entryQueue)
            {
                this.entryQueue.Add( logEntry );
            }
        }

        private void Flush()
        {
            this.TimerCallback( null );
        }

        void ILogWriter.Flush()
        {
            this.Flush();
        }

        void ILogWriter.Close()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            this.Flush();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO
        }

        #endregion

        private void TimerCallback( Object state )
        {
            lock (this.lockObject)
            {
                if (this.entryQueue.Count > 0)
                {
                    if (this.timer != null)
                    {
                        this.timer.Change( Timeout.Infinite, Timeout.Infinite );
                    }

                    LogEntry[] array;

                    lock (this.entryQueue)
                    {
                        Int32 count = this.entryQueue.Count;
                        array = new LogEntry[ count ];
                        this.entryQueue.CopyTo( array );
                        this.entryQueue.Clear();
                    }

                    this.singleThreadPool.QueueUserWorkItem( this.WaitCallback, array );
                    this.pooledItemCount++;

                    if (this.timer != null)
                    {
                        this.timer.Change( Period, Period );
                    }
                }
            }
        }

        private void WaitCallback( Object state )
        {
            try
            {
                LogEntry[] array = (LogEntry[]) state;
                StringBuilder sb = new StringBuilder();
                String commandText;

                for (Int32 i = 0; i < array.Length; i++)
                {
                    commandText = this.logEntryToCommandText( array[ i ] );
                    sb.AppendLine( commandText );
                }

                commandText = sb.ToString();

                using (IDbConnection connection = this.createConnection())
                {
                    connection.Open();
                    IDbCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = commandText;
                    command.CommandTimeout = this.commandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error( e.ToLogString() );
            }

            this.pooledItemCount--;
        }
    }
}