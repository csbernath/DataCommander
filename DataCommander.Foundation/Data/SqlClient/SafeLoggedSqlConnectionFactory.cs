namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class SafeLoggedSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlLog sqlLog;

        private readonly int applicationId;

        private readonly ISqlLoggedSqlCommandFilter filter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="applicationName"></param>
        /// <param name="filter"></param>
        public SafeLoggedSqlConnectionFactory(
            string connectionString,
            string applicationName,
            ISqlLoggedSqlCommandFilter filter)
        {
            this.filter = filter;
            this.sqlLog = new SqlLog(connectionString);
            this.applicationId = this.sqlLog.ApplicationStart(applicationName, LocalTime.Default.Now, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThread Thread
        {
            get
            {
                return this.sqlLog.Thread;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection(string connectionString, string userName, string hostName)
        {
            return new SafeLoggedSqlConnection(
                this.sqlLog,
                this.applicationId,
                userName,
                hostName,
                connectionString,
                this.filter,
                CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
        {
            SafeLoggedSqlConnection safeLoggedSqlConnection = (SafeLoggedSqlConnection)connection;
            SqlLoggedSqlConnection loggedSqlConnection = (SqlLoggedSqlConnection)safeLoggedSqlConnection.Connection;
            SqlConnection sqlConnection = loggedSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}