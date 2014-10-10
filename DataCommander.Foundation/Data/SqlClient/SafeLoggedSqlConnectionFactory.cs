namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class SafeLoggedSqlConnectionFactory : IDbConnectionFactory
    {
        private SqlLog sqlLog;

        private Int32 applicationId;

        private readonly ISqlLoggedSqlCommandFilter filter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="applicationName"></param>
        /// <param name="filter"></param>
        public SafeLoggedSqlConnectionFactory(
            String connectionString,
            String applicationName,
            ISqlLoggedSqlCommandFilter filter)
        {
            this.filter = filter;
            this.sqlLog = new SqlLog(connectionString);
            this.applicationId = this.sqlLog.ApplicationStart(applicationName, OptimizedDateTime.Now, true);
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
        public IDbConnection CreateConnection(String connectionString, String userName, String hostName)
        {
            return new SafeLoggedSqlConnection(
                this.sqlLog,
                this.applicationId,
                userName,
                hostName,
                connectionString,
                this.filter);
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