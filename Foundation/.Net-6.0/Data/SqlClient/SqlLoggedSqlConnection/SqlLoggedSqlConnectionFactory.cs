using System.Data;
using Foundation.Core;
using Foundation.Threading;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlLoggedSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlLog.SqlLog _sqlLog;
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
            _filter = filter;
            _sqlLog = new SqlLog.SqlLog(connectionString);
            _applicationId = _sqlLog.ApplicationStart(applicationName, LocalTime.Default.Now, false);
        }

        WorkerThread IDbConnectionFactory.Thread => _sqlLog.Thread;

        IDbConnection IDbConnectionFactory.CreateConnection(
            string connectionString,
            string userName,
            string hostName)
        {
            return new SqlLoggedSqlConnection(_sqlLog, _applicationId, userName, hostName, connectionString, _filter);
        }

        IDbConnectionHelper IDbConnectionFactory.CreateConnectionHelper(IDbConnection connection)
        {
            var loggedSqlConnection = (SqlLoggedSqlConnection)connection;
            var sqlConnection = loggedSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}
