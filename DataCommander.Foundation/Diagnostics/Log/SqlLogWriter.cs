namespace DataCommander.Foundation.Diagnostics.Log
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class SqlLogWriter : ILogWriter
    {
        #region Private Fields

        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof (SqlLogWriter));
        private const int Period = 10000;
        private readonly Func<IDbConnection> _createConnection;
        private readonly Func<LogEntry, string> _logEntryToCommandText;
        private readonly int _commandTimeout;
        private readonly SingleThreadPool _singleThreadPool;
        private readonly List<LogEntry> _entryQueue = new List<LogEntry>();
        private readonly object _lockObject = new object();
        private Timer _timer;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createConnection"></param>
        /// <param name="logEntryToCommandText"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="singleThreadPool"></param>
        public SqlLogWriter(
            Func<IDbConnection> createConnection,
            Func<LogEntry, string> logEntryToCommandText,
            int commandTimeout,
            SingleThreadPool singleThreadPool)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(createConnection != null);
            Contract.Requires<ArgumentNullException>(logEntryToCommandText != null);
            Contract.Requires<ArgumentNullException>(singleThreadPool != null);
#endif

            this._createConnection = createConnection;
            this._logEntryToCommandText = logEntryToCommandText;
            this._singleThreadPool = singleThreadPool;
            this._commandTimeout = commandTimeout;
        }

#region ILogWriter Members

        void ILogWriter.Open()
        {
            this._timer = new Timer(this.TimerCallback, null, 0, Period);
        }

        void ILogWriter.Write(LogEntry logEntry)
        {
            lock (this._entryQueue)
            {
                this._entryQueue.Add(logEntry);
            }
        }

        private void Flush()
        {
            this.TimerCallback(null);
        }

        void ILogWriter.Flush()
        {
            this.Flush();
        }

        void ILogWriter.Close()
        {
            if (this._timer != null)
            {
                this._timer.Dispose();
                this._timer = null;
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

        private void TimerCallback(object state)
        {
            lock (this._lockObject)
            {
                if (this._entryQueue.Count > 0)
                {
                    if (this._timer != null)
                    {
                        this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                    }

                    LogEntry[] array;

                    lock (this._entryQueue)
                    {
                        var count = this._entryQueue.Count;
                        array = new LogEntry[count];
                        this._entryQueue.CopyTo(array);
                        this._entryQueue.Clear();
                    }

                    this._singleThreadPool.QueueUserWorkItem(this.WaitCallback, array);

                    if (this._timer != null)
                    {
                        this._timer.Change(Period, Period);
                    }
                }
            }
        }

        private void WaitCallback(object state)
        {
            try
            {
                var array = (LogEntry[])state;
                var sb = new StringBuilder();
                string commandText;

                for (var i = 0; i < array.Length; i++)
                {
                    commandText = this._logEntryToCommandText(array[i]);
                    sb.AppendLine(commandText);
                }

                commandText = sb.ToString();

                using (var connection = this._createConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = commandText;
                    command.CommandTimeout = this._commandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToLogString());
            }
        }
    }
}