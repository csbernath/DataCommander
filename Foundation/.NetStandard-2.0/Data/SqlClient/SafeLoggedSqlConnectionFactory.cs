using System.Data;
using System.Threading;
using Foundation.Core;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;
using Foundation.Threading;

namespace Foundation.Data.SqlClient
{
    public class SafeLoggedSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly SqlLog.SqlLog _sqlLog;
        private readonly int _applicationId;
        private readonly ISqlLoggedSqlCommandFilter _filter;

        public SafeLoggedSqlConnectionFactory(string connectionString, string applicationName, ISqlLoggedSqlCommandFilter filter)
        {
            _sqlLog = new SqlLog.SqlLog(connectionString);
            _applicationId = _sqlLog.ApplicationStart(applicationName, LocalTime.Default.Now, true);
            _filter = filter;
        }

        public WorkerThread Thread => _sqlLog.Thread;

        public IDbConnection CreateConnection(string connectionString, string userName, string hostName) =>
            new SafeLoggedSqlConnection(_sqlLog, _applicationId, userName, hostName, connectionString, _filter, CancellationToken.None);

        public IDbConnectionHelper CreateConnectionHelper(IDbConnection connection)
        {
            var safeLoggedSqlConnection = (SafeLoggedSqlConnection) connection;
            var loggedSqlConnection = (SqlLoggedSqlConnection.SqlLoggedSqlConnection) safeLoggedSqlConnection.Connection;
            var sqlConnection = loggedSqlConnection.Connection;
            return new SqlConnectionFactory(sqlConnection, connection);
        }
    }
}