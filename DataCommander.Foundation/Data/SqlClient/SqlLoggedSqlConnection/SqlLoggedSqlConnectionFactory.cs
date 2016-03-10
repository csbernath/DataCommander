namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlLoggedSqlConnectionFactory : IDbConnectionFactory
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
        public SqlLoggedSqlConnectionFactory(
            string connectionString,
            string applicationName,
            ISqlLoggedSqlCommandFilter filter)
        {
            this.filter = filter;
            this.sqlLog = new SqlLog(connectionString);
            this.applicationId = this.sqlLog.ApplicationStart(applicationName, LocalTime.Default.Now, false);
        }

        WorkerThread IDbConnectionFactory.Thread => this.sqlLog.Thread;

        IDbConnection IDbConnectionFactory.CreateConnection(
            string connectionString,
            string userName,
            string hostName)
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
