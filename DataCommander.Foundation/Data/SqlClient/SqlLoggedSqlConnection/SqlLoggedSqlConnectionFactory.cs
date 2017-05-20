namespace DataCommander.Foundation.Data.SqlClient.SqlLoggedSqlConnection
{
    using System.Data;
    using DataCommander.Foundation.Data.SqlClient.SqlLog;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlLoggedSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlLog _sqlLog;
        private readonly int _applicationId;
        private readonly ISqlLoggedSqlCommandFilter _filter;

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
            this._filter = filter;
            this._sqlLog = new SqlLog(connectionString);
            this._applicationId = this._sqlLog.ApplicationStart(applicationName, LocalTime.Default.Now, false);
        }

        WorkerThread IDbConnectionFactory.Thread => this._sqlLog.Thread;

        IDbConnection IDbConnectionFactory.CreateConnection(
            string connectionString,
            string userName,
            string hostName)
        {
            return new SqlLoggedSqlConnection(this._sqlLog, this._applicationId, userName, hostName, connectionString, this._filter);
        }

        IDbConnectionHelper IDbConnectionFactory.CreateConnectionHelper(IDbConnection connection)
        {
            var loggedSqlConnection = (SqlLoggedSqlConnection)connection;
            var sqlConnection = loggedSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}
