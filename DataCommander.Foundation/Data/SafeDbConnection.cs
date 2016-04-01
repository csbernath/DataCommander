namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// Safe IDbConnection wrapper for Windows Services.
    /// </summary>
    public class SafeDbConnection : IDbConnection
    {
        private static readonly ILog log = LogFactory.Instance.GetTypeLog(typeof (SafeDbConnection));
        private ISafeDbConnection safeDbConnection;

        /// <summary>
        /// 
        /// </summary>
        protected SafeDbConnection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="safeDbConnection"></param>
        protected void Initialize(
            IDbConnection connection,
            ISafeDbConnection safeDbConnection)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(safeDbConnection != null);

            this.Connection = connection;
            this.safeDbConnection = safeDbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Connection != null)
                {
                    this.Connection.Dispose();
                    this.Connection = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            return this.Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return this.Connection.BeginTransaction(il);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase(string databaseName)
        {
            this.Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.Connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            IDbCommand command = this.Connection.CreateCommand();
            return new SafeDbCommand(this, command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            int count = 0;

            while (!this.safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                count++;
                var stopwatch = new Stopwatch();

                try
                {
                    stopwatch.Start();
                    this.Connection.Open();
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds >= 100)
                    {
                        log.Trace("SafeDbConnection.Open() finished. {0}, count: {1}, elapsed: {2}",
                            this.Connection.ConnectionString, count, stopwatch.Elapsed);
                    }

                    break;
                }
                catch (Exception e)
                {
                    this.safeDbConnection.HandleException(e, stopwatch.Elapsed);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get { return this.Connection.ConnectionString; }

            set { this.Connection.ConnectionString = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => this.Connection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => this.Connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => this.Connection.State;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        internal IDataReader ExecuteReader(IDbCommand command, CommandBehavior behavior)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            if (this.Connection.State != ConnectionState.Open)
            {
                this.Open();
            }

            IDataReader reader = null;

            while (!this.safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                long ticks = Stopwatch.GetTimestamp();

                try
                {
                    reader = command.ExecuteReader(behavior);
                    break;
                }
                catch (Exception e)
                {
                    ticks = Stopwatch.GetTimestamp() - ticks;

                    if (reader != null)
                    {
                        reader.Dispose();
                    }

                    ConnectionState state = this.Connection.State;

                    log.Write(
                        LogLevel.Error,
                        "command.CommandText: {0}\r\nExecution time: {1}, command.CommandTimeout: {2}, connection.State: {3}\r\n{4}",
                        command.CommandText,
                        StopwatchTimeSpan.ToString(ticks, 3),
                        command.CommandTimeout,
                        state,
                        e.ToLogString());

                    if (state == ConnectionState.Open)
                    {
                        this.safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        this.Open();
                    }
                }
            }

            return reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal object ExecuteScalar(IDbCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            object scalar = null;

            if (this.Connection.State != ConnectionState.Open)
            {
                this.Open();
            }

            while (!this.safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    scalar = command.ExecuteScalar();
                    break;
                }
                catch (Exception e)
                {
                    log.Write(LogLevel.Error, e.ToLogString());

                    if (this.Connection.State == ConnectionState.Open)
                    {
                        this.safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        this.Open();
                    }
                }
            }

            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal int ExecuteNonQuery(IDbCommand command)
        {
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Open();
            }

            int count = 0;
            int tryCount = 0;

            while (tryCount == 0 || !this.safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    count = command.ExecuteNonQuery();
                    break;
                }
                catch (Exception e)
                {
                    log.Write(LogLevel.Error, e.ToLogString());

                    if (this.Connection.State == ConnectionState.Open)
                    {
                        this.safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        this.Open();
                    }
                }

                tryCount++;
            }

            return count;
        }

        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(this.Connection != null);
        }
    }
}