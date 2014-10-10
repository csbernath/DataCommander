namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Threading;

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

        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private static IInternalConnectionHelper internalConnectionHelper;
        private WorkerThread thread;
        private Int32 connectionCounter;
        private SafeSqlConnection connection;
        private Dictionary<Int32, Dictionary<String, SqLoglCommandExecution>> applications = new Dictionary<Int32, Dictionary<String, SqLoglCommandExecution>>();
        private Dictionary<Object, SqlLogConnection> connections = new Dictionary<Object, SqlLogConnection>();
        private Queue<ISqlLogItem> queue = new Queue<ISqlLogItem>();
        private AutoResetEvent queueEvent = new AutoResetEvent( false );

        #endregion

        static SqlLog()
        {
            Version version = Environment.Version;
            Int32 major = version.Major;

            if (major == 1)
            {
                internalConnectionHelper = new InternalConnectionHelper();
            }
            else
            {
                internalConnectionHelper = new InternalConnectionHelper2();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLog"/> class.
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlLog( String connectionString )
        {
            this.thread = new WorkerThread( this.Start )
            {
                Name = typeof (SqlLog).Name,
                Priority = ThreadPriority.Lowest
            };

            this.connection = new SafeSqlConnection( connectionString );
        }

        /// <summary>
        /// Gets the logger thread.
        /// </summary>
        public WorkerThread Thread
        {
            get
            {
                return this.thread;
            }
        }

        private void Flush()
        {
            while (this.queue.Count > 0)
            {
                ISqlLogItem[] array;

                lock (this.queue)
                {
                    array = new ISqlLogItem[this.queue.Count];
                    this.queue.CopyTo( array, 0 );
                    this.queue.Clear();
                }

                StringBuilder sb = new StringBuilder();

                for (Int32 i = 0; i < array.Length; i++)
                {
                    ISqlLogItem item = array[ i ];
                    String commandText = item.CommandText;
                    sb.Append( commandText );
                }

                String cmdText = sb.ToString();
                Int64 ticks = 0;

                try
                {
                    IDbCommand command = this.connection.CreateCommand();
                    command.CommandText = cmdText;
                    command.CommandTimeout = 259200;
                    ticks = Stopwatch.GetTimestamp();
                    command.ExecuteNonQuery();
                    ticks = Stopwatch.GetTimestamp() - ticks;
                }
                catch ( Exception e )
                {
                    log.Write( LogLevel.Error, e.ToString() );
                }
                finally
                {
                    Double seconds = (Double) ticks/Stopwatch.Frequency;
                    Int32 speed = (Int32) ( array.Length/seconds );

                    log.Write(
                        LogLevel.Trace,
                        "SqlLog.Flush() called. Count: {0}, Elapsed: {1}, Speed: {2} item/sec\r\n{3}",
                        array.Length,
                        StopwatchTimeSpan.ToString( ticks, 3 ),
                        speed,
                        cmdText );
                }
            }
        }

        private void Start()
        {
            WaitHandle[] waitHandles = {this.thread.StopRequest, this.queueEvent};

            while (!this.thread.IsStopRequested)
            {
                Int32 i = WaitHandle.WaitAny( waitHandles );
                this.CheckConnections();

                if (i == 0)
                {
                    List<Object> list;

                    lock (this.connections)
                    {
                        list = this.connections.Keys.ToList();
                    }

                    this.CloseConnections( list, OptimizedDateTime.Now );
                }

                this.Flush();
            }

            DateTime endDate = OptimizedDateTime.Now;

            lock (this.applications)
            {
                foreach (Int32 applicationId in this.applications.Keys)
                {
                    SqlLogApplicationEnd item = new SqlLogApplicationEnd( applicationId, endDate );
                    this.Enqueue( item );
                }
            }

            this.Flush();

            log.Trace("queue.Count: {0}", this.queue.Count );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startDate"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        public Int32 ApplicationStart( String name, DateTime startDate, Boolean safe )
        {
            var sb = new StringBuilder();
            sb.Append( "exec LogApplicationStart " );
            sb.Append( name.ToTSqlVarChar() );
            sb.Append( ',' );
            sb.Append( startDate.ToTSqlDateTime() );
            String commandText = sb.ToString();
            Int32 applicationId = 0;

            if (this.connection.State != ConnectionState.Open)
            {
                if (safe)
                {
                    this.connection.Open();
                }
                else
                {
                    this.connection.Connection.Open();
                }
            }

            applicationId = (Int32) this.connection.Connection.ExecuteScalar( null, commandText, CommandType.Text, 0 );
            log.Trace("SqlLog.ApplicationStart({0})", applicationId );
            var commands = new Dictionary<String, SqLoglCommandExecution>();

            lock (this.applications)
            {
                this.applications.Add( applicationId, commands );
            }

            return applicationId;
        }

        /// <summary>
        /// Executes the <c>LogApplicationEnd</c> stored procedure in the SQL Server database.
        /// </summary>
        /// <param name="applicationId">The identifier of the application. See <see cref="ApplicationStart"/>.</param>
        /// <param name="endDate">The end date of the application.</param>
        public void ApplicationEnd( Int32 applicationId, DateTime endDate )
        {
            SqlLogApplicationEnd item = new SqlLogApplicationEnd( applicationId, endDate );
            this.Enqueue( item );

            lock (this.applications)
            {
                this.applications.Remove( applicationId );
            }
        }

        private void ConnectionClosed(
            Object internalConnection,
            DateTime endDate )
        {
            SqlLogConnection sqlLogConnection = this.connections[ internalConnection ];

            lock (this.connections)
            {
                this.connections.Remove( internalConnection );
            }

            if (sqlLogConnection != null)
            {
                Int32 applicationId = sqlLogConnection.ApplicationId;
                Int32 connectionNo = sqlLogConnection.ConnectionNo;
                SqlLogConnectionClose item = new SqlLogConnectionClose( applicationId, connectionNo, endDate );
                this.Enqueue( item );
            }
        }

        private void CheckConnections()
        {
            List<Object> list = new List<Object>();

            lock (this.connections)
            {
                foreach (Object connection in this.connections.Keys)
                {
                    Boolean isOpen = internalConnectionHelper.IsOpen( connection );

                    if (!isOpen)
                    {
                        list.Add( connection );
                    }
                }
            }

            if (list.Count > 0)
            {
                this.CloseConnections( list, OptimizedDateTime.Now );
            }
        }

        private void CloseConnections( IEnumerable<Object> connections, DateTime endDate )
        {
            foreach (Object connection in connections)
            {
                this.ConnectionClosed( connection, endDate );
            }
        }

        private void Enqueue( ISqlLogItem item )
        {
            lock (this.queue)
            {
                this.queue.Enqueue( item );
            }

            this.queueEvent.Set();
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
        public Int32 ConnectionOpen(
            Int32 applicationId,
            IDbConnection connection,
            String userName,
            String hostName,
            DateTime startDate,
            Int64 duration,
            Exception exception )
        {
            Object internalConnection = internalConnectionHelper.GetInternalConnection( connection );
            SqlLogConnection sqlLogConnection = null;

            if (internalConnection != null)
            {
                this.connections.TryGetValue( internalConnection, out sqlLogConnection );
            }

            Boolean isNew = sqlLogConnection == null;
            Int32 connectionNo;

            if (sqlLogConnection != null)
            {
                connectionNo = sqlLogConnection.ConnectionNo;
            }
            else
            {
                connectionNo = Interlocked.Increment( ref this.connectionCounter );
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder( connection.ConnectionString );
                String name = sqlConnectionStringBuilder.ApplicationName;
                sqlLogConnection = new SqlLogConnection( applicationId, connectionNo, name, userName, hostName, startDate, duration, exception );

                if (internalConnection != null)
                {
                    lock (this.connections)
                    {
                        this.connections.Add( internalConnection, sqlLogConnection );
                    }
                }

                String trace = new StackTrace( 2, true ).ToString();
                log.Trace("LoggedSqlConnection.Open() succeeded. ConnectionNo: {0}\r\n{1}", connectionNo, trace );

                this.Enqueue( sqlLogConnection );
            }

            return connectionNo;
        }

        /// <summary>
        /// Calls <see cref="IDbConnection.Close">IDbConnection.Close</see>.
        /// Logs the event if the connection is removed from the pool.
        /// </summary>
        /// <param name="connection"></param>
        public void CloseConnection( IDbConnection connection )
        {
            Object internalConnection = internalConnectionHelper.GetInternalConnection( connection );
            connection.Close();

            if (internalConnection != null)
            {
                Boolean isOpen = internalConnectionHelper.IsOpen( internalConnection );

                if (!isOpen)
                {
                    DateTime endDate = OptimizedDateTime.Now;
                    this.ConnectionClosed( internalConnection, endDate );
                }
            }
        }

        /// <summary>
        /// Calls connection's <see cref="IDisposable.Dispose"/> method.
        /// Logs the close event if the connection is removed from the pool.
        /// </summary>
        /// <param name="connection"></param>
        public void DisposeConnection( IDbConnection connection )
        {
            Object internalConnection = internalConnectionHelper.GetInternalConnection( connection );
            connection.Dispose();

            if (internalConnection != null)
            {
                Boolean isOpen = internalConnectionHelper.IsOpen( internalConnection );

                if (!isOpen)
                {
                    DateTime endDate = OptimizedDateTime.Now;
                    this.ConnectionClosed( internalConnection, endDate );
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
            Int32 applicationId,
            Int32 connectionNo,
            IDbCommand command,
            DateTime startDate,
            Int64 duration,
            Exception exception )
        {
            Dictionary<String, SqLoglCommandExecution> commands = this.applications[ applicationId ];
            var item = new SqlLogCommand( applicationId, commands, connectionNo, command, startDate, duration, exception );
            this.Enqueue( item );
        }
    }
}