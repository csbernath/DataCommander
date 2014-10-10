namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlLoggedSqlConnectionFactory : IDbConnectionFactory
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
        public SqlLoggedSqlConnectionFactory(
            String connectionString,
            String applicationName,
            ISqlLoggedSqlCommandFilter filter)
        {
            this.filter = filter;
            this.sqlLog = new SqlLog(connectionString);
            this.applicationId = this.sqlLog.ApplicationStart(applicationName, OptimizedDateTime.Now, false);
        }

        WorkerThread IDbConnectionFactory.Thread
        {
            get
            {
                return this.sqlLog.Thread;
            }
        }

        IDbConnection IDbConnectionFactory.CreateConnection(
            String connectionString,
            String userName,
            String hostName)
        {
            return new SqlLoggedSqlConnection(this.sqlLog, this.applicationId, userName, hostName, connectionString, this.filter);
        }

        IDbConnectionHelper IDbConnectionFactory.CreateConnectionHelper(IDbConnection connection)
        {
            SqlLoggedSqlConnection loggedSqlConnection = (SqlLoggedSqlConnection)connection;
            SqlConnection sqlConnection = loggedSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}
