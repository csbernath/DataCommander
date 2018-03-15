using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;
using Foundation.Diagnostics;
using Foundation.Log;
using Foundation.Threading;

namespace Foundation.Data.SqlClient.SqlLog
{
    /// <summary>
    /// Logs SQL activity of an application into a database.
    /// </summary>
    /// <remarks>
    /// Currently only <see cref="SqlConnection"/> activity can be logged.
    /// See <see cref="SqlLoggedSqlConnection"/>.
    /// <list type="table">
    ///        <listheader>
    ///            <term>Event</term>
    ///            <description>SqlLog method</description>
    ///        </listheader>
    ///        <item>
    ///            <term>After the application started.</term>
    ///            <description><see cref="ApplicationStart"/></description>
    ///        </item>
    ///        <item>
    ///            <term>Before the application ends.</term>
    ///            <description><see cref="ApplicationEnd"/></description>
    ///        </item>
    ///        <item>
    ///            <term>After <see cref="IDbConnection.Open">IDbConnection.Open</see>.</term>
    ///            <description><see cref="ConnectionOpen"/></description>
    ///        </item>
    ///        <item>
    ///            <term>Instead of <see cref="IDbConnection.Close">IDbConnection.Close</see>.</term>
    ///            <description><see cref="CloseConnection"/></description>
    ///        </item>
    ///        <item>
    ///            <term>Instead of <see cref="IDisposable.Dispose">IDisposable.Dispose</see>.</term>
    ///            <description><see cref="DisposeConnection"/></description>
    ///        </item>
    ///        <item>
    ///            <term>After executing an <see cref="IDbCommand"/>.</term>
    ///            <description><see cref="CommandExecute"/></description>
    ///        </item>
    ///        <item>
    ///            <term></term>
    ///            <description></description>
    ///        </item>
    ///        <item>
    ///            <term></term>
    ///            <description></description>
    ///        </item>
    /// </list>
    /// The diagram of SQL Server database tables:
    /// <img src="LogDatabase.png"/>
    /// </remarks>
    public sealed class SqlLog
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof (SqlLog));
        private static readonly IInternalConnectionHelper InternalConnectionHelper;
        private int _connectionCounter;
        private readonly SafeSqlConnection _connection;
        private readonly Dictionary<int, Dictionary<string, SqLoglCommandExecution>> _applications = new Dictionary<int, Dictionary<string, SqLoglCommandExecution>>();
        private readonly Dictionary<object, SqlLogConnection> _connections = new Dictionary<object, SqlLogConnection>();
        private readonly Queue<ISqlLogItem> _queue = new Queue<ISqlLogItem>();
        private readonly AutoResetEvent _queueEvent = new AutoResetEvent(false);

        #endregion

        static SqlLog()
        {
            var version = Environment.Version;
            var major = version.Major;

            if (major == 1)
            {
                InternalConnectionHelper = new InternalConnectionHelper();
            }
            else
            {
                InternalConnectionHelper = new InternalConnectionHelper2();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLog"/> class.
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlLog(string connectionString)
        {
            Thread = new WorkerThread(Start)
            {
                Name = typeof (SqlLog).Name,
                Priority = ThreadPriority.Lowest
            };

            _connection = new SafeSqlConnection(connectionString);
        }

        /// <summary>
        /// Gets the logger thread.
        /// </summary>
        public WorkerThread Thread { get; }

        private void Flush()
        {
            while (_queue.Count > 0)
            {
                ISqlLogItem[] array;

                lock (_queue)
                {
                    array = new ISqlLogItem[_queue.Count];
                    _queue.CopyTo(array, 0);
                    _queue.Clear();
                }

                var sb = new StringBuilder();

                for (var i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    var commandText = item.CommandText;
                    sb.Append(commandText);
                }

                var cmdText = sb.ToString();
                long ticks = 0;

                try
                {
                    var command = _connection.CreateCommand();
                    command.CommandText = cmdText;
                    command.CommandTimeout = 259200;
                    ticks = Stopwatch.GetTimestamp();
                    command.ExecuteNonQuery();
                    ticks = Stopwatch.GetTimestamp() - ticks;
                }
                catch (Exception e)
                {
                    Log.Write(LogLevel.Error, e.ToString());
                }
                finally
                {
                    var seconds = (double)ticks/Stopwatch.Frequency;
                    var speed = (int)(array.Length/seconds);

                    Log.Trace(
                        "SqlLog.Flush() called. Count: {0}, Elapsed: {1}, Speed: {2} item/sec\r\n{3}",
                        array.Length,
                        StopwatchTimeSpan.ToString(ticks, 3),
                        speed,
                        cmdText);
                }
            }
        }

        private void Start()
        {
            WaitHandle[] waitHandles = { Thread.StopRequest, _queueEvent };

            while (!Thread.IsStopRequested)
            {
                var i = WaitHandle.WaitAny(waitHandles);
                CheckConnections();

                if (i == 0)
                {
                    List<object> list;

                    lock (_connections)
                    {
                        list = _connections.Keys.ToList();
                    }

                    CloseConnections(list, LocalTime.Default.Now);
                }

                Flush();
            }

            var endDate = LocalTime.Default.Now;

            lock (_applications)
            {
                foreach (var applicationId in _applications.Keys)
                {
                    var item = new SqlLogApplicationEnd(applicationId, endDate);
                    Enqueue(item);
                }
            }

            Flush();

            Log.Trace("queue.Count: {0}", _queue.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public int ApplicationStart(string name, DateTime startDate, bool safe)
        {
            var sb = new StringBuilder();
            sb.Append("exec LogApplicationStart ");
            sb.Append(name.ToTSqlVarChar());
            sb.Append(',');
            sb.Append(startDate.ToTSqlDateTime());
            var commandText = sb.ToString();

            if (_connection.State != ConnectionState.Open)
            {
                if (safe)
                    _connection.Open();
                else
                    _connection.Connection.Open();
            }

            var transactionScope = new DbTransactionScope(_connection, null);
            var applicationId = (int)transactionScope.ExecuteScalar(new CommandDefinition { CommandText = commandText });
            Log.Trace("SqlLog.ApplicationStart({0})", applicationId);
            var commands = new Dictionary<string, SqLoglCommandExecution>();

            lock (_applications)
                _applications.Add(applicationId, commands);

            return applicationId;
        }

        /// <summary>
        /// Executes the <c>LogApplicationEnd</c> stored procedure in the SQL Server database.
        /// </summary>
        /// <param name="applicationId">The identifier of the application. See <see cref="ApplicationStart"/>.</param>
        /// <param name="endDate">The end date of the application.</param>
        public void ApplicationEnd(int applicationId, DateTime endDate)
        {
            var item = new SqlLogApplicationEnd(applicationId, endDate);
            Enqueue(item);

            lock (_applications)
                _applications.Remove(applicationId);
        }

        private void ConnectionClosed(
            object internalConnection,
            DateTime endDate)
        {
            var sqlLogConnection = _connections[internalConnection];

            lock (_connections)
                _connections.Remove(internalConnection);

            if (sqlLogConnection != null)
            {
                var applicationId = sqlLogConnection.ApplicationId;
                var connectionNo = sqlLogConnection.ConnectionNo;
                var item = new SqlLogConnectionClose(applicationId, connectionNo, endDate);
                Enqueue(item);
            }
        }

        private void CheckConnections()
        {
            var list = new List<object>();

            lock (_connections)
            {
                foreach (var connection in _connections.Keys)
                {
                    var isOpen = InternalConnectionHelper.IsOpen(connection);

                    if (!isOpen)
                        list.Add(connection);
                }
            }

            if (list.Count > 0)
            {
                CloseConnections(list, LocalTime.Default.Now);
            }
        }

        private void CloseConnections(IEnumerable<object> connections, DateTime endDate)
        {
            foreach (var connection in connections)
                ConnectionClosed(connection, endDate);
        }

        private void Enqueue(ISqlLogItem item)
        {
            lock (_queue)
                _queue.Enqueue(item);

            _queueEvent.Set();
        }

        /// <summary>
        /// Logs an <see cref="IDbConnection.Open"/> method call.
        /// </summary>
        /// <param name="applicationId">The identifier of the application.</param>
        /// <param name="connection"></param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="hostName">The name of the host.</param>
        /// <param name="startDate">The start date of the connection.</param>
        /// <param name="duration">The duration of the <see cref="IDbConnection.Open"/> method in milliseconds.</param>
        /// <param name="exception">The exception caught after <see cref="IDbConnection.Open"/>. Can be <c>null</c>.</param>
        /// <returns>The identifier of the connection.</returns>
        public int ConnectionOpen(
            int applicationId,
            IDbConnection connection,
            string userName,
            string hostName,
            DateTime startDate,
            long duration,
            Exception exception)
        {
            var internalConnection = InternalConnectionHelper.GetInternalConnection(connection);
            SqlLogConnection sqlLogConnection = null;

            if (internalConnection != null)
                _connections.TryGetValue(internalConnection, out sqlLogConnection);

            var isNew = sqlLogConnection == null;
            int connectionNo;

            if (sqlLogConnection != null)
                connectionNo = sqlLogConnection.ConnectionNo;
            else
            {
                connectionNo = Interlocked.Increment(ref _connectionCounter);
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
                var name = sqlConnectionStringBuilder.ApplicationName;
                sqlLogConnection = new SqlLogConnection(applicationId, connectionNo, name, userName, hostName, startDate, duration, exception);

                if (internalConnection != null)
                {
                    lock (_connections)
                        _connections.Add(internalConnection, sqlLogConnection);
                }

                var trace = new StackTrace(2, true).ToString();
                Log.Trace("LoggedSqlConnection.Open() succeeded. ConnectionNo: {0}\r\n{1}", connectionNo, trace);

                Enqueue(sqlLogConnection);
            }

            return connectionNo;
        }

        /// <summary>
        /// Calls <see cref="IDbConnection.Close">IDbConnection.Close</see>.
        /// Logs the event if the connection is removed from the pool.
        /// </summary>
        /// <param name="connection"></param>
        public void CloseConnection(IDbConnection connection)
        {
            var internalConnection = InternalConnectionHelper.GetInternalConnection(connection);
            connection.Close();

            if (internalConnection != null)
            {
                var isOpen = InternalConnectionHelper.IsOpen(internalConnection);

                if (!isOpen)
                {
                    var endDate = LocalTime.Default.Now;
                    ConnectionClosed(internalConnection, endDate);
                }
            }
        }

        /// <summary>
        /// Calls connection's <see cref="IDisposable.Dispose"/> method.
        /// Logs the close event if the connection is removed from the pool.
        /// </summary>
        /// <param name="connection"></param>
        public void DisposeConnection(IDbConnection connection)
        {
            var internalConnection = InternalConnectionHelper.GetInternalConnection(connection);
            connection.Dispose();

            if (internalConnection != null)
            {
                var isOpen = InternalConnectionHelper.IsOpen(internalConnection);

                if (!isOpen)
                {
                    var endDate = LocalTime.Default.Now;
                    ConnectionClosed(internalConnection, endDate);
                }
            }
        }

        /// <summary>
        /// Logs the command execution event to the database.
        /// </summary>
        /// <param name="applicationId">The identifier of the application.</param>
        /// <param name="connectionNo">The identifier of the connection.</param>
        /// <param name="command"></param>
        /// <param name="startDate">The start date of the command execution.</param>
        /// <param name="duration">The duration of the command execution in ticks (see Stopwatch).</param>
        /// <param name="exception">The exception during command execution. Can be <c>null</c>.</param>
        public void CommandExecute(
            int applicationId,
            int connectionNo,
            IDbCommand command,
            DateTime startDate,
            long duration,
            Exception exception)
        {
            var commands = _applications[applicationId];
            var item = new SqlLogCommand(applicationId, commands, connectionNo, command, startDate, duration, exception);
            Enqueue(item);
        }
    }
}