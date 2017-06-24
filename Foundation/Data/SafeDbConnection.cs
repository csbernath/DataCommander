using System;
using System.Data;
using System.Diagnostics;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;

namespace Foundation.Data
{
    /// <summary>
    /// Safe IDbConnection wrapper for Windows Services.
    /// </summary>
    public class SafeDbConnection : IDbConnection
    {
        private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof (SafeDbConnection));
        private ISafeDbConnection _safeDbConnection;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(safeDbConnection != null);
#endif

            Connection = connection;
            _safeDbConnection = safeDbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return Connection.BeginTransaction(il);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            Connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            var command = Connection.CreateCommand();
            return new SafeDbCommand(this, command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            var count = 0;

            while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                count++;
                var stopwatch = new Stopwatch();

                try
                {
                    stopwatch.Start();
                    Connection.Open();
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds >= 100)
                    {
                        Log.Trace("SafeDbConnection.Open() finished. {0}, count: {1}, elapsed: {2}",
                            Connection.ConnectionString, count, stopwatch.Elapsed);
                    }

                    break;
                }
                catch (Exception e)
                {
                    _safeDbConnection.HandleException(e, stopwatch.Elapsed);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get => Connection.ConnectionString;

            set => Connection.ConnectionString = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => Connection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => Connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => Connection.State;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        internal IDataReader ExecuteReader(IDbCommand command, CommandBehavior behavior)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            if (Connection.State != ConnectionState.Open)
            {
                Open();
            }

            IDataReader reader = null;

            while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                var ticks = Stopwatch.GetTimestamp();

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

                    var state = Connection.State;

                    Log.Write(
                        LogLevel.Error,
                        "command.CommandText: {0}\r\nExecution time: {1}, command.CommandTimeout: {2}, connection.State: {3}\r\n{4}",
                        command.CommandText,
                        StopwatchTimeSpan.ToString(ticks, 3),
                        command.CommandTimeout,
                        state,
                        e.ToLogString());

                    if (state == ConnectionState.Open)
                    {
                        _safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        Open();
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            object scalar = null;

            if (Connection.State != ConnectionState.Open)
            {
                Open();
            }

            while (!_safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    scalar = command.ExecuteScalar();
                    break;
                }
                catch (Exception e)
                {
                    Log.Write(LogLevel.Error, e.ToLogString());

                    if (Connection.State == ConnectionState.Open)
                    {
                        _safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        Open();
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
            if (Connection.State != ConnectionState.Open)
            {
                Open();
            }

            var count = 0;
            var tryCount = 0;

            while (tryCount == 0 || !_safeDbConnection.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    count = command.ExecuteNonQuery();
                    break;
                }
                catch (Exception e)
                {
                    Log.Write(LogLevel.Error, e.ToLogString());

                    if (Connection.State == ConnectionState.Open)
                    {
                        _safeDbConnection.HandleException(e, command);
                    }
                    else
                    {
                        Open();
                    }
                }

                tryCount++;
            }

            return count;
        }

        //[ContractInvariantMethod]
        private void ContractInvariant()
        {
#if CONTRACTS_FULL
            Contract.Invariant(this.Connection != null);
#endif
        }
    }
}