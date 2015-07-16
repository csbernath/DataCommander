namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Text;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// Safe SQL Server connection for Windows Services.
    /// </summary>
    public class SafeSqlConnection : SafeDbConnection, ISafeDbConnection, ICloneable
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private Int16 id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SafeSqlConnection( string connectionString )
        {
            SqlConnection connection = new SqlConnection( connectionString );
            this.Initialize( connection, this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public SafeSqlConnection( IDbConnection connection )
        {
            this.Initialize( connection, this );
        }

        #region ICloneable Members
        object ICloneable.Clone()
        {
            string connectionString = this.ConnectionString;
            var connection = new SafeSqlConnection( connectionString );
            return connection;
        }
        #endregion

        internal static Int16 GetId(IDbConnection connection)
        {
            var transactionScope = new DbTransactionScope(connection, null);
            Int16 id = 0;

            try
            {
                id = (Int16)transactionScope.ExecuteScalar(new CommandDefinition {CommandText = "select @@spid"});
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, "Exception:\r\n{0}", e.ToLogString());
            }

            return id;
        }

        object ISafeDbConnection.Id
        {
            get
            {
                if (this.id == 0)
                {
                    this.id = GetId( this.Connection );
                }

                return this.id;
            }
        }

        internal static void HandleException(
            IDbConnection connection,
            Exception exception,
            TimeSpan elapsed )
        {
            string separator = new string( '-', 80 );
            var sb = new StringBuilder();
            sb.AppendFormat( "SafeSqlConnection.HandleException(connection), elapsed: {0}, exception:\r\n{1}", elapsed, exception.ToLogString() );            
            var sqlException = exception as SqlException;
            bool handled = false;
            int timeout = 1 * 60 * 1000; // 1 minutes

            if (sqlException != null)
            {
                switch (sqlException.Number)
                {
                    case -2: // Timeout expired. The timeout period elapsed prior completion of the operation or the server is not responding.
                        handled = true;
                        timeout = 500; // 500 milliseconds
                        break;

                    case 2: // A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections
                        handled = true;
                        timeout = 5 * 1000; // 5 seconds
                        break;

                    case 4060: // Cannot open database requested in login '%.*ls'. Login fails.
                        handled = true;
                        timeout = 5 * 1000; // 5 seconds;
                        break;

                    default:
                        if (connection.State != ConnectionState.Open)
                        {
                            handled = true;
                        }

                        break;
                }
            }
            else
            {
                var win32Exception = exception as Win32Exception;
                if (win32Exception != null)
                {
                    // The wait operation timed out
                    if (win32Exception.NativeErrorCode == 258)
                    {
                        timeout = 0;
                    }
                }
                else if (connection.State != ConnectionState.Open)
                {
                    handled = true;
                }
            }

            if (handled)
            {
                sb.AppendFormat( "\r\nWaiting {0}...", TimeSpan.FromMilliseconds( timeout ) );
                log.Error( sb.ToString() );

                if (timeout > 0)
                {
                    WorkerThread thread = WorkerThread.Current;
                    thread.WaitForStop( timeout );
                }
            }
            else
            {
                throw exception;
            }
        }

        void ISafeDbConnection.HandleException( Exception exception, TimeSpan elapsed )
        {
            HandleException( this.Connection, exception, elapsed );
        }

        internal static void HandleException(
            Exception exception,
            IDbCommand command )
        {
            Contract.Requires( command != null );

            string separator = new string( '-', 80 );
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "SafeSqlConnection.HandleException(command):\r\n" );
            SqlParameterCollection parameters = (SqlParameterCollection) command.Parameters;
            string p = parameters.ToLogString();
            string database = command.Connection.Database;

            sb.AppendFormat( "Database: {0}\r\n", database );
            sb.AppendFormat( "Command: {0}\r\n{1}\r\n{2}\r\n", command.CommandText, p, separator );
            sb.AppendFormat( "Exception:{0}\r\n{1}\r\n", exception, separator );
            SqlException sqlEx = exception as SqlException;
            bool handled = false;

            if (sqlEx != null)
            {
                sb.AppendFormat( "{0}\r\n{1}\r\n", sqlEx.Errors.ToLogString(), separator );

                switch (sqlEx.Number)
                {
                    case 11: // General network error.  Check your network documentation.
                        handled = true;
                        break;

                    case 922: //Database '%.*ls' is being recovered. Waiting until recovery is finished.
                        handled = true;
                        break;

                    case 1205: // Transaction (Process ID %d) was deadlocked on {%Z} resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
                        handled = true;
                        break;
                }
            }

            log.Write( LogLevel.Error, sb.ToString() );

            if (handled)
            {
                WorkerThread thread = WorkerThread.Current;
                thread.WaitForStop( 1 * 60 * 1000 );// 1 minutes
            }
            else
            {
                throw exception;
            }
        }

        void ISafeDbConnection.HandleException( Exception exception, IDbCommand command )
        {
            HandleException( exception, command );
        }
    }
}